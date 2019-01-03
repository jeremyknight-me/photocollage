using System.Windows;
using DL.PhotoCollage.Presentation;

namespace DL.PhotoCollage.UI.Screensaver.Views
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

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Save();
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Save();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
