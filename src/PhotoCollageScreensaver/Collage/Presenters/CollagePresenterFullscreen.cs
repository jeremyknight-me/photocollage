using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal sealed class CollagePresenterFullscreen : CollagePresenter
{
    private readonly List<ConcurrentQueue<CollageImage>> _imageQueues = [];
    private readonly Queue<string> _skippedPortraitImagePaths = new();
    private readonly Queue<string> _skippedLandscapeImagePaths = new();

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
            foreach ((ICollageView view, var index) in Views.WithIndex())
            {
                DisplayViewIndex = index;
                RemoveImageFromQueue();
                CollageImage control = AddImageToQueue();
                SetUserControlPosition(control, view);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            ShutdownHelper.Shutdown();
        }
    }

    private CollageImage GetNextImageForView()
    {
        ICollageView view = Views[DisplayViewIndex];
        if (view.IsPortrait && _skippedPortraitImagePaths.Count > 0)
        {
            return CollageImage.Create(_skippedPortraitImagePaths.Dequeue(), this, view);
        }
        else if (!view.IsPortrait && _skippedLandscapeImagePaths.Count > 0)
        {
            return CollageImage.Create(_skippedLandscapeImagePaths.Dequeue(), this, view);
        }

        for (var i = 0; i < 10; i++) // Only go through this 10 iterations and if we don't get a perfect fit just return the next image after that
        {
            var path = PhotoPathRepository.GetNextPath();
            var processor = ImageProcessorFullscreen.Create(path, SettingsRepository.Current);
            BitmapSource image = processor.GetImage();
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
                    _skippedPortraitImagePaths.Enqueue(path);
                }
                else
                {
                    _skippedLandscapeImagePaths.Enqueue(path);
                }
            }
        }

        // If we don't get one that fits the monitor alignment just return the next image
        return CollageImage.Create(PhotoPathRepository.GetNextPath(), this, view);
    }

    private CollageImage AddImageToQueue(int retryCount = 0)
    {
        ICollageView view = Views[DisplayViewIndex];
        CollageImage control = GetNextImageForView();
        try
        {
            view.ImageCanvas.Children.Add(control);
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            if (retryCount > 3)
            {
                throw new Exception("AddImageToQueue retry count failed");
            }

            AddImageToQueue(retryCount++);
        }

        _imageQueues[DisplayViewIndex].Enqueue(control);
        return control;
    }

    private void RemoveImageFromQueue()
    {
        if (_imageQueues[DisplayViewIndex].TryDequeue(out CollageImage control))
        {
            Action<CollageImage> action = RemoveImageFromPanelForView; // RemoveImageFromPanel
            control.FadeOutImage(action);
        }
    }

    private void RemoveImageFromPanelForView(CollageImage control)
    {
        try
        {
            ICollageView view = Views[DisplayViewIndex];
            if (view.ImageCanvas.Children.Contains(control))
            {
                view.ImageCanvas.Children.Remove(control);
                control.Dispose();
            }
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = ImagePositionerFullscreen.Create(this, control, view);
        positioner.Position();
    }
}
