using System.Text.Json;

namespace TimePoker.Services.Integration;

/// <summary>
/// Snapshot recebido do gerenciador (PokerInimigos) via arquivo JSON.
///
/// Contrato compartilhado em <c>%APPDATA%\TimePoker\bridge.json</c>. Qualquer
/// mudança aqui exige atualizar também o writer no gerenciador.
/// </summary>
public sealed class BridgeSnapshot
{
    public int Versao { get; set; } = 1;
    public DateTime Atualizado { get; set; }
    public int Ano { get; set; }
    public int Etapa { get; set; }
    public string Anfitriao { get; set; } = string.Empty;
    public int Jogadores { get; set; }
    public int Rebuys { get; set; }
    public int BuyIn { get; set; }
    public int ValorRebuy { get; set; }
}

/// <summary>
/// Observa o arquivo bridge.json escrito pelo gerenciador e dispara
/// <see cref="SnapshotRecebido"/> sempre que ele muda.
///
/// O FileSystemWatcher do .NET é ruidoso (pode disparar 2× para um único save —
/// uma vez por Created, outra por Changed). Por isso o handler debounce e
/// re-lê o arquivo inteiro, em vez de tentar interpretar o evento.
/// </summary>
public sealed class BridgeWatcher : IDisposable
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _arquivo;
    private readonly FileSystemWatcher? _watcher;
    private DateTime _ultimoDisparo = DateTime.MinValue;

    public event Action<BridgeSnapshot>? SnapshotRecebido;

    public BridgeWatcher(string? caminhoCustom = null)
    {
        _arquivo = caminhoCustom ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TimePoker", "bridge.json");

        var pasta = Path.GetDirectoryName(_arquivo)!;
        Directory.CreateDirectory(pasta);

        _watcher = new FileSystemWatcher(pasta, Path.GetFileName(_arquivo))
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            EnableRaisingEvents = true
        };
        _watcher.Changed += AoMudar;
        _watcher.Created += AoMudar;
        _watcher.Renamed += AoMudar;
    }

    public string Arquivo => _arquivo;

    /// <summary>Lê o snapshot atual (se existir). Retorna null se ainda não foi escrito.</summary>
    public BridgeSnapshot? LerAtual()
    {
        try
        {
            if (!File.Exists(_arquivo)) return null;
            var json = File.ReadAllText(_arquivo);
            return JsonSerializer.Deserialize<BridgeSnapshot>(json, JsonOpts);
        }
        catch
        {
            // Arquivo pode estar sendo escrito; o próximo evento dispara nova leitura.
            return null;
        }
    }

    private void AoMudar(object sender, FileSystemEventArgs e)
    {
        // Debounce simples: ignora eventos a menos de 150ms do anterior.
        var agora = DateTime.UtcNow;
        if ((agora - _ultimoDisparo).TotalMilliseconds < 150) return;
        _ultimoDisparo = agora;

        // Pequena espera pra escrita atômica terminar (rename do .tmp pro arquivo final).
        Thread.Sleep(50);

        var snapshot = LerAtual();
        if (snapshot != null) SnapshotRecebido?.Invoke(snapshot);
    }

    public void Dispose()
    {
        if (_watcher == null) return;
        _watcher.EnableRaisingEvents = false;
        _watcher.Changed -= AoMudar;
        _watcher.Created -= AoMudar;
        _watcher.Renamed -= AoMudar;
        _watcher.Dispose();
    }
}
