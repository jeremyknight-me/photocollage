using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using PhotoCollage.Common.Enums;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver.UserControls;

public partial class CollageImage : UserControl, IDisposable
{
    private readonly string filePath;
    private readonly CollagePresenter presenter;
    private readonly ICollageView view;

    public CollageImage(string path, CollagePresenter presenterToUse, ICollageView view)
    {
        this.filePath = path;
        this.presenter = presenterToUse;
        this.view = view;
        this.InitializeComponent();
        this.Uid = Guid.NewGuid().ToString();
    }

    public bool IsPortrait { get; private set; }

    public void FadeOutImage(Action<CollageImage> onCompletedAction)
    {
        try
        {
            var storyboard = new Storyboard();
            var duration = new TimeSpan(0, 0, 1);
            var animation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
            storyboard.Completed += delegate
            {
                onCompletedAction(this);
            };
            Storyboard.SetTargetName(animation, this.MainStackPanel.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            storyboard.Children.Add(animation);
            storyboard.Begin(this);
        }
        catch (Exception ex)
        {
            this.presenter.HandleError(ex);
        }
    }

    private void CollageImage_OnInitialized(object sender, EventArgs e)
    {
        try
        {
            var borderType = this.presenter.Configuration.PhotoBorderType;
            if (borderType == BorderType.None)
            {
                this.MainBorder.BorderThickness = new Thickness(0);
                this.InnerBorder.BorderThickness = new Thickness(0);
            }
            else
            {
                this.MainBorder.BorderThickness = new Thickness(10);
                this.InnerBorder.BorderThickness = new Thickness(1);

                if (borderType == BorderType.BorderHeader)
                {
                    this.LoadBorderData(this.HeaderTextBlock);
                }
                else if (borderType == BorderType.BorderFooter)
                {
                    this.LoadBorderData(this.FooterTextBlock);
                }
            }

            if (!this.presenter.Configuration.IsFullScreen)
            {
                this.MainImage.MaxHeight = this.presenter.Configuration.MaximumSize;
                this.MainImage.MaxWidth = this.presenter.Configuration.MaximumSize;
                this.RotateImageFrame();
            }
            this.LoadImage();
        }
        catch (Exception ex)
        {
            this.presenter.HandleError(ex);
        }
    }

    private void LoadBorderData(TextBlock textBlock)
    {
        textBlock.Text = this.GetDate();
        textBlock.Visibility = Visibility.Visible;
    }

    private string GetDate()
    {
        using var fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var options = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
        BitmapSource image = BitmapFrame.Create(fs, options, BitmapCacheOption.None);
        var metadata = (BitmapMetadata)image.Metadata;
        return !(metadata.DateTaken is null)
            ? Convert.ToDateTime(metadata.DateTaken).ToShortDateString()
            : File.GetLastWriteTime(this.filePath).ToShortDateString();
    }

    private void RotateImageFrame()
    {
        var maximumAngle = this.presenter.Configuration.MaximumRotation;
        var angle = this.presenter.GetRandomNumber(-maximumAngle, maximumAngle);
        var transform = new RotateTransform(angle);
        this.MainStackPanel.RenderTransform = transform;
    }

    private void LoadImage()
    {
        IImageProcessor processor = this.presenter.Configuration.IsFullScreen
            ? new ImageProcessorFullscreen(this.filePath, this.presenter.Configuration)
            : new ImageProcessorCollage(this.filePath, this.presenter.Configuration);
        this.MainImage.Source = processor.GetScaledImage(this.view);


        if (!this.presenter.Configuration.IsFullScreen)
        {
            this.MainImage.MaxHeight = this.MainImage.Source.Height;
            this.MainImage.MaxWidth = this.MainImage.Source.Width;
        }

        this.IsPortrait = this.MainImage.Source.Height > this.MainImage.Source.Width;
    }

    public void Dispose()
    {
        this.MainImage.Source = null;
        this.MainStackPanel = null;
        this.DataContext = null;
    }
}
