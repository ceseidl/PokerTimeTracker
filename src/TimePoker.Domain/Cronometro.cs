namespace TimePoker.Domain;

/// <summary>
/// Lógica de cronômetro pura — sem timers do .NET, sem UI.
/// Recebe ticks de tempo (ou um <see cref="Avancar"/> manual) e expõe o tempo restante
/// e o estado. Avança automaticamente para o próximo nível ao zerar.
///
/// Quem orquestra os ticks (DispatcherTimer, Stopwatch, etc.) fica em outra camada.
/// </summary>
public sealed class Cronometro
{
    private readonly Torneio _torneio;
    private TimeSpan _restante;
    private TimeSpan _decorrido;
    private EstadoTorneio _estado = EstadoTorneio.Aguardando;

    public Cronometro(Torneio torneio)
    {
        _torneio = torneio ?? throw new ArgumentNullException(nameof(torneio));
        ResetarTempoDoNivel();
    }

    public EstadoTorneio Estado => _estado;
    public TimeSpan Restante => _restante < TimeSpan.Zero ? TimeSpan.Zero : _restante;
    /// <summary>
    /// Tempo total que o cronômetro ficou rodando (soma dos Ticks no estado Rodando).
    /// Pauses não contam. Usado para reportar a duração real da rodada ao gerenciador.
    /// </summary>
    public TimeSpan Decorrido => _decorrido;
    public Torneio Torneio => _torneio;

    /// <summary>Disparado quando o tempo do nível chega a zero (depois de avançar o nível).</summary>
    public event Action? FimDeNivel;

    /// <summary>Inicia ou retoma a contagem.</summary>
    public void Iniciar()
    {
        if (_torneio.NivelAtual() is null) return;
        _estado = EstadoTorneio.Rodando;
    }

    /// <summary>Pausa a contagem (preserva o tempo restante).</summary>
    public void Pausar()
    {
        if (_estado == EstadoTorneio.Rodando) _estado = EstadoTorneio.Pausado;
    }

    /// <summary>Retoma após pause.</summary>
    public void Retomar()
    {
        if (_estado == EstadoTorneio.Pausado) _estado = EstadoTorneio.Rodando;
    }

    /// <summary>
    /// Consome um tick de tempo (chamado pela camada de UI/host a cada N ms).
    /// Se o tempo zerar, dispara <see cref="FimDeNivel"/> e avança automaticamente
    /// pro próximo nível, recarregando o tempo. Sem efeito se pausado/aguardando/encerrado.
    /// </summary>
    public void Tick(TimeSpan delta)
    {
        if (_estado != EstadoTorneio.Rodando) return;

        _decorrido += delta;
        _restante -= delta;
        if (_restante > TimeSpan.Zero) return;

        FimDeNivel?.Invoke();
        if (_torneio.AvancarNivel())
        {
            ResetarTempoDoNivel();
        }
        else
        {
            _restante = TimeSpan.Zero;
            _estado = EstadoTorneio.Encerrado;
        }
    }

    /// <summary>Avança manualmente pro próximo nível (botão "Próximo").</summary>
    public bool ProximoNivel()
    {
        if (!_torneio.AvancarNivel()) return false;
        ResetarTempoDoNivel();
        return true;
    }

    /// <summary>Volta manualmente pro nível anterior (botão "Anterior").</summary>
    public bool NivelAnterior()
    {
        if (!_torneio.VoltarNivel()) return false;
        ResetarTempoDoNivel();
        if (_estado == EstadoTorneio.Encerrado) _estado = EstadoTorneio.Aguardando;
        return true;
    }

    /// <summary>Reseta o cronômetro inteiro (volta pro nível 0).</summary>
    public void Resetar()
    {
        _torneio.IrParaNivel(0);
        _estado = EstadoTorneio.Aguardando;
        _decorrido = TimeSpan.Zero;
        ResetarTempoDoNivel();
    }

    /// <summary>Adiciona/remove tempo manualmente do nível atual (correções de juiz).</summary>
    public void AjustarTempo(TimeSpan delta)
    {
        _restante += delta;
        if (_restante < TimeSpan.Zero) _restante = TimeSpan.Zero;
    }

    /// <summary>
    /// Restaura o cronômetro a um estado salvo (recovery após queda).
    /// Recupera o índice do nível, o tempo restante e o estado. Se o estado salvo
    /// era <see cref="EstadoTorneio.Rodando"/>, retorna como <see cref="EstadoTorneio.Pausado"/>
    /// por segurança — o anfitrião confirma antes de retomar.
    /// </summary>
    public void Restaurar(int indiceNivel, TimeSpan tempoRestante, EstadoTorneio estado)
    {
        if (_torneio.IrParaNivel(indiceNivel))
        {
            _restante = tempoRestante < TimeSpan.Zero ? TimeSpan.Zero : tempoRestante;
            _estado = estado == EstadoTorneio.Rodando ? EstadoTorneio.Pausado : estado;
        }
    }

    private void ResetarTempoDoNivel()
    {
        _restante = _torneio.NivelAtual()?.Duracao ?? TimeSpan.Zero;
    }
}
