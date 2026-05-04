using System.Text.Json;
using System.Text.Json.Serialization;
using TimePoker.Domain;

namespace TimePoker.Services.Persistence;

/// <summary>
/// Persiste estruturas nomeadas em <c>%APPDATA%\TimePoker\structures\*.json</c>.
///
/// Cada estrutura é um único arquivo cujo nome do arquivo (sem extensão) é o
/// nome amigável da estrutura. Permite o anfitrião ter várias configurações
/// salvas (ex.: "Sábado Padrão", "Final de Ano Deep").
/// </summary>
public sealed class EstruturaStore
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _pasta;

    public EstruturaStore(string? pastaCustom = null)
    {
        _pasta = pastaCustom ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TimePoker",
            "structures");
    }

    public string Pasta => _pasta;

    public IReadOnlyList<string> Listar()
    {
        if (!Directory.Exists(_pasta)) return Array.Empty<string>();
        return Directory.EnumerateFiles(_pasta, "*.json")
            .Select(f => Path.GetFileNameWithoutExtension(f)!)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void Salvar(string nome, Estrutura estrutura)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentNullException.ThrowIfNull(estrutura);
        Directory.CreateDirectory(_pasta);
        estrutura.Nome = nome;
        var caminho = CaminhoDe(nome);
        var tmp = caminho + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(estrutura, JsonOpts));
        if (File.Exists(caminho)) File.Replace(tmp, caminho, null);
        else File.Move(tmp, caminho);
    }

    public Estrutura? Carregar(string nome)
    {
        var caminho = CaminhoDe(nome);
        if (!File.Exists(caminho)) return null;
        try
        {
            var json = File.ReadAllText(caminho);
            return JsonSerializer.Deserialize<Estrutura>(json, JsonOpts);
        }
        catch { return null; }
    }

    public bool Excluir(string nome)
    {
        var caminho = CaminhoDe(nome);
        if (!File.Exists(caminho)) return false;
        try { File.Delete(caminho); return true; }
        catch { return false; }
    }

    private string CaminhoDe(string nome)
    {
        var seguro = SanitizarNome(nome);
        return Path.Combine(_pasta, seguro + ".json");
    }

    /// <summary>Remove caracteres inválidos no nome de arquivo.</summary>
    private static string SanitizarNome(string nome)
    {
        var invalidos = Path.GetInvalidFileNameChars();
        var s = nome.Trim();
        foreach (var c in invalidos) s = s.Replace(c, '_');
        return string.IsNullOrEmpty(s) ? "estrutura" : s;
    }
}
