using PhotoCollageScreensaver.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PhotoCollageScreensaver
{
    internal class ImagePositioner
    {
        private readonly CollagePresenter presenter;
        private readonly UIElement control;
        private readonly int controlHeight;
        private readonly int controlWidth;
        private readonly int viewportHeight;
        private readonly int viewportWidth;

        public ImagePositioner(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
        {
            this.presenter = presenterToUse;
            this.control = controlToPosition;
            this.control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.controlWidth = Convert.ToInt32(this.control.DesiredSize.Width);
            this.controlHeight = Convert.ToInt32(this.control.DesiredSize.Height);
            this.viewportHeight = Convert.ToInt32(view.WindowActualHeight);
            this.viewportWidth = Convert.ToInt32(view.WindowActualWidth);
        }

        public void Position()
        {
            this.SetHorizontalPosition();
            this.SetVerticalPosition();
        }

        private void SetHorizontalPosition()
        {
            var position = this.presenter.GetRandomNumber(0, this.viewportWidth);
            var max = this.viewportWidth - this.controlWidth;

            if (position > max)
            {
                position = max;
            }

            Canvas.SetLeft(this.control, position);
        }

        private void SetVerticalPosition()
        {
            var position = this.presenter.GetRandomNumber(0, this.viewportHeight);
            var max = this.viewportHeight - this.controlHeight;

            if (position > max)
            {
                position = max;
            }

            Canvas.SetTop(this.control, position);
        }
    }
}
