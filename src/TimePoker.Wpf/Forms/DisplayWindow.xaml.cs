using System.Windows;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Key = System.Windows.Input.Key;

namespace TimePoker.Wpf.Forms;

/// <summary>
/// Janela de exibição (kiosk) que vai pra TV. Suporta F11 (toggle fullscreen)
/// e Esc (sai do fullscreen).
/// </summary>
public partial class DisplayWindow : Window
{
    private WindowState _estadoAnterior;
    private WindowStyle _estiloAnterior;
    private bool _fullscreen;

    public DisplayWindow()
    {
        InitializeComponent();
        KeyDown += AoTeclar;
    }

    private void AoTeclar(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11) AlternarFullscreen();
        else if (e.Key == Key.Escape && _fullscreen) AlternarFullscreen();
    }

    /// <summary>Move pro monitor escolhido e ativa fullscreen.</summary>
    public void EntrarFullscreenEm(System.Windows.Forms.Screen tela)
    {
        var bounds = tela.Bounds;
        Left = bounds.Left;
        Top = bounds.Top;
        if (!_fullscreen) AlternarFullscreen();
    }

    private void AlternarFullscreen()
    {
        if (_fullscreen)
        {
            WindowStyle = _estiloAnterior;
            WindowState = _estadoAnterior;
            ResizeMode = ResizeMode.CanResize;
            _fullscreen = false;
        }
        else
        {
            _estadoAnterior = WindowState;
            _estiloAnterior = WindowStyle;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Normal;       // garante reaplicação
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;
            _fullscreen = true;
        }
    }
}
