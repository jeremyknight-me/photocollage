using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

internal interface IImageProcessor
{
    BitmapSource GetImage();

    public ImageSource GetScaledImage(ICollageView view, BitmapSource sourceImage=null);
}
