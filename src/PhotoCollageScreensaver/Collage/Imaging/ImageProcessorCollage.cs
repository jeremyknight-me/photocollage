﻿using System.Windows.Media;
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
        sourceImage ??= this.GetBitmapImage();
        return this.DoesImageNeedScaling(sourceImage.Height, sourceImage.Width)
            ? this.GetScaledTransformedImage(sourceImage)
            : sourceImage;
    }

    private bool DoesImageNeedScaling(double height, double width)
        => height > this.MaximumSizeDiu
            || width > this.MaximumSizeDiu
            || this.DpiScale > 1;

    private TransformedBitmap GetScaledTransformedImage(BitmapSource original)
    {
        var scale = original.Height > original.Width
            ? this.GetScale(original.Height)
            : this.GetScale(original.Width);
        RenderOptions.SetBitmapScalingMode(original, BitmapScalingMode.HighQuality);
        var transform = new ScaleTransform(scale, scale);
        return new TransformedBitmap(original, transform);
    }

    private double GetScale(double value) => this.MaximumSizeDiu / value * this.DpiScale;
}
