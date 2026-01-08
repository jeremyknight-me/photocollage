using System.Windows.Controls;
using PhotoCollageScreensaver.Collage.Presenters;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal sealed class ImagePositionerCollage : ImagePositioner
{
    private ImagePositionerCollage(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
        : base(presenterToUse, controlToPosition, view)
    {
    }

    public static ImagePositionerCollage Create(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
        => new(presenterToUse, controlToPosition, view);

    public override void Position()
    {
        SetHorizontalPosition();
        SetVerticalPosition();
    }

    private void SetHorizontalPosition()
    {
        var position = CollagePresenter.GetRandomNumber(0, ViewportWidth);
        var max = ViewportWidth - ControlWidth;

        if (position > max)
        {
            position = max;
        }

        Canvas.SetLeft(Control, position);
    }

    private void SetVerticalPosition()
    {
        var position = CollagePresenter.GetRandomNumber(0, ViewportHeight);
        var max = ViewportHeight - ControlHeight;

        if (position > max)
        {
            position = max;
        }

        Canvas.SetTop(Control, position);
    }
}
