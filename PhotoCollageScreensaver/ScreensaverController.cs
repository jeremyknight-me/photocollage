using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

public sealed class ScreensaverController
{
    private readonly CollagePresenter presenter;

    public ScreensaverController(CollagePresenter collagePresenter)
    {
        this.presenter = collagePresenter;
    }

    public void Start()
    {
        foreach (var screen in Monitors.Monitor.GetScreens())
        {
            var collageWindow = new CollageWindow();
            this.presenter.SetupWindow(collageWindow, screen);
        }

        this.presenter.StartAnimation();
    }
}
