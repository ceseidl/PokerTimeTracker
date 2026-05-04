namespace TimePoker.Domain;

/// <summary>
/// Estrutura completa de um torneio: lista ordenada de níveis + parâmetros econômicos.
/// É a unidade de persistência (serializável em JSON e nomeável).
/// </summary>
public sealed class Estrutura
{
    public string Nome { get; set; } = "Sem nome";
    public int BuyIn { get; set; } = 70;
    public int ValorRebuy { get; set; } = 50;
    public List<Nivel> Niveis { get; set; } = new();

    public int TotalNiveis => Niveis.Count;
    public bool EstaVazia() => Niveis.Count == 0;

    /// <summary>Duração total da estrutura (somando todos os níveis).</summary>
    public TimeSpan DuracaoTotal() =>
        Niveis.Aggregate(TimeSpan.Zero, (acc, n) => acc + n.Duracao);
}
