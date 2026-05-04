namespace TimePoker.Services.Persistence;

/// <summary>
/// Localiza e lê o arquivo <c>participantes.md</c> usado pelo painel
/// <c>PokerInimigos</c> (lista oficial dos membros do clube).
///
/// Estratégia de descoberta:
///   1. Sobe a árvore de diretórios a partir de <see cref="AppContext.BaseDirectory"/>
///      até achar um <c>participantes.md</c>.
///   2. Se não achar, retorna null — caller mostra um diálogo "Selecionar arquivo".
/// </summary>
public sealed class ParticipantesImporter
{
    public const string NomeArquivo = "participantes.md";

    /// <summary>Procura o arquivo em diretórios pais. Retorna null se não achar.</summary>
    public string? LocalizarArquivo()
    {
        var atual = new DirectoryInfo(AppContext.BaseDirectory);
        while (atual != null)
        {
            var caminho = Path.Combine(atual.FullName, NomeArquivo);
            if (File.Exists(caminho)) return caminho;
            atual = atual.Parent;
        }
        return null;
    }

    /// <summary>Lê o arquivo e devolve a lista de nomes (uma linha = um nome).</summary>
    public IReadOnlyList<string> Ler(string caminho)
    {
        return File.ReadAllLines(caminho)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("#"))
            .ToList();
    }
}
