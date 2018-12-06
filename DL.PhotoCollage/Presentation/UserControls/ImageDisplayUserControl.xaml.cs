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
                if (!this.presenter.Configuration.ShowPhotoBorder)
                {
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Transparent);
                    mySolidColorBrush.Opacity = 0;
                    this.ImageDate.Background = mySolidColorBrush;
                    this.InnerBorder.Background = mySolidColorBrush;
                }
                else {
                    if (this.presenter.Configuration.ShowPhotoDate)
                    {
                        this.ImageDate.Header = GetDate();
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
            string date = "";

            using (FileStream fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapSource img = BitmapFrame.Create(fs);
                BitmapMetadata md = (BitmapMetadata)img.Metadata;
                if (!(md.DateTaken is null))
                {
                    date = Convert.ToDateTime(md.DateTaken).ToShortDateString();
                }
                else
                {
                    date = File.GetLastWriteTime(this.filePath).ToShortDateString();
                }
            }
            return date;
        }

        private void RotateImageFrame()
        {
            int angle = this.presenter.GetRandomNumber(-maximumAngle, maximumAngle);
            var transform = new RotateTransform(angle);
            this.MainBorder.RenderTransform = transform;
        }

        private void LoadImage()
        {
            var processor = new ImageProcessor(this.filePath, this.presenter.Configuration);
            this.MainImage.Source = processor.GetImage();
        }
    }
}
