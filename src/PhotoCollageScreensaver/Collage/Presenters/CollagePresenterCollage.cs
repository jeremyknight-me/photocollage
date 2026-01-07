using System.Collections.Concurrent;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal sealed class CollagePresenterCollage : CollagePresenter
{
    private readonly ConcurrentQueue<CollageImage> _imageQueue = new();

    public CollagePresenterCollage(
        ILogger logger,
        ISettingsRepository settingsRepository,
        IPhotoRepository photoRepository,
        IPhotoPathRepository photoPathRepository)
        : base(logger, settingsRepository, photoRepository, photoPathRepository)
    {
    }

    protected override void DisplayImageTimerTick(object sender, EventArgs e)
    {
        try
        {
            var path = PhotoPathRepository.GetNextPath();
            ICollageView view = GetNextDisplayView();
            var control = CollageImage.Create(path, this, Views[DisplayViewIndex]);
            view.ImageCanvas.Children.Add(control);
            _imageQueue.Enqueue(control);

            if (_imageQueue.Count > SettingsRepository.Current.NumberOfPhotos)
            {
                RemoveImageFromPanel(control);
            }

            SetUserControlPosition(control, view);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            ShutdownHelper.Shutdown();
        }
    }

    private ICollageView GetNextDisplayView()
    {
        var nextIndex = DisplayViewIndex + 1;
        if (nextIndex >= Views.Count)
        {
            nextIndex = 0;
        }

        DisplayViewIndex = nextIndex;
        return Views[nextIndex];
    }

    private void RemoveImageFromPanel(CollageImage control)
    {
        try
        {
            foreach (ICollageView view in Views)
            {
                if (view.ImageCanvas.Children.Contains(control))
                {
                    view.ImageCanvas.Children.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = ImagePositionerCollage.Create(this, control, view);
        positioner.Position();
    }
}
