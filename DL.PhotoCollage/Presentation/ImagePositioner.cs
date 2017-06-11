using System;
using System.Windows;
using System.Windows.Controls;

namespace DL.PhotoCollage.Presentation
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
            int position = this.presenter.GetRandomNumber(0, this.viewportWidth);
            int max = this.viewportWidth - this.controlWidth;

            if (position > max)
            {
                position = max;
            }

            Canvas.SetLeft(control, position);
        }

        private void SetVerticalPosition()
        {
            int position = this.presenter.GetRandomNumber(0, this.viewportHeight);
            int max = this.viewportHeight - this.controlHeight;

            if (position > max)
            {
                position = max;
            }

            Canvas.SetTop(control, position);
        }
    }
}
