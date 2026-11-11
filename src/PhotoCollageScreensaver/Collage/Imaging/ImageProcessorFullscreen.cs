using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal sealed class ImageProcessorFullscreen : ImageProcessor
{
    private int _rotationAngle;

    private ImageProcessorFullscreen(string imagePathToUse, CollageSettings collageSettings)
        : base(imagePathToUse, collageSettings)
    {
    }

    public bool ImageIsRotatedPlusMinusNinetyDegrees { get; private set; }

    public static ImageProcessorFullscreen Create(string imagePathToUse, CollageSettings collageSettings)
        => new(imagePathToUse, collageSettings);

    public override ImageSource GetImageSource(ICollageView view, BitmapSource sourceImage = null)
    {
        sourceImage ??= GetBitmapImage();
        GetExifRotationData();
        if (_rotationAngle != 0)
        {
            TransformedBitmap correctlyRotatedImage = GetRotateTransformedImage(sourceImage);
            return GetFullScreenScaledImage(correctlyRotatedImage, view);
        }

        return GetFullScreenScaledImage(sourceImage, view);
    }

    private void GetExifRotationData()
    {
        _rotationAngle = 0;
        ImageIsRotatedPlusMinusNinetyDegrees = false;

        if (!Configuration.RotateBasedOnEXIF)
        {
            return;
        }

        switch (ReadExifOrientation())
        {
            case 3:
                _rotationAngle = 180;
                break;
            case 6:
                _rotationAngle = 90;
                ImageIsRotatedPlusMinusNinetyDegrees = true;
                break;
            case 8:
                _rotationAngle = 270;
                ImageIsRotatedPlusMinusNinetyDegrees = true;
                break;
        }
    }

    // BitmapImage throws when asked for metadata, so read the orientation from a frame decoded straight from the file.
    private ushort ReadExifOrientation()
    {
        try
        {
            using FileStream stream = new(ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapCreateOptions options = BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache;
            BitmapFrame frame = BitmapFrame.Create(stream, options, BitmapCacheOption.None);
            return frame.Metadata is BitmapMetadata metadata && metadata.GetQuery("System.Photo.Orientation") is ushort orientation
                ? orientation
                : (ushort)0;
        }
        catch (Exception exception) when (exception is NotSupportedException or ArgumentException or IOException)
        {
            return 0;
        }
    }

    private TransformedBitmap GetRotateTransformedImage(BitmapSource original)
    {
        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new RotateTransform(_rotationAngle);
        return new TransformedBitmap(original, transform);
    }

    private TransformedBitmap GetFullScreenScaledImage(BitmapSource original, ICollageView view)
    {
        var scaledHeight = view.WindowActualHeight / original.Height;
        var scaledWidth = view.WindowActualWidth / original.Width;

        // center image
        scaledWidth = scaledHeight = scaledHeight > scaledWidth ? scaledWidth : scaledHeight;

        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new ScaleTransform(scaledWidth, scaledHeight);
        return new TransformedBitmap(original, transform);
    }
}
