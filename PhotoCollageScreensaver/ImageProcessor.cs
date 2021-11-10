using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhotoCollage.Common;

namespace PhotoCollageScreensaver;

internal class ImageProcessor
{
    private readonly string imagePath;
    private readonly CollageSettings configuration;
    private double dpiScale;
    private double maximumSizeDiu;

    public ImageProcessor(string imagePathToUse, CollageSettings configurationToUse)
    {
        this.imagePath = imagePathToUse;
        this.configuration = configurationToUse;
    }

    public ImageSource GetImage()
    {
        BitmapSource rawImage = this.GetRawImage();
        var sourceImage = this.configuration.IsGrayscale
            ? this.GetGrayscaleImage(rawImage)
            : rawImage;

        this.dpiScale = sourceImage.DpiX / 96;
        this.maximumSizeDiu = this.configuration.MaximumSize / this.dpiScale;

        return this.DoesImageNeedScaling(sourceImage.Height, sourceImage.Width)
            ? this.GetScaledImage(sourceImage)
            : sourceImage;
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
        => height > this.maximumSizeDiu
            || width > this.maximumSizeDiu
            || this.dpiScale > 1;

    private TransformedBitmap GetScaledImage(BitmapSource original)
    {
        var scale = original.Height > original.Width
                ? this.GetScale(original.Height)
                : this.GetScale(original.Width);
        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new ScaleTransform(scale, scale);
        return new TransformedBitmap(original, transform);
    }

    private double GetScale(double value) => this.maximumSizeDiu / value * this.dpiScale;
}
