using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DL.PhotoCollage.Presentation
{
    internal class ImageProcessor
    {
        private readonly string imagePath;

        private readonly ScreensaverConfiguration configuration;

        private double dpiScale;

        private double maximumSizeDiu;

        public ImageProcessor(string imagePathToUse, ScreensaverConfiguration configurationToUse)
        {
            this.imagePath = imagePathToUse;
            this.configuration = configurationToUse;
        }

        public ImageSource GetImage()
        {
            BitmapSource rawImage = this.GetRawImage();
            BitmapSource sourceImage = this.configuration.IsGrayscale
                ? this.GetGrayscaleImage(rawImage)
                : rawImage;

            this.dpiScale = sourceImage.DpiX / 96;
            this.maximumSizeDiu = this.configuration.MaximumSize / this.dpiScale;

            if (this.DoesImageNeedScaling(sourceImage.Height, sourceImage.Width))
            {
                double scale = sourceImage.Height > sourceImage.Width
                    ? this.GetPortraitScale(sourceImage.Height)
                    : this.GetLandscapeScale(sourceImage.Width);
                return this.GetScaledImage(sourceImage, scale);
            }

            return sourceImage;
        }

        private BitmapImage GetRawImage()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.None;
            image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(this.imagePath, UriKind.Absolute);
            image.EndInit();
            return image;
        }

        private FormatConvertedBitmap GetGrayscaleImage(BitmapSource source)
        {
            var image = new FormatConvertedBitmap();
            image.BeginInit();
            image.Source = source;
            image.DestinationFormat = PixelFormats.Gray32Float;
            image.EndInit();
            return image;
        }

        private bool DoesImageNeedScaling(double height, double width)
        {
            return height > this.maximumSizeDiu
                || width > this.maximumSizeDiu
                || this.dpiScale > 1;
        }

        private TransformedBitmap GetScaledImage(BitmapSource original, double scale)
        {
            RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
            var transform = new ScaleTransform(scale, scale);
            return new TransformedBitmap(original, transform);
        }

        private double GetLandscapeScale(double width)
        {
            return (this.maximumSizeDiu / width) * this.dpiScale;
        }

        private double GetPortraitScale(double height)
        {
            return (this.maximumSizeDiu / height) * this.dpiScale;
        }
    }
}
