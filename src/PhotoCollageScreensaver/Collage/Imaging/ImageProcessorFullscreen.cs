using System.Drawing;
using System.Drawing.Imaging;
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
        if (Configuration.RotateBasedOnEXIF && _rotationAngle != 0)
        {
            TransformedBitmap correctlyRotatedImage = GetRotateTransformedImage(sourceImage);
            return GetFullScreenScaledImage(correctlyRotatedImage, view);
        }

        return GetFullScreenScaledImage(sourceImage, view);
    }

    public BitmapSource GetImage()
    {
        BitmapSource sourceImage = GetBitmapImage();
        if (Configuration.RotateBasedOnEXIF)
        {
            using var image = Image.FromFile(ImagePath);
            GetExifRotationData(image);
        }

        return sourceImage;
    }

    private void GetExifRotationData(Image image)
    {
        const int exifOrientationTagId = 0x0112;
        if (image.PropertyIdList.Contains(exifOrientationTagId))
        {
            PropertyItem propertyItem = image.GetPropertyItem(exifOrientationTagId);
            int rotationValue = BitConverter.ToUInt16(propertyItem.Value, 0);

            // EXIF rotation values (defined by the EXIF specification)
            _rotationAngle = 0;
            ImageIsRotatedPlusMinusNinetyDegrees = false;
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
