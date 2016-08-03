using System.Windows.Controls;

namespace DL.PhotoCollage.Presentation
{
    public interface ICollageView
    {
        Canvas ImageCanvas { get; }

        double WindowActualHeight { get; }

        double WindowActualWidth { get; }

        void Close();
    }
}
