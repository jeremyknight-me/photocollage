using System.Windows.Controls;

namespace PhotoCollageScreensaver.Views;

public interface ICollageView
{
    Canvas ImageCanvas { get; }
    double WindowActualHeight { get; }
    double WindowActualWidth { get; }
}
