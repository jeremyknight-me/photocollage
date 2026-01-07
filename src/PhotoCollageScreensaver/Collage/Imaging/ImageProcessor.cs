using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal abstract class ImageProcessor
{
    public ImageProcessor(string imagePathToUse, CollageSettings collageSettings)
    {
        Configuration = collageSettings;
        ImagePath = imagePathToUse;
    }

    protected CollageSettings Configuration { get; }
    protected double DpiScale { get; set; }
    protected string ImagePath { get; }
    protected double MaximumSizeDiu { get; set; }

    public abstract ImageSource GetImageSource(ICollageView view, BitmapSource sourceImage = null);

    protected BitmapSource GetBitmapImage()
    {
        BitmapSource rawImage = GetRawImage();
        BitmapSource sourceImage = Configuration.IsGrayscale
            ? GetGrayscaleImage(rawImage)
            : rawImage;

        DpiScale = sourceImage.DpiX / 96;
        MaximumSizeDiu = Configuration.MaximumSize / DpiScale; // default
        return sourceImage;
    }

    private BitmapImage GetRawImage()
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.None;
        image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
        image.UriSource = new Uri(ImagePath, UriKind.Absolute);
        image.EndInit();

        return image;
    }

    protected static FormatConvertedBitmap GetGrayscaleImage(BitmapSource source)
    {
        var image = new FormatConvertedBitmap();
        image.BeginInit();
        image.Source = source;
        image.DestinationFormat = PixelFormats.Gray32Float;
        image.EndInit();
        return image;
    }
}
