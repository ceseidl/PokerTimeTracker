namespace TimePoker.Domain;

/// <summary>Tipo de um nível dentro da estrutura do torneio.</summary>
public enum TipoNivel
{
    /// <summary>Nível normal de jogo (com small/big blind ativos).</summary>
    Jogo = 0,
    /// <summary>Intervalo (break) — sem blinds, jogadores descansam.</summary>
    Break = 1
}
