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
        var centeredHorizontal = (ViewportWidth - ControlWidth) / 2;
        var centeredVertical = (ViewportHeight - ControlHeight) / 2;
        Canvas.SetLeft(Control, centeredHorizontal);
        Canvas.SetTop(Control, centeredVertical);
    }
}
