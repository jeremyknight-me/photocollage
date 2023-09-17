using PhotoCollageScreensaver.Views;
using System.Windows.Controls;

namespace PhotoCollageScreensaver;

internal class ImagePositionerFullscreen: ImagePositioner, IImagePositioner
{
    public ImagePositionerFullscreen(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view) : base(presenterToUse, controlToPosition, view)
    {
    
    }

    public override void Position()
    {
        if (this.Presenter.Configuration.IsFullScreen)
        {
            if (this.Presenter.Configuration.PhotoFullScreenMode == PhotoCollage.Common.Enums.FullScreenMode.Stretched)
            {
                Canvas.SetLeft(this.Control, 0);
                Canvas.SetTop(this.Control, 0);
            }
            else
            {
                this.SetCentredPosition();
            }
        }
    }

    private void SetCentredPosition()
    {
        var centredHorizontal = (this.ViewportWidth - this.ControlWidth) / 2;
        var centredVertical = (this.ViewportHeight - this.ControlHeight) / 2;
        Canvas.SetLeft(this.Control, centredHorizontal);
        Canvas.SetTop(this.Control, centredVertical);
    }
}
