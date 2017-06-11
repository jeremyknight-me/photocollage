using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DL.PhotoCollage.Presentation;

namespace DL.PhotoCollage.UI.StandAlone.Views
{
    public partial class CollageWindow : Window, ICollageView
    {
        private readonly ApplicationController controller;

        public CollageWindow(ApplicationController controllerToUse)
        {
            this.controller = controllerToUse;
            this.InitializeComponent();
        }

        public Canvas ImageCanvas
        {
            get { return this.MainCanvas; }
        }

        public double WindowActualHeight
        {
            get { return this.ActualHeight; }
        }

        public double WindowActualWidth
        {
            get { return this.ActualWidth; }
        }

        private void ApplicationClose(object sender, ExecutedRoutedEventArgs e)
        {
            this.controller.CollagePresenter.Close();
            this.controller.Reset();
        }
    }
}
