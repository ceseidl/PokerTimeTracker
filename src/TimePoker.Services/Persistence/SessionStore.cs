using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimePoker.Services.Persistence;

/// <summary>
/// Persistência leve do estado de sessão pra recovery após queda do app.
/// Vive em <c>%APPDATA%\TimePoker\session.json</c>.
///
/// Estratégia: o ViewModel chama <see cref="Salvar"/> a cada mudança relevante
/// (e a cada tick do cronômetro). No boot, <see cref="Carregar"/> retorna o
/// último snapshot — o usuário decide se restaura.
/// </summary>
public sealed class SessionStore
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _arquivo;

    public SessionStore(string? caminhoCustom = null)
    {
        _arquivo = caminhoCustom ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TimePoker",
            "session.json");
    }

    public string Caminho => _arquivo;

    public bool Existe() => File.Exists(_arquivo);

    public void Salvar(SessionState estado)
    {
        ArgumentNullException.ThrowIfNull(estado);
        estado.UltimaAtualizacao = DateTime.UtcNow;
        var dir = Path.GetDirectoryName(_arquivo);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

        // Escrita atômica: salva em arquivo temporário e move por cima
        var tmp = _arquivo + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(estado, JsonOpts));
        if (File.Exists(_arquivo)) File.Replace(tmp, _arquivo, null);
        else File.Move(tmp, _arquivo);
    }

    public SessionState? Carregar()
    {
        if (!File.Exists(_arquivo)) return null;
        try
        {
            var json = File.ReadAllText(_arquivo);
            return JsonSerializer.Deserialize<SessionState>(json, JsonOpts);
        }
        catch
        {
            // Arquivo corrompido — tratamos como "sem sessão"
            return null;
        }
    }

    public void Limpar()
    {
        if (File.Exists(_arquivo))
        {
            try { File.Delete(_arquivo); } catch { /* idempotente */ }
        }
    }
}
