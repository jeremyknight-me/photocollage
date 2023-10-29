using System.Collections.Concurrent;
using PhotoCollage.Common.Photos;
using PhotoCollage.Common.Settings;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal sealed class CollagePresenterCollage : CollagePresenter
{
    private readonly ConcurrentQueue<CollageImage> imageQueue = new();

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
            var path = this.PhotoPathRepository.GetNextPath();
            var view = this.GetNextDisplayView();
            var control = CollageImage.Create(path, this, this.Views[this.DisplayViewIndex]);
            view.ImageCanvas.Children.Add(control);
            this.imageQueue.Enqueue(control);

            if (this.imageQueue.Count > this.SettingsRepository.Current.NumberOfPhotos)
            {
                this.RemoveImageFromPanel(control);
            }

            this.SetUserControlPosition(control, view);
        }
        catch (Exception ex)
        {
            this.Logger.Log(ex);
            ShutdownHelper.Shutdown();
        }
    }

    private ICollageView GetNextDisplayView()
    {
        var nextIndex = this.DisplayViewIndex + 1;
        if (nextIndex >= this.Views.Count)
        {
            nextIndex = 0;
        }

        this.DisplayViewIndex = nextIndex;
        return this.Views[nextIndex];
    }

    private void RemoveImageFromPanel(CollageImage control)
    {
        try
        {
            foreach (var view in this.Views)
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
            this.Logger.Log(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = ImagePositionerCollage.Create(this, control, view);
        positioner.Position();
    }
}
