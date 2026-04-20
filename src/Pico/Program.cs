using Avalonia;
using Pico.Infrastructure;

namespace Pico;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (OperatingSystem.IsWindows())
        {
            var fileRevealService = new FileRevealService(new SystemPlatformInfo(), new SystemProcessLauncher());
            if (WindowsProtocolActivation.TryHandle(args, fileRevealService))
            {
                return;
            }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
