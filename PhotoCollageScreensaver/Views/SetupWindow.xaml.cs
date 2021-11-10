namespace PhotoCollageScreensaver.Views;

public partial class SetupWindow : Window
{
    private readonly ApplicationController controller;

    public SetupWindow(ApplicationController controllerToUse)
    {
        this.controller = controllerToUse;
        this.DataContext = this.controller.MakeSetupViewModel();
        this.InitializeComponent();
    }
}
