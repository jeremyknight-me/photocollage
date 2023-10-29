using PhotoCollageScreensaver.Collage.Presenters;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal abstract class ImagePositioner
{
    protected ImagePositioner(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
    {
        this.Presenter = presenterToUse;
        this.Control = controlToPosition;
        this.Control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        this.ControlWidth = Convert.ToInt32(this.Control.DesiredSize.Width);
        this.ControlHeight = Convert.ToInt32(this.Control.DesiredSize.Height);
        this.ViewportHeight = Convert.ToInt32(view.WindowActualHeight);
        this.ViewportWidth = Convert.ToInt32(view.WindowActualWidth);
    }

    protected CollagePresenter Presenter { get; }
    protected UIElement Control { get; }
    protected int ControlHeight { get; }
    protected int ControlWidth { get; }
    protected int ViewportHeight { get; }
    protected int ViewportWidth { get; }

    public abstract void Position();
}
