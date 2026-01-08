using PhotoCollageScreensaver.Collage.Presenters;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal abstract class ImagePositioner
{
    protected ImagePositioner(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
    {
        Presenter = presenterToUse;
        Control = controlToPosition;
        Control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        ControlWidth = Convert.ToInt32(Control.DesiredSize.Width);
        ControlHeight = Convert.ToInt32(Control.DesiredSize.Height);
        ViewportHeight = Convert.ToInt32(view.WindowActualHeight);
        ViewportWidth = Convert.ToInt32(view.WindowActualWidth);
    }

    protected CollagePresenter Presenter { get; }
    protected UIElement Control { get; }
    protected int ControlHeight { get; }
    protected int ControlWidth { get; }
    protected int ViewportHeight { get; }
    protected int ViewportWidth { get; }

    public abstract void Position();
}
