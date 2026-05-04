using TimePoker.Domain;
using Xunit;

namespace TimePoker.Domain.Tests;

public class CronometroTests
{
    private static Torneio TorneioCom(params Nivel[] niveis) => new()
    {
        Estrutura = new Estrutura
        {
            Nome = "Teste", BuyIn = 70, ValorRebuy = 50,
            Niveis = niveis.ToList()
        }
    };

    [Fact]
    public void Novo_EstadoEhAguardando_E_TempoEhDuracaoDoNivelAtual()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        Assert.Equal(EstadoTorneio.Aguardando, c.Estado);
        Assert.Equal(TimeSpan.FromMinutes(10), c.Restante);
    }

    [Fact]
    public void Iniciar_MudaEstadoParaRodando()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        c.Iniciar();
        Assert.Equal(EstadoTorneio.Rodando, c.Estado);
    }

    [Fact]
    public void Tick_AguardandoOuPausado_NaoConsome()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        var inicial = c.Restante;
        c.Tick(TimeSpan.FromMinutes(2));   // aguardando
        Assert.Equal(inicial, c.Restante);

        c.Iniciar();
        c.Pausar();
        c.Tick(TimeSpan.FromMinutes(2));   // pausado
        Assert.Equal(inicial, c.Restante);
    }

    [Fact]
    public void Tick_Rodando_DescontaTempo()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        c.Iniciar();
        c.Tick(TimeSpan.FromMinutes(3));
        Assert.Equal(TimeSpan.FromMinutes(7), c.Restante);
    }

    [Fact]
    public void Tick_Zerando_DisparaFimDeNivel_E_AvancaProProximo()
    {
        var c = new Cronometro(TorneioCom(
            Nivel.DeJogo(25, 50, 10),
            Nivel.DeJogo(50, 100, 15)));
        var disparado = false;
        c.FimDeNivel += () => disparado = true;

        c.Iniciar();
        c.Tick(TimeSpan.FromMinutes(10));
        Assert.True(disparado);
        Assert.Equal(1, c.Torneio.IndiceNivelAtual);
        Assert.Equal(TimeSpan.FromMinutes(15), c.Restante);
        Assert.Equal(EstadoTorneio.Rodando, c.Estado);
    }

    [Fact]
    public void Tick_ZerandoNoUltimoNivel_FicaEncerrado()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        c.Iniciar();
        c.Tick(TimeSpan.FromMinutes(10));
        Assert.Equal(EstadoTorneio.Encerrado, c.Estado);
        Assert.Equal(TimeSpan.Zero, c.Restante);
    }

    [Fact]
    public void ProximoNivel_Manual_RecarregaTempo()
    {
        var c = new Cronometro(TorneioCom(
            Nivel.DeJogo(25, 50, 10),
            Nivel.DeJogo(50, 100, 30)));
        c.Iniciar();
        c.Tick(TimeSpan.FromMinutes(2));   // 8 min restantes
        Assert.True(c.ProximoNivel());
        Assert.Equal(1, c.Torneio.IndiceNivelAtual);
        Assert.Equal(TimeSpan.FromMinutes(30), c.Restante);
    }

    [Fact]
    public void NivelAnterior_VoltaERecarregaTempo()
    {
        var c = new Cronometro(TorneioCom(
            Nivel.DeJogo(25, 50, 10),
            Nivel.DeJogo(50, 100, 15)));
        c.ProximoNivel();
        c.Tick(TimeSpan.FromMinutes(5));
        Assert.True(c.NivelAnterior());
        Assert.Equal(0, c.Torneio.IndiceNivelAtual);
        Assert.Equal(TimeSpan.FromMinutes(10), c.Restante);
    }

    [Fact]
    public void AjustarTempo_AdicionaTempoSemUltrapassarZero()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        c.AjustarTempo(TimeSpan.FromMinutes(5));    // +5 → 15min
        Assert.Equal(TimeSpan.FromMinutes(15), c.Restante);
        c.AjustarTempo(TimeSpan.FromMinutes(-50));  // não vai negativo
        Assert.Equal(TimeSpan.Zero, c.Restante);
    }

    [Fact]
    public void Restaurar_RetomaIndiceETempo_E_NaoRetomaAutomatico()
    {
        var c = new Cronometro(TorneioCom(
            Nivel.DeJogo(25, 50, 10),
            Nivel.DeJogo(50, 100, 15),
            Nivel.DeJogo(100, 200, 20)));
        c.Restaurar(indiceNivel: 1,
                    tempoRestante: TimeSpan.FromMinutes(7),
                    estado: EstadoTorneio.Rodando);
        Assert.Equal(1, c.Torneio.IndiceNivelAtual);
        Assert.Equal(TimeSpan.FromMinutes(7), c.Restante);
        // Por segurança, restaurações de "Rodando" voltam pausadas
        Assert.Equal(EstadoTorneio.Pausado, c.Estado);
    }

    [Fact]
    public void Restaurar_IndiceInvalido_NaoQuebra()
    {
        var c = new Cronometro(TorneioCom(Nivel.DeJogo(25, 50, 10)));
        c.Restaurar(99, TimeSpan.FromMinutes(1), EstadoTorneio.Pausado);
        Assert.Equal(0, c.Torneio.IndiceNivelAtual);    // ficou no original
    }

    [Fact]
    public void Resetar_VoltaParaNivelZero_E_Aguardando()
    {
        var c = new Cronometro(TorneioCom(
            Nivel.DeJogo(25, 50, 10),
            Nivel.DeJogo(50, 100, 15)));
        c.Iniciar();
        c.ProximoNivel();
        c.Resetar();
        Assert.Equal(0, c.Torneio.IndiceNivelAtual);
        Assert.Equal(EstadoTorneio.Aguardando, c.Estado);
        Assert.Equal(TimeSpan.FromMinutes(10), c.Restante);
    }
}
