using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimePoker.Domain;
using TimePoker.Services.Persistence;
using TimePoker.Wpf.Services;

namespace TimePoker.Wpf.ViewModels;

/// <summary>
/// State compartilhado entre ControlWindow e DisplayWindow.
/// Encapsula o domain (<see cref="Torneio"/> + <see cref="Cronometro"/>) e expõe
/// propriedades observáveis + comandos pra a UI.
///
/// Tick orquestrado por <see cref="DispatcherTimer"/> (1s) — quando rodando,
/// chama <c>_cronometro.Tick(1s)</c>.
/// </summary>
public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherTimer _ticker;
    private readonly DispatcherTimer _autoSaveTimer;
    private readonly Torneio _torneio;
    private readonly Cronometro _cronometro;
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);
    /// <summary>Intervalo do auto-save periódico (default: 1 min).</summary>
    public static TimeSpan AutoSaveInterval { get; set; } = TimeSpan.FromMinutes(1);

    [ObservableProperty] private string _nomeTorneio = "Saturday Night Poker";

    // Reflexo das propriedades do domain (atualizadas a cada tick)
    [ObservableProperty] private string _tempoRestanteFormatado = "00:00";
    [ObservableProperty] private string _blindsAtual = "—";
    [ObservableProperty] private string _blindsAnterior = "—";
    [ObservableProperty] private string _blindsProximo = "—";
    [ObservableProperty] private int _nivelAtualNumero = 1;
    [ObservableProperty] private bool _proximoEhBreak;
    [ObservableProperty] private bool _atualEhBreak;
    [ObservableProperty] private int _jogadores;
    [ObservableProperty] private int _rebuys;
    [ObservableProperty] private int _prizePool;
    [ObservableProperty] private EstadoTorneio _estado = EstadoTorneio.Aguardando;
    [ObservableProperty] private string _tempoAteProximoBreakFormatado = "—";

    /// <summary>Lista observável de todos os níveis da estrutura (com status visual).</summary>
    public ObservableCollection<NivelLinhaViewModel> Niveis { get; } = new();

    private readonly SessionStore _sessao;
    private readonly AlarmeService _alarme;

    /// <summary>Liga/desliga o alarme de fim de nível.</summary>
    [ObservableProperty] private bool _alarmeHabilitado = true;

    partial void OnAlarmeHabilitadoChanged(bool value) => _alarme.Habilitado = value;

    public MainViewModel() : this(PreSets.Padrao(), null, new SessionStore()) { }

    public MainViewModel(SessionStore sessao) : this(PreSets.Padrao(), null, sessao) { }

    public MainViewModel(Estrutura estrutura) : this(estrutura, null, new SessionStore()) { }

    /// <summary>Construtor principal — aceita estrutura + estado salvo (recovery) + store.</summary>
    public MainViewModel(Estrutura estrutura, SessionState? restaurar, SessionStore sessao)
    {
        _sessao = sessao;
        _alarme = new AlarmeService();

        // Se for restaurar, usa a estrutura salva (que tem os níveis customizados)
        var estruturaAtiva = restaurar?.Estrutura ?? estrutura;
        _torneio = new Torneio { Nome = "Torneio", Estrutura = estruturaAtiva };
        _cronometro = new Cronometro(_torneio);
        _cronometro.FimDeNivel += AoFimDeNivel;

        // Tick do cronômetro a cada 1s (atualiza UI), SEM salvar.
        _ticker = new DispatcherTimer { Interval = TickInterval };
        _ticker.Tick += (_, _) =>
        {
            _cronometro.Tick(TickInterval);
            AtualizarPropriedades();
        };

        // Auto-save periódico a cada AutoSaveInterval (default 1 min) — só roda enquanto o ticker está ativo.
        _autoSaveTimer = new DispatcherTimer { Interval = AutoSaveInterval };
        _autoSaveTimer.Tick += (_, _) => SalvarSessao();

        if (restaurar != null)
        {
            NomeTorneio = restaurar.NomeTorneio;
            _torneio.Jogadores = restaurar.Jogadores;
            _torneio.Rebuys = restaurar.Rebuys;
            _cronometro.Restaurar(restaurar.IndiceNivelAtual,
                                   restaurar.TempoRestante,
                                   restaurar.Estado);
        }

        ReconstruirListaNiveis();
        AtualizarPropriedades();
    }

    public Torneio Torneio => _torneio;
    public Cronometro Cronometro => _cronometro;

    /// <summary>Disparado quando um nível termina — UI pode tocar alarme.</summary>
    public event Action? NivelTerminou;

    [RelayCommand]
    private void IniciarOuPausar()
    {
        if (_cronometro.Estado == EstadoTorneio.Rodando)
        {
            _cronometro.Pausar();
            _ticker.Stop();
            _autoSaveTimer.Stop();
        }
        else if (_cronometro.Estado == EstadoTorneio.Pausado)
        {
            _cronometro.Retomar();
            _ticker.Start();
            _autoSaveTimer.Start();
        }
        else if (_cronometro.Estado == EstadoTorneio.Aguardando)
        {
            _cronometro.Iniciar();
            _ticker.Start();
            _autoSaveTimer.Start();
        }
        AposAcao();
    }

    [RelayCommand]
    private void Proximo()
    {
        _cronometro.ProximoNivel();
        AposAcao();
    }

    [RelayCommand]
    private void Anterior()
    {
        _cronometro.NivelAnterior();
        AposAcao();
    }

    [RelayCommand]
    private void Resetar()
    {
        _ticker.Stop();
        _autoSaveTimer.Stop();
        _cronometro.Resetar();
        _torneio.Jogadores = 0;
        _torneio.Rebuys = 0;
        AtualizarPropriedades();
        // Reset = começo limpo: apaga sessão pra não restaurar mais
        _sessao.Limpar();
    }

    [RelayCommand] private void TestarAlarme() => _ = _alarme.TocarAsync();

    [RelayCommand]
    private void ImportarPokerInimigos()
    {
        var dlg = new Forms.ImportarJogadoresDialog
        {
            Owner = System.Windows.Application.Current?.MainWindow
        };
        if (dlg.ShowDialog() != true) return;

        _torneio.Jogadores = dlg.QuantidadeMarcada;
        AtualizarPropriedades();
        SalvarSessao();
    }

    [RelayCommand]
    private void SalvarEstruturaComo()
    {
        var dlg = new Forms.SalvarComoDialog(_torneio.Estrutura.Nome)
        {
            Owner = System.Windows.Application.Current?.MainWindow
        };
        if (dlg.ShowDialog() != true) return;
        try
        {
            new EstruturaStore().Salvar(dlg.NomeEscolhido, _torneio.Estrutura);
            System.Windows.MessageBox.Show(System.Windows.Application.Current?.MainWindow!,
                $"Estrutura \"{dlg.NomeEscolhido}\" salva.",
                "Salvo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(System.Windows.Application.Current?.MainWindow!,
                "Falha ao salvar:\n" + ex.Message,
                "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void CarregarEstruturaSalva()
    {
        var store = new EstruturaStore();
        if (store.Listar().Count == 0)
        {
            System.Windows.MessageBox.Show(System.Windows.Application.Current?.MainWindow!,
                "Nenhuma estrutura salva ainda.\nUse \"Salvar como...\" pra criar a primeira.",
                "Sem estruturas", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return;
        }

        var dlg = new Forms.CarregarEstruturaDialog(store)
        {
            Owner = System.Windows.Application.Current?.MainWindow
        };
        if (dlg.ShowDialog() != true || dlg.NomeEscolhido is null) return;

        var carregada = store.Carregar(dlg.NomeEscolhido);
        if (carregada == null || carregada.Niveis.Count == 0)
        {
            System.Windows.MessageBox.Show(System.Windows.Application.Current?.MainWindow!,
                "Estrutura não encontrada ou vazia.",
                "Erro", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        CarregarEstrutura(carregada);
        SalvarSessao();
    }

    [RelayCommand] private void CarregarTurbo()      => SubstituirEstrutura(PreSets.Turbo());
    [RelayCommand] private void CarregarPadrao()     => SubstituirEstrutura(PreSets.Padrao());
    [RelayCommand] private void CarregarDeepStack()  => SubstituirEstrutura(PreSets.DeepStack());
    [RelayCommand] private void CarregarCustomizada() => SubstituirEstrutura(PreSets.Customizada());

    private void SubstituirEstrutura(Estrutura nova)
    {
        var resp = System.Windows.MessageBox.Show(
            $"Carregar a estrutura \"{nova.Nome}\"?\n\n" +
            "Isso vai descartar a estrutura atual e zerar o cronômetro.",
            "Confirmar troca de estrutura",
            System.Windows.MessageBoxButton.OKCancel,
            System.Windows.MessageBoxImage.Question);
        if (resp != System.Windows.MessageBoxResult.OK) return;
        CarregarEstrutura(nova);
        SalvarSessao();
    }

    [RelayCommand] private void AdicionarJogador()    { _torneio.Jogadores++; AposAcao(); }
    [RelayCommand] private void RemoverJogador()      { if (_torneio.Jogadores > 0) _torneio.Jogadores--; AposAcao(); }
    [RelayCommand] private void AdicionarRebuy()      { _torneio.Rebuys++; AposAcao(); }
    [RelayCommand] private void RemoverRebuy()        { if (_torneio.Rebuys > 0) _torneio.Rebuys--; AposAcao(); }
    [RelayCommand] private void AdicionarUmMinuto()   { _cronometro.AjustarTempo(TimeSpan.FromMinutes(1)); AposAcao(); }
    [RelayCommand] private void RemoverUmMinuto()     { _cronometro.AjustarTempo(TimeSpan.FromMinutes(-1)); AposAcao(); }

    private void AposAcao()
    {
        AtualizarPropriedades();
        SalvarSessao();
    }

    private void SalvarSessao()
    {
        try
        {
            _sessao.Salvar(new SessionState
            {
                NomeTorneio = NomeTorneio,
                Estrutura = _torneio.Estrutura,
                IndiceNivelAtual = _torneio.IndiceNivelAtual,
                TempoRestante = _cronometro.Restante,
                Estado = _cronometro.Estado,
                Jogadores = _torneio.Jogadores,
                Rebuys = _torneio.Rebuys
            });
        }
        catch
        {
            // Falhar em salvar não derruba a noite — toleramos silenciosamente.
        }
    }

    /// <summary>Trocar a estrutura inteira (ex.: ao escolher um pré-set).</summary>
    public void CarregarEstrutura(Estrutura nova)
    {
        _ticker.Stop();
        _torneio.Estrutura = nova;
        _torneio.Resetar();
        _cronometro.Resetar();
        ReconstruirListaNiveis();
        AtualizarPropriedades();
    }

    private bool _refletindo;   // evita reentrada quando AoMudar dispara durante reconstrução

    private void ReconstruirListaNiveis()
    {
        _refletindo = true;
        try
        {
            foreach (var l in Niveis) l.AoMudar = null;
            Niveis.Clear();
            var lista = _torneio.Estrutura.Niveis;
            for (int i = 0; i < lista.Count; i++)
            {
                var status = i < _torneio.IndiceNivelAtual ? StatusLinha.Passou
                           : i == _torneio.IndiceNivelAtual ? StatusLinha.Atual
                           : StatusLinha.Futuro;
                var linha = NivelLinhaViewModel.De(i + 1, lista[i], status);
                linha.AoMudar = RefletirNoDomain;
                Niveis.Add(linha);
            }
        }
        finally { _refletindo = false; }
    }

    /// <summary>Sincroniza alterações inline da grid de volta pro <see cref="Estrutura.Niveis"/>.</summary>
    private void RefletirNoDomain()
    {
        if (_refletindo) return;
        _refletindo = true;
        try
        {
            _torneio.Estrutura.Niveis.Clear();
            foreach (var l in Niveis) _torneio.Estrutura.Niveis.Add(l.ParaNivel());
            AtualizarPropriedades();
            SalvarSessao();
        }
        finally { _refletindo = false; }
    }

    [RelayCommand]
    private void AdicionarNivel()
    {
        var ult = _torneio.Estrutura.Niveis.LastOrDefault(n => !n.EhBreak());
        var sb = ult != null ? ult.BigBlind : 25;
        var bb = ult != null ? ult.BigBlind * 2 : 50;
        var dur = ult != null ? (int)ult.Duracao.TotalMinutes : 15;
        _torneio.Estrutura.Niveis.Add(Nivel.DeJogo(sb, bb, dur));
        ReconstruirListaNiveis();
        AposAcao();
    }

    [RelayCommand]
    private void AdicionarBreak()
    {
        _torneio.Estrutura.Niveis.Add(Nivel.DeBreak(10));
        ReconstruirListaNiveis();
        AposAcao();
    }

    [RelayCommand]
    private void RemoverNivel(NivelLinhaViewModel? linha)
    {
        if (linha == null) return;
        var idx = linha.Numero - 1;
        if (idx < 0 || idx >= _torneio.Estrutura.Niveis.Count) return;
        _torneio.Estrutura.Niveis.RemoveAt(idx);
        if (_torneio.IndiceNivelAtual >= _torneio.Estrutura.Niveis.Count)
            _torneio.IrParaNivel(Math.Max(0, _torneio.Estrutura.Niveis.Count - 1));
        ReconstruirListaNiveis();
        AposAcao();
    }

    [RelayCommand]
    private void SubirNivel(NivelLinhaViewModel? linha)
    {
        if (linha == null) return;
        var idx = linha.Numero - 1;
        if (idx <= 0 || idx >= _torneio.Estrutura.Niveis.Count) return;
        var lista = _torneio.Estrutura.Niveis;
        (lista[idx], lista[idx - 1]) = (lista[idx - 1], lista[idx]);
        ReconstruirListaNiveis();
        AposAcao();
    }

    [RelayCommand]
    private void DescerNivel(NivelLinhaViewModel? linha)
    {
        if (linha == null) return;
        var idx = linha.Numero - 1;
        if (idx < 0 || idx >= _torneio.Estrutura.Niveis.Count - 1) return;
        var lista = _torneio.Estrutura.Niveis;
        (lista[idx], lista[idx + 1]) = (lista[idx + 1], lista[idx]);
        ReconstruirListaNiveis();
        AposAcao();
    }

    private void AtualizarStatusListaNiveis()
    {
        for (int i = 0; i < Niveis.Count; i++)
        {
            Niveis[i].Status = i < _torneio.IndiceNivelAtual ? StatusLinha.Passou
                             : i == _torneio.IndiceNivelAtual ? StatusLinha.Atual
                             : StatusLinha.Futuro;
        }
    }

    private void AoFimDeNivel()
    {
        // Toca o alarme (em background, não bloqueia)
        _ = _alarme.TocarAsync();
        // Marshal para a UI thread (evento pode vir de outro contexto futuramente)
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(() => NivelTerminou?.Invoke());
    }

    private void AtualizarPropriedades()
    {
        var atual = _torneio.NivelAtual();
        var anterior = _torneio.NivelAnterior();
        var proximo = _torneio.NivelProximo();

        TempoRestanteFormatado = $"{(int)_cronometro.Restante.TotalMinutes:D2}:{_cronometro.Restante.Seconds:D2}";
        BlindsAtual    = atual?.FormatarBlinds()    ?? "—";
        BlindsAnterior = anterior?.FormatarBlinds() ?? "—";
        BlindsProximo  = proximo?.FormatarBlinds()  ?? "—";
        AtualEhBreak   = atual?.EhBreak() == true;
        ProximoEhBreak = proximo?.EhBreak() == true;
        NivelAtualNumero = _torneio.IndiceNivelAtual + 1;
        Jogadores      = _torneio.Jogadores;
        Rebuys         = _torneio.Rebuys;
        PrizePool      = _torneio.CalcularPrizePool();
        Estado         = _cronometro.Estado;
        TempoAteProximoBreakFormatado = CalcularTempoAteProximoBreak();
        AtualizarStatusListaNiveis();
    }

    private string CalcularTempoAteProximoBreak()
    {
        var idx = _torneio.IndiceNivelAtual;
        var niveis = _torneio.Estrutura.Niveis;
        var acumulado = _cronometro.Restante;
        for (int i = idx + 1; i < niveis.Count; i++)
        {
            if (niveis[i].EhBreak())
            {
                var total = (int)acumulado.TotalMinutes;
                return total > 0 ? $"{total} min" : "<1 min";
            }
            acumulado += niveis[i].Duracao;
        }
        return "—";
    }

    public void Dispose()
    {
        _ticker.Stop();
        _autoSaveTimer.Stop();
        _cronometro.FimDeNivel -= AoFimDeNivel;
    }
}
