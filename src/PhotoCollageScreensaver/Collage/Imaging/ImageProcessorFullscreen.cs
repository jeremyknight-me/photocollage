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
        GetExifRotationData(sourceImage);
        if (_rotationAngle != 0)
        {
            TransformedBitmap correctlyRotatedImage = GetRotateTransformedImage(sourceImage);
            return GetFullScreenScaledImage(correctlyRotatedImage, view);
        }

        return GetFullScreenScaledImage(sourceImage, view);
    }

    private void GetExifRotationData(BitmapSource sourceImage)
    {
        _rotationAngle = 0;
        ImageIsRotatedPlusMinusNinetyDegrees = false;

        if (!Configuration.RotateBasedOnEXIF
            || sourceImage.Metadata is not BitmapMetadata metadata
            || metadata.GetQuery("/app1/ifd/{ushort=274}") is not ushort rotationValue)
        {
            return;
        }

        switch (rotationValue)
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
