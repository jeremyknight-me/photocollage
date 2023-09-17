using PhotoCollage.Common;
using PhotoCollageScreensaver.UserControls;
using PhotoCollageScreensaver.Views;
using System.Collections.Concurrent;

namespace PhotoCollageScreensaver;

public class CollagePresenterCollage: CollagePresenter, ICollagePresenter
{
    private readonly ConcurrentQueue<CollageImage> imageQueue;

    public CollagePresenterCollage(ApplicationController controllerToUse, CollageSettings configurationToUse) : base(controllerToUse, configurationToUse)
    {
        this.imageQueue = new ConcurrentQueue<CollageImage>();
    }

    protected override void DisplayImageTimerTick(object sender, EventArgs e)
    {
        try
        {
            var path = this.PhotoRepository.GetNextPhotoFilePath();
            var view = this.GetNextDisplayView();
            var control = new CollageImage(path, this, this.Views[this.DisplayViewIndex]);
            view.ImageCanvas.Children.Add(control);
            this.imageQueue.Enqueue(control);

            if (this.imageQueue.Count > this.Configuration.NumberOfPhotos)
            {
                this.RemoveImageFromPanel(control);
            }

            this.SetUserControlPosition(control, view);
        }
        catch (Exception ex)
        {
            this.Controller.HandleError(ex);
            this.Controller.Shutdown();
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
            this.Controller.HandleError(ex);
        }
    }

    protected override void SetUserControlPosition(UIElement control, ICollageView view)
    {
        var positioner = new ImagePositionerCollage(this, control, view);
        positioner.Position();
    }
}
