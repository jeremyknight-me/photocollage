using System;
using System.Windows;
using System.Windows.Controls;
using DL.PhotoCollage.Core;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (this.ViewModel.Configuration.Speed)
            {
                case ScreensaverSpeed.Slow:
                    this.SpeedComboBox.SelectedItem = this.SpeedComboBox.Items[0];
                    break;
                case ScreensaverSpeed.Medium:
                    this.SpeedComboBox.SelectedItem = this.SpeedComboBox.Items[1];
                    break;
                case ScreensaverSpeed.Fast:
                    this.SpeedComboBox.SelectedItem = this.SpeedComboBox.Items[2];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DirectorySelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.RequestDirectoryFromUser();
            this.DirectoryTextBox.Text = this.ViewModel.Configuration.Directory;
        }

        private void SpeedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = ((ComboBoxItem)this.SpeedComboBox.SelectedItem).Content.ToString();
            this.ViewModel.SetScreensaverSpeed(selectedValue);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
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
