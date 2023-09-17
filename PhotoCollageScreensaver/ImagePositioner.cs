using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

internal abstract class ImagePositioner : IImagePositioner
{
    protected readonly CollagePresenter Presenter;
    protected readonly UIElement Control;
    protected readonly int ControlHeight;
    protected readonly int ControlWidth;
    protected readonly int ViewportHeight;
    protected readonly int ViewportWidth;

    public ImagePositioner(CollagePresenter presenterToUse, UIElement controlToPosition, ICollageView view)
    {
        this.Presenter = presenterToUse;
        this.Control = controlToPosition;
        this.Control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        this.ControlWidth = Convert.ToInt32(this.Control.DesiredSize.Width);
        this.ControlHeight = Convert.ToInt32(this.Control.DesiredSize.Height);
        this.ViewportHeight = Convert.ToInt32(view.WindowActualHeight);
        this.ViewportWidth = Convert.ToInt32(view.WindowActualWidth);
    }

    public virtual void Position()
    {
    }
}
