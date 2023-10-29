using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

public abstract class CollagePresenter
{
    internal CollagePresenter(
        ISettingsRepository settingsRepository,
        IPhotoRepository photoRepository,
        ErrorHandler errorHandler)
    {
        this.SettingsRepository = settingsRepository;
        this.ErrorHandler = errorHandler;
        this.PhotoRepository = photoRepository;
    }

    public CollageSettings Configuration => this.SettingsRepository.Current;
    public ErrorHandler ErrorHandler { get; }

    protected int DisplayViewIndex { get; set; } = -1;
    protected IPhotoRepository PhotoRepository { get; }
    protected ISettingsRepository SettingsRepository { get; }
    protected List<ICollageView> Views { get; } = new();

    public void StartAnimation()
    {
        try
        {
            if (!this.PhotoRepository.HasPhotos)
            {
                this.ErrorHandler.DisplayErrorMessage("Folder does not contain any supported photos.");
                ShutdownHelper.Shutdown();
            }

            this.DisplayImageTimerTick(null, null);

            var seconds = (int)this.SettingsRepository.Current.Speed;
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, seconds) };
            timer.Tick += this.DisplayImageTimerTick;
            timer.Start();
        }
        catch (Exception ex)
        {
            this.ErrorHandler.HandleError(ex);
        }
    }

    public int GetRandomNumber(int min, int max)
    {
        var value = 0;
        var random = Random.Shared.Next(min, max);
        Interlocked.Exchange(ref value, random);
        return value;
    }

    public virtual void SetupWindow<T>(T window, Monitors.Screen screen) where T : Window, ICollageView
    {
        var backgroundBrush = new SolidColorBrush
        {
            Opacity = this.SettingsRepository.Current.Opacity,
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
