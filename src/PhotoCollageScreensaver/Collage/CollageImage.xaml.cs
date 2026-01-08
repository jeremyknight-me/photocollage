using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using PhotoCollageScreensaver.Collage.Imaging;
using PhotoCollageScreensaver.Collage.Presenters;

namespace PhotoCollageScreensaver.Collage;

public partial class CollageImage : UserControl, IDisposable
{
    private readonly string _filePath;
    private readonly CollagePresenter _presenter;
    private readonly ICollageView _view;

    private CollageImage(string path, CollagePresenter presenterToUse, ICollageView view)
    {
        _filePath = path;
        _presenter = presenterToUse;
        _view = view;
        InitializeComponent();
        Uid = Guid.NewGuid().ToString();
    }

    public bool IsPortrait { get; private set; }

    public static CollageImage Create(string path, CollagePresenter presenterToUse, ICollageView view)
        => new(path, presenterToUse, view);

    public void FadeInImage()
    {
        try
        {
            Storyboard storyboard = new();
            DoubleAnimation animation = new()
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            Storyboard.SetTargetName(animation, MainStackPanel.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);
        }
        catch (Exception ex)
        {
            _presenter.Logger.Log(ex);
        }
    }

    public void FadeOutImage(Action<CollageImage> onCompletedAction)
    {
        try
        {
            Storyboard storyboard = new();
            DoubleAnimation animation = new()
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            storyboard.Completed += delegate
            {
                onCompletedAction(this);
            };
            Storyboard.SetTargetName(animation, MainStackPanel.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);
        }
        catch (Exception ex)
        {
            _presenter.Logger.Log(ex);
        }
    }

    private void CollageImage_OnInitialized(object sender, EventArgs e)
    {
        try
        {
            BorderType borderType = _presenter.Configuration.PhotoBorderType;
            if (borderType == BorderType.None || _presenter.Configuration.IsFullScreen)
            {
                MainBorder.BorderThickness = new Thickness(0);
                InnerBorder.BorderThickness = new Thickness(0);
            }
            else
            {
                MainBorder.BorderThickness = new Thickness(10);
                InnerBorder.BorderThickness = new Thickness(1);

                if (borderType == BorderType.BorderHeader)
                {
                    LoadBorderData(HeaderTextBlock);
                }
                else if (borderType == BorderType.BorderFooter)
                {
                    LoadBorderData(FooterTextBlock);
                }
            }

            if (!_presenter.Configuration.IsFullScreen)
            {
                MainImage.MaxHeight = _presenter.Configuration.MaximumSize;
                MainImage.MaxWidth = _presenter.Configuration.MaximumSize;
                RotateImageFrame();
            }

            LoadImage();
        }
        catch (Exception ex)
        {
            _presenter.Logger.Log(ex);
        }
    }

    private void LoadBorderData(TextBlock textBlock)
    {
        textBlock.Text = GetDate();
        textBlock.Visibility = Visibility.Visible;
    }

    private string GetDate()
    {
        using FileStream fs = new(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        BitmapCreateOptions options = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
        BitmapSource image = BitmapFrame.Create(fs, options, BitmapCacheOption.None);
        var metadata = image.Metadata as BitmapMetadata;
        return metadata?.DateTaken is null
            ? File.GetLastWriteTime(_filePath).ToShortDateString()
            : Convert.ToDateTime(metadata.DateTaken).ToShortDateString();
    }

    private void RotateImageFrame()
    {
        var maximumAngle = _presenter.Configuration.MaximumRotation;
        var angle = CollagePresenter.GetRandomNumber(-maximumAngle, maximumAngle);
        var transform = new RotateTransform(angle);
        MainStackPanel.RenderTransform = transform;
    }

    private void LoadImage()
    {
        ImageProcessor processor = _presenter.CreateImageProcessor(_filePath);
        MainImage.Source = processor.GetImageSource(_view);
        IsPortrait = MainImage.Source.Height > MainImage.Source.Width;
    }

    public void Dispose()
    {
        MainImage.Source = null;
        DataContext = null;
        GC.SuppressFinalize(this);
    }
}
