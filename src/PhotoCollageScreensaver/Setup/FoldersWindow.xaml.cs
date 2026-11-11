namespace PhotoCollageScreensaver.Setup;

public partial class FoldersWindow : Window
{
    private readonly FoldersViewModel _viewModel;

    public FoldersWindow(FoldersViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    private void FolderTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        => _viewModel.SelectedNode = e.NewValue as FolderNode;

    private void OkButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
}
