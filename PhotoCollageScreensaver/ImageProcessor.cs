using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhotoCollage.Common;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

internal abstract class ImageProcessor : IImageProcessor
{
    protected readonly string ImagePath;
    protected readonly CollageSettings Configuration;
    protected double DpiScale;
    protected double MaximumSizeDiu;

    public ImageProcessor(string imagePathToUse, CollageSettings configurationToUse)
    {
        this.ImagePath = imagePathToUse;
        this.Configuration = configurationToUse;
    }

    public BitmapSource GetImage()
    {
        BitmapSource rawImage = this.GetRawImage();
        var sourceImage = this.Configuration.IsGrayscale
            ? this.GetGrayscaleImage(rawImage)
            : rawImage;

        this.DpiScale = sourceImage.DpiX / 96;
        this.MaximumSizeDiu = this.Configuration.MaximumSize / this.DpiScale; // default
        return sourceImage;
    }

    public virtual ImageSource GetScaledImage(ICollageView view, BitmapSource sourceImage = null) => this.GetImage();

    private BitmapImage GetRawImage()
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.None;
        image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
        image.UriSource = new Uri(this.ImagePath, UriKind.Absolute);
        image.EndInit();

        return image;
    }

    protected FormatConvertedBitmap GetGrayscaleImage(BitmapSource source)
    {
        var image = new FormatConvertedBitmap();
        image.BeginInit();
        image.Source = source;
        image.DestinationFormat = PixelFormats.Gray32Float;
        image.EndInit();
        return image;
    }
}
