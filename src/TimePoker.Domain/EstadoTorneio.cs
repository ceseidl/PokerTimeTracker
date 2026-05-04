namespace TimePoker.Domain;

/// <summary>Estado do cronômetro/torneio.</summary>
public enum EstadoTorneio
{
    /// <summary>Torneio configurado mas ainda não iniciado.</summary>
    Aguardando = 0,
    /// <summary>Cronômetro rodando.</summary>
    Rodando = 1,
    /// <summary>Cronômetro pausado pelo anfitrião.</summary>
    Pausado = 2,
    /// <summary>Todos os níveis terminaram.</summary>
    Encerrado = 3
}
