using System.Collections.Concurrent;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal sealed class CollagePresenterFullscreen : CollagePresenter
{
    private readonly ConcurrentDictionary<int, CollageImage> _images = new();
    private readonly FullscreenImageRetriever _imageRetriever;

    public CollagePresenterFullscreen(
        ILogger logger,
        ISettingsRepository settingsRepository,
        IPhotoRepository photoRepository,
        IPhotoPathRepository photoPathRepository)
        : base(logger, settingsRepository, photoRepository, photoPathRepository)
    {
        _imageRetriever = settingsRepository.Current.FullscreenMatchOrientation
            ? new MatchedImageRetriever(photoPathRepository)
            : new CenteredImageRetriever(photoPathRepository);
    }

    protected override void DisplayImageTimerTick(object sender, EventArgs e)
    {
        try
        {
            List<CollageImage> imagesToRemove = [];
            List<CollageImage> imagesToShow = [];
            foreach ((ICollageView view, var index) in Views.WithIndex())
            {
                if (_images.TryGetValue(index, out CollageImage existingImage))
                {
                    imagesToRemove.Add(existingImage);
                }

                CollageImage control = _imageRetriever.GetNextImage(view, this);
                view.ImageCanvas.Children.Add(control);
                SetUserControlPosition(control, view);

                _images.AddOrUpdate(
                    key: index,
                    addValue: control,
                    updateValueFactory: (key, oldValue) => control);

                imagesToShow.Add(control);
            }

            imagesToRemove.ForEach(RemoveImageFromQueue);
            imagesToShow.ForEach(image => image.FadeInImage());
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
            ShutdownHelper.Shutdown();
        }
    }

    private void RemoveImageFromQueue(CollageImage control)
    {
        Action<CollageImage> action = RemoveImageFromPanel;
        control.FadeOutImage(action);
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = ImagePositionerFullscreen.Create(this, control, view);
        positioner.Position();
    }
}
