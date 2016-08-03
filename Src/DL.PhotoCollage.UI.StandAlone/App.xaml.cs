using System.Windows;
using System.Windows.Threading;

using DL.PhotoCollage.Presentation;
using DL.PhotoCollage.UI.StandAlone.Views;

namespace DL.PhotoCollage.UI.StandAlone
{
    public partial class App : Application
    {
        private ApplicationController controller;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            this.controller = new ApplicationController(Current);
            var setupWindow = new SetupWindow(this.controller);
            setupWindow.Show();
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.controller.HandleError(e.Exception);
            e.Handled = true;
        }
    }
}
