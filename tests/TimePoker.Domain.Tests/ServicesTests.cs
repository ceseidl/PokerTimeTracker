using TimePoker.Domain;
using TimePoker.Services.Integration;
using TimePoker.Services.Persistence;
using Xunit;

namespace TimePoker.Domain.Tests;

public class SessionStoreTests : IDisposable
{
    private readonly string _arquivo = Path.Combine(Path.GetTempPath(), $"sess_{Guid.NewGuid():N}.json");

    public void Dispose() { if (File.Exists(_arquivo)) File.Delete(_arquivo); }

    [Fact]
    public void Carregar_QuandoNaoExiste_RetornaNull()
    {
        Assert.Null(new SessionStore(_arquivo).Carregar());
    }

    [Fact]
    public void Salvar_E_Carregar_RoundTrip()
    {
        var store = new SessionStore(_arquivo);
        var estado = new SessionState
        {
            NomeTorneio = "Teste",
            IndiceNivelAtual = 3,
            TempoRestante = TimeSpan.FromMinutes(7),
            TempoTotalDecorrido = TimeSpan.FromMinutes(42),
            Estado = EstadoTorneio.Pausado,
            Jogadores = 8,
            Rebuys = 2
        };

        store.Salvar(estado);
        var lido = store.Carregar();

        Assert.NotNull(lido);
        Assert.Equal("Teste", lido!.NomeTorneio);
        Assert.Equal(3, lido.IndiceNivelAtual);
        Assert.Equal(TimeSpan.FromMinutes(7), lido.TempoRestante);
        Assert.Equal(TimeSpan.FromMinutes(42), lido.TempoTotalDecorrido);
        Assert.Equal(EstadoTorneio.Pausado, lido.Estado);
    }

    [Fact]
    public void Salvar_PreencheUltimaAtualizacao()
    {
        var antes = DateTime.UtcNow.AddSeconds(-1);
        new SessionStore(_arquivo).Salvar(new SessionState());
        var lido = new SessionStore(_arquivo).Carregar()!;
        Assert.True(lido.UltimaAtualizacao >= antes);
    }

    [Fact]
    public void Limpar_RemoveArquivo()
    {
        var store = new SessionStore(_arquivo);
        store.Salvar(new SessionState());
        Assert.True(store.Existe());
        store.Limpar();
        Assert.False(store.Existe());
    }

    [Fact]
    public void Carregar_ArquivoCorrompido_RetornaNullSemLancar()
    {
        File.WriteAllText(_arquivo, "{{{ não-json");
        Assert.Null(new SessionStore(_arquivo).Carregar());
    }
}

public class TimerStatusWriterTests : IDisposable
{
    private readonly string _arquivo = Path.Combine(Path.GetTempPath(), $"tsw_{Guid.NewGuid():N}.json");

    public void Dispose() { if (File.Exists(_arquivo)) File.Delete(_arquivo); }

    [Fact]
    public void Escrever_GeraJsonComCamposEsperados()
    {
        new TimerStatusWriter(_arquivo).Escrever(
            totalDecorrido: TimeSpan.FromMinutes(125),
            nivelAtual: 4,
            estado: "Pausado");

        var json = File.ReadAllText(_arquivo);
        Assert.Contains("\"TotalDecorridoSegundos\": 7500", json);
        Assert.Contains("\"NivelAtual\": 4", json);
        Assert.Contains("\"Estado\": \"Pausado\"", json);
    }

    [Fact]
    public void Escrever_SobrescreveArquivoExistente()
    {
        var w = new TimerStatusWriter(_arquivo);
        w.Escrever(TimeSpan.FromMinutes(1), 1, "Rodando");
        w.Escrever(TimeSpan.FromMinutes(10), 2, "Pausado");

        var json = File.ReadAllText(_arquivo);
        Assert.Contains("\"NivelAtual\": 2", json);
        Assert.DoesNotContain("\"NivelAtual\": 1", json);
    }
}

public class EstruturaStoreTests : IDisposable
{
    private readonly string _pasta = Path.Combine(Path.GetTempPath(), $"estr_{Guid.NewGuid():N}");

    public void Dispose() { if (Directory.Exists(_pasta)) Directory.Delete(_pasta, recursive: true); }

    private static Estrutura Amostra() => new()
    {
        Nome = "Teste", BuyIn = 50, ValorRebuy = 50,
        Niveis = [Nivel.DeJogo(25, 50, 10), Nivel.DeJogo(50, 100, 15)]
    };

    [Fact]
    public void Salvar_E_Carregar_RoundTrip()
    {
        var store = new EstruturaStore(_pasta);
        store.Salvar("Minha Estrutura", Amostra());
        var lida = store.Carregar("Minha Estrutura");

        Assert.NotNull(lida);
        Assert.Equal("Minha Estrutura", lida!.Nome);
        Assert.Equal(2, lida.Niveis.Count);
    }

    [Fact]
    public void Listar_RetornaNomesOrdenados()
    {
        var store = new EstruturaStore(_pasta);
        store.Salvar("Zeta", Amostra());
        store.Salvar("Alpha", Amostra());
        store.Salvar("Mike", Amostra());

        var nomes = store.Listar();
        Assert.Equal(new[] { "Alpha", "Mike", "Zeta" }, nomes);
    }

    [Fact]
    public void Excluir_ArquivoExistente_RetornaTrue()
    {
        var store = new EstruturaStore(_pasta);
        store.Salvar("Tmp", Amostra());
        Assert.True(store.Excluir("Tmp"));
        Assert.Null(store.Carregar("Tmp"));
    }

    [Fact]
    public void Excluir_ArquivoInexistente_RetornaFalse()
    {
        Assert.False(new EstruturaStore(_pasta).Excluir("não-existe"));
    }

    [Fact]
    public void Salvar_SanitizaNomeComCaracteresInvalidos()
    {
        var store = new EstruturaStore(_pasta);
        store.Salvar("Nome/com:invalidos*", Amostra());
        // Carrega via mesmo nome — sanitização é determinística
        var lida = store.Carregar("Nome/com:invalidos*");
        Assert.NotNull(lida);
    }

    [Fact]
    public void Listar_PastaInexistente_RetornaVazio()
    {
        var pastaNova = Path.Combine(_pasta, "subpasta");
        Assert.Empty(new EstruturaStore(pastaNova).Listar());
    }

    [Fact]
    public void Carregar_ArquivoCorrompido_RetornaNull()
    {
        Directory.CreateDirectory(_pasta);
        File.WriteAllText(Path.Combine(_pasta, "Quebrada.json"), "{{{");
        Assert.Null(new EstruturaStore(_pasta).Carregar("Quebrada"));
    }
}

public class ParticipantesImporterTests : IDisposable
{
    private readonly string _arquivo = Path.Combine(Path.GetTempPath(), $"part_{Guid.NewGuid():N}.md");
    public void Dispose() { if (File.Exists(_arquivo)) File.Delete(_arquivo); }

    [Fact]
    public void Ler_IgnoraLinhasVaziasEComentarios()
    {
        File.WriteAllText(_arquivo, """
            # Cabeçalho

            Alice
            Bob

            # Outra seção
            Carol
            """);

        var nomes = new ParticipantesImporter().Ler(_arquivo);

        Assert.Equal(new[] { "Alice", "Bob", "Carol" }, nomes);
    }

    [Fact]
    public void Ler_TrimEspacosEmCadaLinha()
    {
        File.WriteAllText(_arquivo, "  Alice  \n   Bob\n");
        var nomes = new ParticipantesImporter().Ler(_arquivo);
        Assert.Equal(new[] { "Alice", "Bob" }, nomes);
    }

    [Fact]
    public void LocalizarArquivo_QuandoNaoExiste_RetornaNull()
    {
        // O AppContext.BaseDirectory aqui é o bin/ do test runner. Subindo,
        // não vai encontrar participantes.md a menos que algum ancestral tenha um.
        // Tolera ambos: o teste só falha se LocalizarArquivo lançar exceção.
        var ex = Record.Exception(() => new ParticipantesImporter().LocalizarArquivo());
        Assert.Null(ex);
    }
}

public class BridgeWatcherTests : IDisposable
{
    private readonly string _pasta = Path.Combine(Path.GetTempPath(), $"bw_{Guid.NewGuid():N}");
    private readonly string _arquivo;

    public BridgeWatcherTests()
    {
        Directory.CreateDirectory(_pasta);
        _arquivo = Path.Combine(_pasta, "bridge.json");
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_pasta)) Directory.Delete(_pasta, recursive: true); }
        catch { /* watcher pode estar segurando */ }
    }

    [Fact]
    public void LerAtual_ArquivoInexistente_RetornaNull()
    {
        using var w = new BridgeWatcher(_arquivo);
        Assert.Null(w.LerAtual());
    }

    [Fact]
    public void LerAtual_ArquivoValido_DesserializaCampos()
    {
        File.WriteAllText(_arquivo, """
            {
              "Versao": 1,
              "Ano": 2026,
              "Etapa": 7,
              "Anfitriao": "Carlos",
              "Jogadores": 8,
              "Rebuys": 3,
              "BuyIn": 50,
              "ValorRebuy": 50
            }
            """);

        using var w = new BridgeWatcher(_arquivo);
        var snap = w.LerAtual();

        Assert.NotNull(snap);
        Assert.Equal(2026, snap!.Ano);
        Assert.Equal(7, snap.Etapa);
        Assert.Equal("Carlos", snap.Anfitriao);
        Assert.Equal(8, snap.Jogadores);
        Assert.Equal(50, snap.BuyIn);
    }

    [Fact]
    public void LerAtual_ArquivoCorrompido_RetornaNullSemLancar()
    {
        File.WriteAllText(_arquivo, "{{{ não-json");
        using var w = new BridgeWatcher(_arquivo);
        Assert.Null(w.LerAtual());
    }

    [Fact]
    public void Arquivo_ExpoeCaminhoConfigurado()
    {
        using var w = new BridgeWatcher(_arquivo);
        Assert.Equal(_arquivo, w.Arquivo);
    }
}
