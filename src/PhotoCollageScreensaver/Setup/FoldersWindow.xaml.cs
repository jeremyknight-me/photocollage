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

    // An empty list would load back as My Pictures, so make the user keep at least one folder.
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.GetDirectories().Count == 0)
        {
            _ = MessageBox.Show(this, "Tick at least one folder to show photos from.", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }
}
