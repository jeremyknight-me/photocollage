using System.Windows.Controls;
using System.Windows.Input;

namespace PhotoCollageScreensaver.Views;

public partial class CollageWindow : Window, ICollageView
{
    private Point? initialMousePosition;

    public CollageWindow()
    {
        this.InitializeComponent();
    }

    public Canvas ImageCanvas => this.MainCanvas;
    public double WindowActualHeight => this.ActualHeight;
    public double WindowActualWidth => this.ActualWidth;
    public bool IsPortrait => this.ActualHeight > this.ActualWidth;

    private void Window_KeyDown(object sender, KeyEventArgs e) => ShutdownHelper.Shutdown();
    private void Window_MouseDown(object sender, MouseButtonEventArgs e) => ShutdownHelper.Shutdown();
    private void Window_TouchDown(object sender, TouchEventArgs e) => ShutdownHelper.Shutdown();

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        // Shut down application when mouse has moved significantly
        var position = e.GetPosition(this);

        if (!this.initialMousePosition.HasValue)
        {
            this.initialMousePosition = position;
        }
        else if (Math.Abs(this.initialMousePosition.Value.X - position.X) > 10
            || Math.Abs(this.initialMousePosition.Value.Y - position.Y) > 10)
        {
            ShutdownHelper.Shutdown();
        }
    }
}
