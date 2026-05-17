using TimePoker.Domain;

namespace TimePoker.Services.Persistence;

/// <summary>
/// Snapshot serializável do estado do torneio para recovery após queda.
/// Inclui timestamp e versão pra detectar incompatibilidades futuras.
/// </summary>
public sealed class SessionState
{
    public int Versao { get; set; } = 1;
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public string NomeTorneio { get; set; } = "Torneio";
    public Estrutura Estrutura { get; set; } = new();
    public int IndiceNivelAtual { get; set; }
    public long TempoRestanteTicks { get; set; }
    public long TempoTotalDecorridoTicks { get; set; }
    public EstadoTorneio Estado { get; set; } = EstadoTorneio.Aguardando;
    public int Jogadores { get; set; }
    public int Rebuys { get; set; }

    public TimeSpan TempoRestante
    {
        get => TimeSpan.FromTicks(TempoRestanteTicks);
        set => TempoRestanteTicks = value.Ticks;
    }

    public TimeSpan TempoTotalDecorrido
    {
        get => TimeSpan.FromTicks(TempoTotalDecorridoTicks);
        set => TempoTotalDecorridoTicks = value.Ticks;
    }
}
