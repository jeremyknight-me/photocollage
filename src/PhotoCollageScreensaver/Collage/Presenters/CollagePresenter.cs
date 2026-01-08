using System.Windows.Media;
using System.Windows.Threading;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Monitors;
using PhotoCollageScreensaver.Photos;

namespace PhotoCollageScreensaver.Collage.Presenters;

public abstract class CollagePresenter
{
    private readonly IPhotoRepository _photoRepo;

    protected CollagePresenter(
        ILogger logger,
        ISettingsRepository settingsRepository,
        IPhotoRepository photoRepository,
        IPhotoPathRepository photoPathRepository)
    {
        Logger = logger;
        SettingsRepository = settingsRepository;
        _photoRepo = photoRepository;
        PhotoPathRepository = photoPathRepository;
    }

    public CollageSettings Configuration => SettingsRepository.Current;
    public ILogger Logger { get; }

    protected int DisplayViewIndex { get; set; } = -1;
    protected IPhotoPathRepository PhotoPathRepository { get; }
    protected ISettingsRepository SettingsRepository { get; }
    protected List<ICollageView> Views { get; } = [];

    public void Start()
    {
        List<Screen> screens = Monitor.GetScreens();
        foreach (Screen screen in screens)
        {
            var collageWindow = new CollageWindow();
            SetupWindow(collageWindow, screen);
        }

        _photoRepo.LoadPhotoPaths();
        StartAnimation();
    }

    public static int GetRandomNumber(int min, int max) => Random.Shared.Next(min, max);

    public void SetupWindow<T>(T window, Screen screen) where T : Window, ICollageView
    {
        var backgroundBrush = new SolidColorBrush
        {
            Opacity = SettingsRepository.Current.Opacity,
            Color = Colors.Black
        };
        window.Background = backgroundBrush;
        window.Left = screen.Left;
        window.Top = screen.Top;
        window.Width = screen.Width;
        window.Height = screen.Height;
        window.Show();
        Views.Add(window);
    }

    internal ImageProcessor CreateImageProcessor(string filePath)
        => Configuration.IsFullScreen
            ? ImageProcessorFullscreen.Create(filePath, Configuration)
            : new ImageProcessorCollage(filePath, Configuration);

    protected abstract void DisplayImageTimerTick(object sender, EventArgs e);

    protected abstract void SetUserControlPosition(UIElement control, ICollageView view);

    protected void RemoveImageFromPanel(CollageImage control)
    {
        try
        {
            foreach (ICollageView view in Views)
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
            Logger.Log(ex);
        }
    }

    private void StartAnimation()
    {
        try
        {
            if (!PhotoPathRepository.HasPhotos)
            {
                MessageBoxHelper.DisplayError("Folder does not contain any supported photos.");
                ShutdownHelper.Shutdown();
            }

            DisplayImageTimerTick(null, null);

            var seconds = (int)SettingsRepository.Current.Speed;
            DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(seconds) };
            timer.Tick += DisplayImageTimerTick;
            timer.Start();
        }
        catch (Exception ex)
        {
            Logger.Log(ex);
        }
    }
}
