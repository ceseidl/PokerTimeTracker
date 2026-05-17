using System.IO;
using System.Windows;
using TimePoker.Domain;
using TimePoker.Services.Persistence;
using TimePoker.Wpf.Forms;
using TimePoker.Wpf.ViewModels;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace TimePoker.Wpf;

/// <summary>
/// Boot da aplicação:
///   1. Cria as 2 janelas principais (Control + Display) com ViewModel compartilhado
///   2. Mostra splash em cima (não-modal, auto-fecha em 7s)
///   3. ShutdownMode = OnMainWindowClose — só sai quando a janela de controle fecha
/// </summary>
public partial class App : System.Windows.Application
{
    public static MainViewModel? Vm { get; private set; }

    private static readonly string CrashLog =
        Path.Combine(Path.GetTempPath(), "TimePoker_crash.log");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Captura QUALQUER exceção não tratada e grava em log antes de a app sumir
        DispatcherUnhandledException += (_, ev) =>
        {
            File.AppendAllText(CrashLog,
                $"[{DateTime.Now:HH:mm:ss}] DispatcherUnhandledException:\n{ev.Exception}\n\n");
            ev.Handled = true;   // não derruba a app
            MessageBox.Show("Erro:\n" + ev.Exception.Message, "TimePoker — Erro");
        };
        AppDomain.CurrentDomain.UnhandledException += (_, ev) =>
        {
            File.AppendAllText(CrashLog,
                $"[{DateTime.Now:HH:mm:ss}] UnhandledException:\n{ev.ExceptionObject}\n\n");
        };

        // App só fecha por chamada explícita ao Shutdown() — controlado abaixo.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // 1) Splash sozinho na tela. Quando fechar (após 7s), abrimos as 2 janelas principais.
        var splash = new SplashWindow();
        splash.Closed += (_, _) => AbrirJanelasPrincipais();
        splash.Show();
    }

    private void AbrirJanelasPrincipais()
    {
        SemearEstruturasPadrao();

        var sessao = new SessionStore();
        var estadoSalvo = sessao.Carregar();

        // Recovery: se havia rodada Rodando/Pausada, retoma automaticamente
        // (descontando o tempo offline se estava Rodando). Se só Aguardando,
        // ignora — não vale a pena pedir restauração de torneio nunca iniciado.
        SessionState? aRestaurar = null;
        if (estadoSalvo != null
            && estadoSalvo.Estrutura.Niveis.Count > 0
            && estadoSalvo.Estado != EstadoTorneio.Aguardando
            && estadoSalvo.Estado != EstadoTorneio.Encerrado)
        {
            if (estadoSalvo.Estado == EstadoTorneio.Rodando)
            {
                // Desconta o tempo offline do restante e soma ao decorrido total
                // — o cronômetro "continuou virtualmente" enquanto fechado.
                // Se passou do tempo do nível, clipa em zero (usuário avança manualmente).
                var offline = DateTime.UtcNow - estadoSalvo.UltimaAtualizacao;
                if (offline < TimeSpan.Zero) offline = TimeSpan.Zero;
                var consumido = offline > estadoSalvo.TempoRestante
                    ? estadoSalvo.TempoRestante
                    : offline;
                estadoSalvo.TempoRestante -= consumido;
                estadoSalvo.TempoTotalDecorrido += consumido;
            }
            aRestaurar = estadoSalvo;
        }
        else if (estadoSalvo != null)
        {
            // Sessão sem partida em andamento — descarta pra começar limpo.
            sessao.Limpar();
        }

        // Default da Liga: estrutura "Inimigos do Royal Flush" (6 níveis 20min + 2 breaks).
        // Quando há sessão salva, ela já vem com a estrutura usada na noite anterior.
        Vm = new MainViewModel(PreSets.InimigosRoyalFlush(), aRestaurar, sessao);

        var control = new ControlWindow { DataContext = Vm };
        MainWindow = control;
        control.Closed += (_, _) =>
        {
            // Salva snapshot final pra retomada no próximo boot — o estado
            // em memória do VM é a fonte da verdade, não o auto-save velho.
            // Se estava Aguardando/Encerrado, limpa para boot limpo.
            try
            {
                if (Vm.Estado == EstadoTorneio.Rodando || Vm.Estado == EstadoTorneio.Pausado)
                    Vm.ForcarSalvarSessao();
                else
                    sessao.Limpar();
            }
            catch { /* best-effort */ }
            Shutdown();
        };
        control.Show();

        var display = new DisplayWindow { DataContext = Vm };
        display.Owner = control;
        display.Show();
    }

    /// <summary>
    /// Garante que as estruturas pré-prontas oficiais da Liga existam em
    /// <c>%APPDATA%\TimePoker\structures\</c>. Idempotente: só grava se ainda
    /// não houver arquivo com aquele nome — não sobrescreve customizações.
    /// </summary>
    private static void SemearEstruturasPadrao()
    {
        var store = new EstruturaStore();
        var existentes = store.Listar();
        const string nomeSeed = "Inimigos do Royal Flush";
        if (!existentes.Contains(nomeSeed, StringComparer.OrdinalIgnoreCase))
        {
            try { store.Salvar(nomeSeed, PreSets.InimigosRoyalFlush()); }
            catch { /* best-effort */ }
        }
    }

    private static string Formatar(TimeSpan idade)
    {
        if (idade.TotalSeconds < 60) return $"{(int)idade.TotalSeconds}s";
        if (idade.TotalMinutes < 60) return $"{(int)idade.TotalMinutes} min";
        if (idade.TotalHours < 24)   return $"{(int)idade.TotalHours}h{idade.Minutes:D2}";
        return $"{(int)idade.TotalDays}d";
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Vm?.Dispose();
        base.OnExit(e);
    }
}
