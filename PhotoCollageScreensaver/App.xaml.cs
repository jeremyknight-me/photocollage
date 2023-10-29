using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using PhotoCollageScreensaver.Collage.Presenters;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

public partial class App : Application
{
    private readonly IServiceProvider serviceProvider;

    public App()
    {
        this.serviceProvider = DependencyInjectionHelper.CreateServiceProvider();
    }

    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        var commandArg = string.Empty;
        if (e.Args.Length > 0)
        {
            commandArg = e.Args[0].ToLower().Trim()[..2];
        }

        switch (commandArg)
        {
            case "/p": // preview
                ShutdownHelper.Shutdown();
                break;
            case "/s": // screensaver
                var presenter = this.serviceProvider.GetService<CollagePresenter>();
                presenter.Start();
                break;
            default: // no argument or /c both show config
                var setupWindow = this.serviceProvider.GetRequiredService<SetupWindow>();
                setupWindow.Show();
                break;
        }
    }

    private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var logger = this.serviceProvider.GetRequiredService<ILogger>();
        logger.Log(e.Exception);
        e.Handled = true;
    }
}
