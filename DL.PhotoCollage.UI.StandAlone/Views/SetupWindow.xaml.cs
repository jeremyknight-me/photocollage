using System.Windows;
using DL.PhotoCollage.Presentation;

namespace DL.PhotoCollage.UI.StandAlone.Views
{
    public partial class SetupWindow : Window
    {
        private readonly ApplicationController controller;

        public SetupWindow(ApplicationController controllerToUse)
        {
            this.controller = controllerToUse;
            this.DataContext = this.controller.MakeSetupViewModel();
            this.InitializeComponent();
        }

        public SetupViewModel ViewModel
        {
            get { return this.DataContext as SetupViewModel; }
        }

        private void DirectorySelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.RequestDirectoryFromUser();
            this.DirectoryTextBox.Text = this.ViewModel.Configuration.Directory;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //this.ViewModel.Save();

            var collagePresenter = this.controller.CollagePresenter;

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                var collageWindow = new CollageWindow(this.controller);
                collagePresenter.SetupWindow(collageWindow, screen);
            }

            collagePresenter.StartAnimation();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.controller.Shutdown();
        }
    }
}
