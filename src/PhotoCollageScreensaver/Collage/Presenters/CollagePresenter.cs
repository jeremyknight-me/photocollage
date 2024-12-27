using System.Windows.Media;
using System.Windows.Threading;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

public abstract class CollagePresenter
{
    private readonly IPhotoRepository photoRepo;

    protected CollagePresenter(
        ILogger logger,
        ISettingsRepository settingsRepository,
        IPhotoRepository photoRepository,
        IPhotoPathRepository photoPathRepository)
    {
        this.Logger = logger;
        this.SettingsRepository = settingsRepository;
        this.photoRepo = photoRepository;
        this.PhotoPathRepository = photoPathRepository;
    }

    public CollageSettings Configuration => this.SettingsRepository.Current;
    public ILogger Logger { get; }

    protected int DisplayViewIndex { get; set; } = -1;
    protected IPhotoPathRepository PhotoPathRepository { get; }
    protected ISettingsRepository SettingsRepository { get; }
    protected List<ICollageView> Views { get; } = new();

    public void Start()
    {
        foreach (var screen in Monitors.Monitor.GetScreens())
        {
            var collageWindow = new CollageWindow();
            this.SetupWindow(collageWindow, screen);
        }

        this.photoRepo.LoadPhotoPaths();
        this.StartAnimation();
    }

    public int GetRandomNumber(int min, int max) => Random.Shared.Next(min, max);

    public void SetupWindow<T>(T window, Monitors.Screen screen) where T : Window, ICollageView
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

    internal ImageProcessor CreateImageProcessor(string filePath)
        => this.Configuration.IsFullScreen
            ? ImageProcessorFullscreen.Create(filePath, this.Configuration)
            : new ImageProcessorCollage(filePath, this.Configuration);

    protected abstract void DisplayImageTimerTick(object sender, EventArgs e);

    protected abstract void SetUserControlPosition(UIElement control, ICollageView view);

    private void StartAnimation()
    {
        try
        {
            if (!this.PhotoPathRepository.HasPhotos)
            {
                MessageBoxHelper.DisplayError("Folder does not contain any supported photos.");
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
            this.Logger.Log(ex);
        }
    }
}
