using System.Windows.Controls;

namespace PhotoCollageScreensaver.Contracts
{
    public interface ICollageView
    {
        Canvas ImageCanvas { get; }
        double WindowActualHeight { get; }
        double WindowActualWidth { get; }
        void Close();
    }
}
