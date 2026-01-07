using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal sealed class ImageProcessorCollage : ImageProcessor
{
    public ImageProcessorCollage(string imagePathToUse, CollageSettings collageSettings)
        : base(imagePathToUse, collageSettings)
    {
    }

    public override ImageSource GetImageSource(ICollageView view, BitmapSource sourceImage = null)
    {
        sourceImage ??= GetBitmapImage();
        return DoesImageNeedScaling(sourceImage.Height, sourceImage.Width)
            ? GetScaledTransformedImage(sourceImage)
            : sourceImage;
    }

    private bool DoesImageNeedScaling(double height, double width)
        => height > MaximumSizeDiu
            || width > MaximumSizeDiu
            || DpiScale > 1;

    private TransformedBitmap GetScaledTransformedImage(BitmapSource original)
    {
        var scale = original.Height > original.Width
            ? GetScale(original.Height)
            : GetScale(original.Width);
        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new ScaleTransform(scale, scale);
        return new TransformedBitmap(original, transform);
    }

    private double GetScale(double value) => MaximumSizeDiu / value * DpiScale;
}
