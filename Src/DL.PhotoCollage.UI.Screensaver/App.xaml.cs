using System.Windows;
using System.Windows.Threading;
using DL.PhotoCollage.Core;
using DL.PhotoCollage.Presentation;
using DL.PhotoCollage.UI.Screensaver.Views;

namespace DL.PhotoCollage.UI.Screensaver
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

            switch (commandArg)
            {
                case "/p": // preview
                    this.Shutdown();
                    break;
                case "/s": // screensaver
                    this.controller = new ApplicationController(Current);
                    this.StartScreensaver();
                    break;
                default: // no argument or /c both show config
                    this.controller = new ApplicationController(Current);
                    var setupWindow = new SetupWindow(this.controller);
                    setupWindow.Show();
                    break;
            }
        }

        private void StartScreensaver()
        {
            var collagePresenter = this.controller.CollagePresenter;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                var collageWindow = new CollageWindow(this.controller);
                collagePresenter.SetupWindow(collageWindow, screen);
            }

            collagePresenter.StartAnimation();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.controller.HandleError(e.Exception);
            e.Handled = true;
        }
    }
}
