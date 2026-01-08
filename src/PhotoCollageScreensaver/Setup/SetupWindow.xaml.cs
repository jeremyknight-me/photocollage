using PhotoCollageScreensaver.Setup;

namespace PhotoCollageScreensaver.Views;

public partial class SetupWindow : Window
{
    public SetupWindow(SetupViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
