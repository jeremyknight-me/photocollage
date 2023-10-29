using System.Windows.Controls;
using PhotoCollageScreensaver.Collage.Presenters;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal sealed class ImagePositionerFullscreen : ImagePositioner
{
    private ImagePositionerFullscreen(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
        : base(presenterToUse, controlToPosition, view)
    {
    }

    public static ImagePositionerFullscreen Create(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
        => new(presenterToUse, controlToPosition, view);

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
                this.SetCenteredPosition();
            }
        }
    }

    private void SetCenteredPosition()
    {
        var centeredHorizontal = (this.ViewportWidth - this.ControlWidth) / 2;
        var centeredVertical = (this.ViewportHeight - this.ControlHeight) / 2;
        Canvas.SetLeft(this.Control, centeredHorizontal);
        Canvas.SetTop(this.Control, centeredVertical);
    }
}
