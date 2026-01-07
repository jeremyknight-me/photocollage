using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoCollageScreensaver.Collage;

public partial class CollageWindow : Window, ICollageView
{
    private Point? _initialMousePosition;

    public CollageWindow()
    {
        InitializeComponent();
    }

    public Canvas ImageCanvas => MainCanvas;
    public double WindowActualHeight => ActualHeight;
    public double WindowActualWidth => ActualWidth;
    public bool IsPortrait => ActualHeight > ActualWidth;

    private void Window_KeyDown(object sender, KeyEventArgs e) => ShutdownHelper.Shutdown();
    private void Window_MouseDown(object sender, MouseButtonEventArgs e) => ShutdownHelper.Shutdown();
    private void Window_TouchDown(object sender, TouchEventArgs e) => ShutdownHelper.Shutdown();

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        // Shut down application when mouse has moved significantly
        Point position = e.GetPosition(this);

        if (!_initialMousePosition.HasValue)
        {
            _initialMousePosition = position;
        }
        else if (Math.Abs(_initialMousePosition.Value.X - position.X) > 10
            || Math.Abs(_initialMousePosition.Value.Y - position.Y) > 10)
        {
            ShutdownHelper.Shutdown();
        }
    }
}
