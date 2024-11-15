using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoCollageScreensaver.Collage.Imaging;

internal sealed class ImageProcessorFullscreen : ImageProcessor
{
    private int rotationAngle;

    private ImageProcessorFullscreen(string imagePathToUse, CollageSettings collageSettings)
        : base(imagePathToUse, collageSettings)
    {
    }

    public bool ImageIsRotatedPlusMinusNinetyDegrees { get; private set; }

    public static ImageProcessorFullscreen Create(string imagePathToUse, CollageSettings collageSettings)
        => new(imagePathToUse, collageSettings);

    public override ImageSource GetImageSource(ICollageView view, BitmapSource sourceImage = null)
    {
        sourceImage ??= this.GetBitmapImage();
        if (this.Configuration.RotateBasedOnEXIF && this.rotationAngle != 0)
        {
            var correctlyRotatedImage = this.GetRotateTransformedImage(sourceImage);
            return this.GetFullScreenScaledImage(correctlyRotatedImage, view);
        }

        return this.GetFullScreenScaledImage(sourceImage, view);
    }

    public BitmapSource GetImage()
    {
        var sourceImage = this.GetBitmapImage();
        if (this.Configuration.RotateBasedOnEXIF)
        {
            using var image = Image.FromFile(this.ImagePath);
            this.GetExifRotationData(image);
        }

        return sourceImage;
    }

    private void GetExifRotationData(Image image)
    {
        using var memoryStream = new MemoryStream();
        // Save the image to the memory stream in a suitable format (e.g., PNG, JPEG)
        image.Save(memoryStream, ImageFormat.Png); // You can change the format if needed

        if (image.PropertyIdList.Contains(0x0112)) // 0x0112 is the EXIF tag for orientation
        {
            var propertyItem = image.GetPropertyItem(0x0112);
            int rotationValue = BitConverter.ToUInt16(propertyItem.Value, 0);

            // EXIF rotation values (defined by the EXIF specification)
            this.rotationAngle = 0;
            this.ImageIsRotatedPlusMinusNinetyDegrees = false;
            switch (rotationValue)
            {
                case 3:
                    this.rotationAngle = 180;
                    break;
                case 6:
                    this.rotationAngle = 90;
                    this.ImageIsRotatedPlusMinusNinetyDegrees = true;
                    break;
                case 8:
                    this.rotationAngle = 270;
                    this.ImageIsRotatedPlusMinusNinetyDegrees = true;
                    break;
            }
        }
    }

    private TransformedBitmap GetRotateTransformedImage(BitmapSource original)
    {
        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new RotateTransform(this.rotationAngle);
        return new TransformedBitmap(original, transform);
    }

    private TransformedBitmap GetFullScreenScaledImage(BitmapSource original, ICollageView view)
    {
        var scaledHeight = view.WindowActualHeight / original.Height;
        var scaledWidth = view.WindowActualWidth / original.Width;
        if (this.Configuration.PhotoFullScreenMode == FullScreenMode.Centered)
        {
            scaledHeight = view.WindowActualHeight / original.Height;
            scaledWidth = view.WindowActualWidth / original.Width;
            scaledWidth = scaledHeight = scaledHeight > scaledWidth ? scaledWidth : scaledHeight;
        }

        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new ScaleTransform(scaledWidth, scaledHeight);
        return new TransformedBitmap(original, transform);
    }
}
