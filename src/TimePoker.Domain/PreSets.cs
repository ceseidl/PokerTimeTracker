namespace TimePoker.Domain;

/// <summary>
/// Estruturas pré-prontas pra escolha rápida no editor.
/// Todas seguem padrão sem ante (mais comum em mesa caseira) e crescimento ~1.5–2x por nível.
/// </summary>
public static class PreSets
{
    /// <summary>Turbo (~2h): níveis de 10 min, blinds sobem rápido.</summary>
    public static Estrutura Turbo() => new()
    {
        Nome = "Turbo (≈2h)",
        BuyIn = 70, ValorRebuy = 50,
        Niveis =
        {
            Nivel.DeJogo(   25,   50, 10),
            Nivel.DeJogo(   50,  100, 10),
            Nivel.DeJogo(   75,  150, 10),
            Nivel.DeJogo(  100,  200, 10),
            Nivel.DeBreak(5),
            Nivel.DeJogo(  150,  300, 10),
            Nivel.DeJogo(  200,  400, 10),
            Nivel.DeJogo(  300,  600, 10),
            Nivel.DeJogo(  500, 1000, 10),
            Nivel.DeJogo(  800, 1600, 10),
            Nivel.DeJogo( 1500, 3000, 10),
        }
    };

    /// <summary>Padrão (≈3h): níveis de 15–20 min, equilíbrio entre estratégia e duração.</summary>
    public static Estrutura Padrao() => new()
    {
        Nome = "Padrão (≈3h)",
        BuyIn = 70, ValorRebuy = 50,
        Niveis =
        {
            Nivel.DeJogo(   25,   50, 15),
            Nivel.DeJogo(   50,  100, 15),
            Nivel.DeJogo(   75,  150, 15),
            Nivel.DeJogo(  100,  200, 20),
            Nivel.DeBreak(10),
            Nivel.DeJogo(  150,  300, 20),
            Nivel.DeJogo(  200,  400, 20),
            Nivel.DeJogo(  300,  600, 20),
            Nivel.DeBreak(10, "BREAK FINAL"),
            Nivel.DeJogo(  500, 1000, 20),
            Nivel.DeJogo(  800, 1600, 20),
            Nivel.DeJogo( 1200, 2400, 20),
        }
    };

    /// <summary>Deep stack (≈4h): níveis de 30+ min, jogo mais técnico.</summary>
    public static Estrutura DeepStack() => new()
    {
        Nome = "Deep stack (≈4h+)",
        BuyIn = 70, ValorRebuy = 50,
        Niveis =
        {
            Nivel.DeJogo(   25,   50, 30),
            Nivel.DeJogo(   50,  100, 30),
            Nivel.DeJogo(   75,  150, 30),
            Nivel.DeJogo(  100,  200, 30),
            Nivel.DeBreak(15, "BREAK"),
            Nivel.DeJogo(  150,  300, 30),
            Nivel.DeJogo(  200,  400, 30),
            Nivel.DeJogo(  300,  600, 30),
            Nivel.DeBreak(15, "BREAK FINAL"),
            Nivel.DeJogo(  500, 1000, 30),
            Nivel.DeJogo(  800, 1600, 30),
        }
    };

    /// <summary>
    /// Estrutura padrão dos torneios da Liga Inimigos do Royal Flush:
    /// 6 níveis de 20 min com 2 breaks. Blinds começam em 50/100 e dobram a
    /// cada nível até 2000/3000.
    /// </summary>
    public static Estrutura InimigosRoyalFlush() => new()
    {
        Nome = "Inimigos do Royal Flush",
        BuyIn = 50, ValorRebuy = 50,
        Niveis =
        {
            Nivel.DeJogo(   50,  100, 20),
            Nivel.DeJogo(  100,  200, 20),
            Nivel.DeJogo(  200,  400, 20),
            Nivel.DeBreak(10),
            Nivel.DeJogo(  500, 1000, 20),
            Nivel.DeJogo( 1000, 2000, 20),
            Nivel.DeJogo( 2000, 3000, 20),
            Nivel.DeBreak(10, "BREAK FINAL"),
        }
    };

    /// <summary>Estrutura vazia para edição manual do zero.</summary>
    public static Estrutura Customizada() => new()
    {
        Nome = "Customizada",
        BuyIn = 70, ValorRebuy = 50,
        Niveis = new List<Nivel>()
    };

    /// <summary>Lista todos os pré-sets disponíveis (útil pra montar combo na UI).</summary>
    public static IReadOnlyList<Func<Estrutura>> Todos() =>
        new Func<Estrutura>[] { Turbo, Padrao, DeepStack, InimigosRoyalFlush, Customizada };
}
