using System.Windows.Controls;

namespace PhotoCollageScreensaver.Collage;

public interface ICollageView
{
    Canvas ImageCanvas { get; }
    double WindowActualHeight { get; }
    double WindowActualWidth { get; }
    bool IsPortrait { get; }
}
