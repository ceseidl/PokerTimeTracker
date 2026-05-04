using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace TimePoker.Wpf.Forms;

/// <summary>
/// Splash inicial — 7 segundos, barra de progresso animada, "Carregando: ..."
/// com nomes aleatórios de pacotes/módulos pra dar a impressão de inicialização.
/// </summary>
public partial class SplashWindow : Window
{
    private const int DuracaoTotalMs = 7000;
    private const int IntervaloTickMs = 180;

    private static readonly string[] ItensCarregando =
    {
        "Microsoft.NETCore.App",
        "Microsoft.WindowsDesktop.App",
        "PresentationCore",
        "PresentationFramework",
        "WindowsBase",
        "System.Windows.Media",
        "System.Windows.Threading",
        "System.Threading.Tasks",
        "CommunityToolkit.Mvvm",
        "TimePoker.Domain.Cronometro",
        "TimePoker.Domain.Estrutura",
        "TimePoker.Domain.PreSets",
        "TimePoker.Wpf.Theme",
        "System.Globalization.CultureInfo",
        "System.Text.Json",
    };

    private readonly DispatcherTimer _ticker;
    private readonly DispatcherTimer _fechar;
    private readonly Random _rnd = new();
    private int _tick;
    private readonly int _totalTicks = DuracaoTotalMs / IntervaloTickMs;

    public SplashWindow()
    {
        InitializeComponent();
        LblVersao.Text = $"Versão {ObterVersao()}  ·  © Seidl Software Ltda";
        LblCarregando.Text = "Carregando: ...";

        _ticker = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(IntervaloTickMs) };
        _ticker.Tick += (_, _) => Avancar();

        _fechar = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DuracaoTotalMs) };
        _fechar.Tick += (_, _) => { _fechar.Stop(); Close(); };

        Loaded += (_, _) => { _ticker.Start(); _fechar.Start(); };
        Closed += (_, _) => { _ticker.Stop(); _fechar.Stop(); };
    }

    private void Avancar()
    {
        _tick++;
        Barra.Value = Math.Min(100, _tick * 100.0 / _totalTicks);
        LblCarregando.Text = $"Carregando: {ItensCarregando[_rnd.Next(ItensCarregando.Length)]}";
        if (_tick >= _totalTicks) _ticker.Stop();
    }

    private static string ObterVersao()
    {
        var v = typeof(SplashWindow).Assembly.GetName().Version;
        return v?.ToString(3) ?? "1.0.0";
    }
}
