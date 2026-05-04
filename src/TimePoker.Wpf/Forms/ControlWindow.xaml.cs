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
        display?.EntrarFullscreenEm(alvo);
    }
}
