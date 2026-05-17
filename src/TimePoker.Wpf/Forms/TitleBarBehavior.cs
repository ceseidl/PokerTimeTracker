using System.Windows;

namespace TimePoker.Wpf.Forms;

internal static class TitleBarBehavior
{
    public static void Attach(Window w)
    {
        w.StateChanged += (_, _) =>
            w.BorderThickness = w.WindowState == WindowState.Maximized
                ? new Thickness(7)
                : new Thickness(0);
    }

    public static void OnMinimize(Window w) => SystemCommands.MinimizeWindow(w);

    public static void OnMaxRestore(Window w)
    {
        if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
        else SystemCommands.MaximizeWindow(w);
    }

    public static void OnClose(Window w) => SystemCommands.CloseWindow(w);
}
