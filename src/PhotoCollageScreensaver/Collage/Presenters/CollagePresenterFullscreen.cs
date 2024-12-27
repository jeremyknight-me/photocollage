using System.Collections.Concurrent;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal sealed class CollagePresenterFullscreen : CollagePresenter
{
    private readonly List<ConcurrentQueue<CollageImage>> imageQueues = new();
    private readonly Queue<string> skippedPortraitImagePaths = new();
    private readonly Queue<string> skippedLandscapeImagePaths = new();

    public CollagePresenterFullscreen(
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
            foreach (var (view, index) in this.Views.WithIndex())
            {
                this.DisplayViewIndex = index;
                this.RemoveImageFromQueue();
                var control = this.AddImageToQueue();
                this.SetUserControlPosition(control, view);
            }
        }
        catch (Exception ex)
        {
            this.Logger.Log(ex);
            ShutdownHelper.Shutdown();
        }
    }

    private CollageImage GetNextImageForView()
    {
        var view = this.Views[this.DisplayViewIndex];
        if (view.IsPortrait && this.skippedPortraitImagePaths.Count > 0)
        {
            return CollageImage.Create(this.skippedPortraitImagePaths.Dequeue(), this, view);
        }
        else if (!view.IsPortrait && this.skippedLandscapeImagePaths.Count > 0)
        {
            return CollageImage.Create(this.skippedLandscapeImagePaths.Dequeue(), this, view);
        }

        for (var i = 0; i < 10; i++) // Only go through this 10 iterations and if we don't get a perfect fit just return the next image after that
        {
            var path = this.PhotoPathRepository.GetNextPath();
            var processor = ImageProcessorFullscreen.Create(path, this.SettingsRepository.Current);
            var image = processor.GetImage();
            var isImagePortait = image.Height > image.Width;
            isImagePortait = processor.ImageIsRotatedPlusMinusNinetyDegrees ? !isImagePortait : isImagePortait;

            if (view.IsPortrait && isImagePortait)
            {
                return CollageImage.Create(path, this, view);
            }
            else if (!view.IsPortrait && !isImagePortait)
            {
                return CollageImage.Create(path, this, view);
            }
            else
            {
                if (isImagePortait)
                {
                    this.skippedPortraitImagePaths.Enqueue(path);
                }
                else
                {
                    this.skippedLandscapeImagePaths.Enqueue(path);
                }
            }
        }

        // If we don't get one that fits the monitor alignment just return the next image
        return CollageImage.Create(this.PhotoPathRepository.GetNextPath(), this, view);
    }

    private CollageImage AddImageToQueue(int retryCount = 0)
    {
        var view = this.Views[this.DisplayViewIndex];
        var control = this.GetNextImageForView();
        try
        {
            view.ImageCanvas.Children.Add(control);
        }
        catch (Exception ex)
        {
            this.Logger.Log(ex);
            if (retryCount > 3)
            {
                throw new Exception("AddImageToQueue retry count failed");
            }

            this.AddImageToQueue(retryCount++);
        }

        this.imageQueues[this.DisplayViewIndex].Enqueue(control);
        return control;
    }

    private void RemoveImageFromQueue()
    {
        if (this.imageQueues[this.DisplayViewIndex].TryDequeue(out var control))
        {
            Action<CollageImage> action = this.RemoveImageFromPanelForView; // RemoveImageFromPanel
            control.FadeOutImage(action);
        }
    }

    private void RemoveImageFromPanelForView(CollageImage control)
    {
        try
        {
            var view = this.Views[this.DisplayViewIndex];
            if (view.ImageCanvas.Children.Contains(control))
            {
                view.ImageCanvas.Children.Remove(control);
                control.Dispose();
            }
        }
        catch (Exception ex)
        {
            this.Logger.Log(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = ImagePositionerFullscreen.Create(this, control, view);
        positioner.Position();
    }
}
