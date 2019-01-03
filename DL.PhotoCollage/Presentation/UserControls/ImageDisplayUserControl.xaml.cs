using DL.PhotoCollage.Core;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DL.PhotoCollage.Presentation.UserControls
{
    public partial class ImageDisplayUserControl : UserControl
    {
        private const int maximumAngle = 15;

        private readonly string filePath;

        private readonly CollagePresenter presenter;

        public ImageDisplayUserControl(string path, CollagePresenter presenterToUse)
        {
            this.filePath = path;
            this.presenter = presenterToUse;
            this.InitializeComponent();
        }

        public void FadeOutImage(Action<ImageDisplayUserControl, ICollageView> onCompletedAction, ICollageView view)
        {
            try
            {
                var storyboard = new Storyboard();
                var duration = new TimeSpan(0, 0, 1);

                var animation
                    = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = new Duration(duration)
                    };

                storyboard.Completed += delegate { onCompletedAction(this, view); };

                Storyboard.SetTargetName(animation, this.MainBorder.Name);
                Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
                storyboard.Children.Add(animation);
                storyboard.Begin(this);
            }
            catch (Exception ex)
            {
                this.presenter.HandleError(ex);
            }
        }

        private void ImageDisplayUserControl_OnInitialized(object sender, EventArgs e)
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
                        this.HeaderTextBlock.Text = this.GetDate();
                        this.HeaderTextBlock.Visibility = Visibility.Visible;
                    }
                    else if (borderType == BorderType.BorderFooter)
                    {
                        this.FooterTextBlock.Text = this.GetDate();
                        this.FooterTextBlock.Visibility = Visibility.Visible;
                    }
                }

                this.MainImage.MaxHeight = this.presenter.Configuration.MaximumSize;
                this.MainImage.MaxWidth = this.presenter.Configuration.MaximumSize;
                this.RotateImageFrame();
                this.LoadImage();
            }
            catch (Exception ex)
            {
                this.presenter.HandleError(ex);
            }
        }

        public string GetDate()
        {
            using (var fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                var md = (BitmapMetadata)img.Metadata;
                return !(md.DateTaken is null)
                    ? Convert.ToDateTime(md.DateTaken).ToShortDateString()
                    : File.GetLastWriteTime(this.filePath).ToShortDateString();
            }
        }

        private void RotateImageFrame()
        {
            int angle = this.presenter.GetRandomNumber(-maximumAngle, maximumAngle);
            var transform = new RotateTransform(angle);
            this.MainStackPanel.RenderTransform = transform;
        }

        private void LoadImage()
        {
            var processor = new ImageProcessor(this.filePath, this.presenter.Configuration);
            this.MainImage.Source = processor.GetImage();
        }
    }
}
