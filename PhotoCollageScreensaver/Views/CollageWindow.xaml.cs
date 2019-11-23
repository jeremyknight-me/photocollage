using PhotoCollageScreensaver.Contracts;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoCollageScreensaver.Views
{
    public partial class CollageWindow : Window, ICollageView
    {
        private readonly ApplicationController controller;

        private Point? initialMousePosition;

        public CollageWindow(ApplicationController controllerToUse)
        {
            this.controller = controllerToUse;
            this.InitializeComponent();
        }

        public Canvas ImageCanvas => this.MainCanvas;
        public double WindowActualHeight => this.ActualHeight;
        public double WindowActualWidth => this.ActualWidth;
        private void Window_KeyDown(object sender, KeyEventArgs e) => this.controller.Shutdown();
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => this.controller.Shutdown();
        private void Window_TouchDown(object sender, TouchEventArgs e) => this.controller.Shutdown();

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            // Shut down application when mouse has moved significantly
            Point position = e.GetPosition(this);

            if (!initialMousePosition.HasValue)
            {
                initialMousePosition = position;
            }
            else if (Math.Abs(initialMousePosition.Value.X - position.X) > 10
                || Math.Abs(initialMousePosition.Value.Y - position.Y) > 10)
            {
                this.controller.Shutdown();
            }
        }
    }
}
