using System.Linq;
using System.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Key = System.Windows.Input.Key;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;

namespace TimePoker.Wpf.Forms;

/// <summary>
/// Janela de controle (notebook). Atalhos:
///  - Espaço: iniciar/pausar
///  - ←/→: anterior/próximo
///  - Ctrl+Shift+C: traz o controle pra frente
/// </summary>
public partial class ControlWindow : Window
{
    public ControlWindow()
    {
        InitializeComponent();
        KeyDown += AoTeclar;
        Loaded += AjustarAoMonitor;
        TitleBarBehavior.Attach(this);
        StateChanged += (_, _) =>
            BtnMaxRestore.Content = WindowState == WindowState.Maximized ? "" : "";
    }

    private void AoMinimizar(object sender, RoutedEventArgs e) => TitleBarBehavior.OnMinimize(this);
    private void AoMaximizarRestaurar(object sender, RoutedEventArgs e) => TitleBarBehavior.OnMaxRestore(this);
    private void AoFechar(object sender, RoutedEventArgs e) => TitleBarBehavior.OnClose(this);

    /// <summary>
    /// Redimensiona a janela proporcionalmente à área útil do monitor onde ela abriu.
    /// Garante que cabe em notebooks pequenos (1366x768 com taskbar) e aproveita
    /// telas maiores sem ficar com sobras enormes. Respeita o MinWidth/MinHeight do XAML.
    /// </summary>
    private void AjustarAoMonitor(object? sender, RoutedEventArgs e)
    {
        var tela = System.Windows.Forms.Screen.FromHandle(
            new System.Windows.Interop.WindowInteropHelper(this).Handle);
        var area = tela.WorkingArea;  // já desconta taskbar
        // 85% da área útil, capado pra não passar do conteúdo
        Width = Math.Max(MinWidth, Math.Min(area.Width * 0.85, 1400));
        Height = Math.Max(MinHeight, Math.Min(area.Height * 0.95, 900));
        Left = area.Left + (area.Width - Width) / 2;
        Top = area.Top + (area.Height - Height) / 2;
    }

    private void AoTeclar(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ViewModels.MainViewModel vm) return;

        if (e.Key == Key.Space) { vm.IniciarOuPausarCommand.Execute(null); e.Handled = true; }
        else if (e.Key == Key.Right) { vm.ProximoCommand.Execute(null); e.Handled = true; }
        else if (e.Key == Key.Left)  { vm.AnteriorCommand.Execute(null); e.Handled = true; }
    }

    private void AoClicarFullscreen(object sender, RoutedEventArgs e)
    {
        var displays = System.Windows.Forms.Screen.AllScreens;
        if (displays.Length < 2)
        {
            MessageBox.Show(this,
                "Apenas 1 monitor detectado. Conecte a TV via HDMI e tente de novo.",
                "Sem segundo monitor", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // Pega o monitor diferente do qual o controle está
        var meu = System.Windows.Forms.Screen.FromHandle(
            new System.Windows.Interop.WindowInteropHelper(this).Handle);
        var alvo = displays.FirstOrDefault(s => !s.Equals(meu)) ?? displays.Last();

        var display = System.Windows.Application.Current.Windows
            .OfType<DisplayWindow>().FirstOrDefault();
        if (display == null)
        {
            // Display foi fechado pelo usuário — recria com o mesmo DataContext
            // do Control pra continuar refletindo o estado do timer.
            display = new DisplayWindow { DataContext = DataContext, Owner = this };
            display.Show();
        }
        display.EntrarFullscreenEm(alvo);
    }
}
