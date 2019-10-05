using PhotoCollageScreensaver.ViewModels;
using System.Windows;

namespace PhotoCollageScreensaver.Views
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
            this.DirectoryTextBox.Text = this.ViewModel.Config.Directory;
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

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            this.controller.StartScreensaver();
        }
    }
}
