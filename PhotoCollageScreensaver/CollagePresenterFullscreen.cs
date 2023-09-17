using System.Collections.Concurrent;
using PhotoCollageScreensaver.UserControls;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

public sealed class CollagePresenterFullscreen : CollagePresenter
{
    private readonly List<ConcurrentQueue<CollageImage>> imageQueues;
    private readonly Queue<string> skippedPortraitImagePaths;
    private readonly Queue<string> skippedLandscapeImagePaths;

    public CollagePresenterFullscreen(ApplicationController controllerToUse, CollageSettings configurationToUse)
        : base(controllerToUse, configurationToUse)
    {
        this.imageQueues = new List<ConcurrentQueue<CollageImage>>();
        this.skippedPortraitImagePaths = new Queue<string>();
        this.skippedLandscapeImagePaths = new Queue<string>();
    }

    public override void SetupWindow<T>(T window, Monitors.Screen screen)
    {
        base.SetupWindow(window, screen);
        this.imageQueues.Add(new ConcurrentQueue<CollageImage>());
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
            this.Controller.HandleError(ex);
            this.Controller.Shutdown();
        }
    }

    private CollageImage GetNextImageForView()
    {
        var view = this.Views[this.DisplayViewIndex];
        if (view.IsPortrait && this.skippedPortraitImagePaths.Count > 0)
        {
            return new CollageImage(this.skippedPortraitImagePaths.Dequeue(), this, view);
        }
        else if (!view.IsPortrait && this.skippedLandscapeImagePaths.Count > 0)
        {
            return new CollageImage(this.skippedLandscapeImagePaths.Dequeue(), this, view);
        }

        for (var i = 0; i < 10; i++) // Only go through this 10 iterations and if we don't get a perfect fit just return the next image after that
        {
            var path = this.PhotoRepository.GetNextPhotoFilePath();
            var processor = new ImageProcessorFullscreen(path, this.Configuration);
            var image = processor.GetImage();
            var isImagePortait = image.Height > image.Width;
            isImagePortait = processor.ImageIsRotatedPlusMinusNinetyDegrees ? !isImagePortait : isImagePortait;

            if (view.IsPortrait && isImagePortait)
            {
                return new CollageImage(path, this, view);
            }
            else if (!view.IsPortrait && !isImagePortait)
            {
                return new CollageImage(path, this, view);
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
        return new CollageImage(this.PhotoRepository.GetNextPhotoFilePath(), this, view);
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
            this.Controller.HandleError(ex);
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
            this.Controller.HandleError(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = new ImagePositionerFullscreen(this, control, view);
        positioner.Position();
    }
}
