using TimePoker.Domain;
using Xunit;

namespace TimePoker.Domain.Tests;

public class TorneioTests
{
    private static Torneio Novo(params Nivel[] niveis) => new()
    {
        Estrutura = new Estrutura
        {
            Nome = "Teste",
            BuyIn = 70,
            ValorRebuy = 50,
            Niveis = niveis.ToList()
        }
    };

    [Fact]
    public void Novo_ComecaNoNivelZero()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        Assert.Equal(0, t.IndiceNivelAtual);
        Assert.Equal(25, t.NivelAtual()!.SmallBlind);
    }

    [Fact]
    public void AvancarNivel_AtualizaIndice()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        Assert.True(t.AvancarNivel());
        Assert.Equal(1, t.IndiceNivelAtual);
        Assert.Equal(50, t.NivelAtual()!.SmallBlind);
    }

    [Fact]
    public void AvancarNivel_NoUltimo_RetornaFalse()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10));
        Assert.False(t.AvancarNivel());
    }

    [Fact]
    public void VoltarNivel_NoPrimeiro_RetornaFalse()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        Assert.False(t.VoltarNivel());
    }

    [Fact]
    public void VoltarNivel_DepoisDeAvancar_VoltaParaAnterior()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        t.AvancarNivel();
        Assert.True(t.VoltarNivel());
        Assert.Equal(0, t.IndiceNivelAtual);
    }

    [Fact]
    public void NivelAnterior_NoPrimeiro_RetornaNull()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        Assert.Null(t.NivelAnterior());
    }

    [Fact]
    public void NivelProximo_NoUltimo_RetornaNull()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10));
        Assert.Null(t.NivelProximo());
    }

    [Fact]
    public void CalcularPrizePool_ConsideraJogadoresERebuys()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10));
        t.Jogadores = 9;     // 9 × 70 = 630
        t.Rebuys = 2;        // 2 × 50 = 100
        Assert.Equal(730, t.CalcularPrizePool());
    }

    [Fact]
    public void CalcularPrizePool_SemRebuys_FazSomaSimples()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10));
        t.Jogadores = 10;
        t.Rebuys = 0;
        Assert.Equal(700, t.CalcularPrizePool());
    }

    [Fact]
    public void Resetar_VoltaParaInicio_E_ZeraContadores()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10));
        t.AvancarNivel();
        t.Jogadores = 8;
        t.Rebuys = 3;
        t.Resetar();
        Assert.Equal(0, t.IndiceNivelAtual);
        Assert.Equal(0, t.Jogadores);
        Assert.Equal(0, t.Rebuys);
    }

    [Fact]
    public void IrParaNivel_Valido_PuloDireto()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 10), Nivel.DeJogo(100, 200, 10));
        Assert.True(t.IrParaNivel(2));
        Assert.Equal(2, t.IndiceNivelAtual);
    }

    [Fact]
    public void IrParaNivel_ForaDoLimite_RetornaFalse()
    {
        var t = Novo(Nivel.DeJogo(25, 50, 10));
        Assert.False(t.IrParaNivel(5));
        Assert.False(t.IrParaNivel(-1));
    }
}
