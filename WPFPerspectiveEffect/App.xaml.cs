using System.Windows;

namespace WPFPerspectiveEffect;

public sealed partial class App : Application
{
    protected override void OnStartup(StartupEventArgs args)
    {
        base.OnStartup(args);

        MainWindow window = new();

        window.Show();
    }
}