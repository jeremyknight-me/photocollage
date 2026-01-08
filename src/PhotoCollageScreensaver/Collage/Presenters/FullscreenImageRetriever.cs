using System.Collections.Concurrent;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

internal abstract class FullscreenImageRetriever(IPhotoPathRepository photoPathRepository)
{
    public IPhotoPathRepository PhotoPathRepository { get; } = photoPathRepository;

    public abstract CollageImage GetNextImage(ICollageView view, CollagePresenter presenter);
}

internal sealed class CenteredImageRetriever(IPhotoPathRepository photoPathRepository)
        : FullscreenImageRetriever(photoPathRepository)
{
    public override CollageImage GetNextImage(ICollageView view, CollagePresenter presenter)
    {
        var path = PhotoPathRepository.GetNextPath();
        return CollageImage.Create(path, presenter, view);
    }
}

internal sealed class MatchedImageRetriever(IPhotoPathRepository photoPathRepository)
    : FullscreenImageRetriever(photoPathRepository)
{
    private readonly ConcurrentQueue<string> _skippedPortraitPaths = new();
    private readonly ConcurrentQueue<string> _skippedLandscapePaths = new();

    public override CollageImage GetNextImage(ICollageView view, CollagePresenter presenter)
    {
        int count = 0;
        while(true)
        {
            var path = GetPathByOrientation(view);
            var control = CollageImage.Create(path, presenter, view);

            count++;
            if (control.IsPortrait == view.IsPortrait || count > 20)
            {
                return control;
            }

            if (control.IsPortrait)
            {
                _skippedPortraitPaths.Enqueue(path);
            }
            else
            {
                _skippedLandscapePaths.Enqueue(path);
            }
        }
    }

    private string GetPathByOrientation(ICollageView view)
    {
        if (view.IsPortrait && _skippedPortraitPaths.TryDequeue(out var portraitPath))
        {
            return portraitPath;
        }

        if (!view.IsPortrait && _skippedLandscapePaths.TryDequeue(out var landscapePath))
        {
            return landscapePath;
        }

        return PhotoPathRepository.GetNextPath();
    }
}


