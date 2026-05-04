namespace TimePoker.Domain;

/// <summary>
/// Um nível da estrutura do torneio.
/// Imutável — alterações são feitas reconstruindo a <see cref="Estrutura"/>.
/// </summary>
public sealed class Nivel
{
    public TipoNivel Tipo { get; init; } = TipoNivel.Jogo;
    public int SmallBlind { get; init; }
    public int BigBlind { get; init; }
    public int Ante { get; init; }
    public TimeSpan Duracao { get; init; } = TimeSpan.FromMinutes(20);
    /// <summary>Rótulo opcional (ex.: "Break do dinheiro").</summary>
    public string Rotulo { get; init; } = string.Empty;

    /// <summary>True se for um intervalo (sem blinds ativos).</summary>
    public bool EhBreak() => Tipo == TipoNivel.Break;

    /// <summary>Texto curto pra exibição (ex.: "200 / 400" ou "200 / 400 (25)" com ante).</summary>
    public string FormatarBlinds() =>
        EhBreak()
            ? string.IsNullOrEmpty(Rotulo) ? "BREAK" : Rotulo
            : Ante > 0 ? $"{SmallBlind} / {BigBlind} ({Ante})" : $"{SmallBlind} / {BigBlind}";

    public static Nivel DeJogo(int smallBlind, int bigBlind, int duracaoMinutos, int ante = 0) =>
        new()
        {
            Tipo = TipoNivel.Jogo,
            SmallBlind = smallBlind,
            BigBlind = bigBlind,
            Ante = ante,
            Duracao = TimeSpan.FromMinutes(duracaoMinutos)
        };

    public static Nivel DeBreak(int duracaoMinutos, string rotulo = "BREAK") =>
        new()
        {
            Tipo = TipoNivel.Break,
            Duracao = TimeSpan.FromMinutes(duracaoMinutos),
            Rotulo = rotulo
        };
}
