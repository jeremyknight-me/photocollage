using PhotoCollageScreensaver.Views;
using System.Windows.Controls;

namespace PhotoCollageScreensaver;

internal class ImagePositionerCollage: ImagePositioner, IImagePositioner
{
    public ImagePositionerCollage(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view) : base(presenterToUse, controlToPosition, view)
    {
    
    }

    public override void Position()
    {
        this.SetHorizontalPosition();
        this.SetVerticalPosition();
    }

    private void SetHorizontalPosition()
    {
       var position = this.Presenter.GetRandomNumber(0, this.ViewportWidth);
       var max = this.ViewportWidth - this.ControlWidth;

       if (position > max)
       {
           position = max;
       }

       Canvas.SetLeft(this.Control, position);
    }

    private void SetVerticalPosition()
    {
       var position = this.Presenter.GetRandomNumber(0, this.ViewportHeight);
       var max = this.ViewportHeight - this.ControlHeight;

       if (position > max)
       {
           position = max;
       }

       Canvas.SetTop(this.Control, position);
    }
}
