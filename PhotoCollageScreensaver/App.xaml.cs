using System.Windows;
using System.Windows.Threading;

namespace PhotoCollageScreensaver
{
    public partial class App : Application
    {
        private ApplicationController controller;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            string commandArg = string.Empty;
            if (e.Args.Length > 0)
            {
                commandArg = e.Args[0].ToLower().Trim().Substring(0, 2);
            }

            this.controller = new ApplicationController();
            switch (commandArg)
            {
                case "/p": // preview
                    this.Shutdown();
                    break;
                case "/s": // screensaver
                    this.controller.StartScreensaver();
                    break;
                default: // no argument or /c both show config
                    this.controller.StartSetup();
                    break;
            }
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.controller.HandleError(e.Exception);
            e.Handled = true;
        }
    }
}
