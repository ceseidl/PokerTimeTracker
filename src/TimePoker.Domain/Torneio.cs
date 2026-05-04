namespace TimePoker.Domain;

/// <summary>
/// Torneio "vivo" — envolve a <see cref="Estrutura"/> escolhida e o estado durante a noite
/// (qual nível está ativo, quantos jogadores, rebuys, prize pool).
/// </summary>
public sealed class Torneio
{
    public string Nome { get; set; } = "Torneio";
    public Estrutura Estrutura { get; set; } = PreSets.Padrao();
    public int IndiceNivelAtual { get; private set; }
    public int Jogadores { get; set; }
    public int Rebuys { get; set; }

    public Nivel? NivelAtual()      => Estrutura.Niveis.ElementAtOrDefault(IndiceNivelAtual);
    public Nivel? NivelAnterior()   => IndiceNivelAtual > 0
                                          ? Estrutura.Niveis.ElementAtOrDefault(IndiceNivelAtual - 1)
                                          : null;
    public Nivel? NivelProximo()    => Estrutura.Niveis.ElementAtOrDefault(IndiceNivelAtual + 1);

    public bool TemProximo() => IndiceNivelAtual < Estrutura.Niveis.Count - 1;
    public bool TemAnterior() => IndiceNivelAtual > 0;

    /// <summary>Avança 1 nível. Retorna true se houve avanço.</summary>
    public bool AvancarNivel()
    {
        if (!TemProximo()) return false;
        IndiceNivelAtual++;
        return true;
    }

    /// <summary>Volta 1 nível. Retorna true se houve retrocesso.</summary>
    public bool VoltarNivel()
    {
        if (!TemAnterior()) return false;
        IndiceNivelAtual--;
        return true;
    }

    /// <summary>Reseta o torneio (volta pro nível 0, zera contadores).</summary>
    public void Resetar()
    {
        IndiceNivelAtual = 0;
        Jogadores = 0;
        Rebuys = 0;
    }

    /// <summary>Pula direto pra um nível específico.</summary>
    public bool IrParaNivel(int indice)
    {
        if (indice < 0 || indice >= Estrutura.Niveis.Count) return false;
        IndiceNivelAtual = indice;
        return true;
    }

    /// <summary>
    /// Prize pool atual = (Jogadores × Buy-in) + (Rebuys × ValorRebuy).
    /// Não considera regras de premiação (top 1/2/3) — isso é responsabilidade
    /// de uma futura "calculadora de premiação" (fora do MVP).
    /// </summary>
    public int CalcularPrizePool() =>
        (Jogadores * Estrutura.BuyIn) + (Rebuys * Estrutura.ValorRebuy);
}
