using PhotoCollage.Common;
using PhotoCollage.Common.Data;
using PhotoCollageScreensaver.Views;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;

namespace PhotoCollageScreensaver;

public abstract class CollagePresenter
{
    private readonly Random random;
    protected readonly IPhotoRepository PhotoRepository;
    protected readonly ApplicationController Controller;
    protected int DisplayViewIndex;
    protected readonly List<ICollageView> Views;

    public CollagePresenter(ApplicationController controllerToUse, CollageSettings configurationToUse)
    {
        this.random = new Random();
        this.Views = new List<ICollageView>();
        this.Controller = controllerToUse;
        this.Configuration = configurationToUse;
        this.PhotoRepository = new PhotoRepositoryFactory(this.Configuration).Make();
        this.DisplayViewIndex = -1;
    }

    public CollageSettings Configuration { get; }

    public void StartAnimation()
    {
        try
        {
            if (!this.PhotoRepository.HasPhotos)
            {
                this.Controller.DisplayErrorMessage("Folder does not contain any supported photos.");
                this.Controller.Shutdown();
            }

            this.DisplayImageTimerTick(null, null);

            var seconds = (int)this.Configuration.Speed;
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, seconds) };
            timer.Tick += this.DisplayImageTimerTick;
            timer.Start();
        }
        catch (Exception ex)
        {
            this.Controller.HandleError(ex);
        }
    }

    public int GetRandomNumber(int min, int max)
    {
        var value = 0;
        var random = this.random.Next(min, max);
        Interlocked.Exchange(ref value, random);
        return value;
    }

    public void HandleError(Exception ex, bool showMessage = false) => this.Controller.HandleError(ex, showMessage);

    public virtual void SetupWindow<T>(T window, Monitors.Screen screen) where T : Window, ICollageView
    {
        var backgroundBrush = new SolidColorBrush
        {
            Opacity = this.Configuration.Opacity,
            Color = Colors.Black
        };
        window.Background = backgroundBrush;
        window.Left = screen.Left;
        window.Top = screen.Top;
        window.Width = screen.Width;
        window.Height = screen.Height;
        window.Show();
        this.Views.Add(window);
    }

    protected abstract void DisplayImageTimerTick(object sender, EventArgs e);

    protected abstract void SetUserControlPosition(UIElement control, ICollageView view);
}
