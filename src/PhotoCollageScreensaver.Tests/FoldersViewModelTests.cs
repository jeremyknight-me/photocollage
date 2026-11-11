using PhotoCollageScreensaver.Setup;

namespace PhotoCollageScreensaver.Tests;

public sealed class FoldersViewModelTests : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly string _photos;
    private readonly string _holidays;

    public FoldersViewModelTests()
    {
        _photos = _temp.CreateDirectory("Photos");
        _temp.CreateDirectory(@"Photos\2024");
        _temp.CreateDirectory(@"Photos\Screenshots\Old");
        _holidays = _temp.CreateDirectory("Holidays");
    }

    public void Dispose() => _temp.Dispose();

    [Fact]
    public void Roots_FolderWithExcludedSubfolder_IsPartiallyChecked()
    {
        var viewModel = new FoldersViewModel([_photos], [_temp.Combine(@"Photos\Screenshots")]);

        Assert.Null(viewModel.Roots[0].IsChecked);
    }

    [Fact]
    public void IsExpanded_True_LoadsSubfoldersSorted()
    {
        var viewModel = new FoldersViewModel([_photos], []);

        FolderNode root = viewModel.Roots[0];
        root.IsExpanded = true;

        Assert.Equal(["2024", "Screenshots"], root.Children.Select(c => c.Name));
    }

    [Fact]
    public void IsChecked_UntickSubfolder_ExcludesIt()
    {
        var viewModel = new FoldersViewModel([_photos], []);
        FolderNode screenshots = Expand(viewModel.Roots[0], "Screenshots");

        screenshots.IsChecked = false;

        Assert.False(screenshots.IsChecked);
        Assert.Equal([_temp.Combine(@"Photos\Screenshots")], viewModel.GetExcludedDirectories());
    }

    [Fact]
    public void IsChecked_RetickExcludedSubfolder_IncludesItAgain()
    {
        var viewModel = new FoldersViewModel([_photos], [_temp.Combine(@"Photos\Screenshots")]);
        FolderNode screenshots = Expand(viewModel.Roots[0], "Screenshots");

        screenshots.IsChecked = true;

        Assert.True(screenshots.IsChecked);
        Assert.Empty(viewModel.GetExcludedDirectories());
    }

    [Fact]
    public void IsEnabled_FolderInsideExcludedFolder_IsUncheckedAndDisabled()
    {
        var viewModel = new FoldersViewModel([_photos], [_temp.Combine(@"Photos\Screenshots")]);
        FolderNode old = Expand(Expand(viewModel.Roots[0], "Screenshots"), "Old");

        Assert.False(old.IsChecked);
        Assert.False(old.IsEnabled);
    }

    [Fact]
    public void IsChecked_UntickParentOfExclusion_ReplacesTheNarrowerExclusion()
    {
        var viewModel = new FoldersViewModel([_photos], [_temp.Combine(@"Photos\Screenshots\Old")]);
        FolderNode screenshots = Expand(viewModel.Roots[0], "Screenshots");

        screenshots.IsChecked = false;

        Assert.Equal([_temp.Combine(@"Photos\Screenshots")], viewModel.GetExcludedDirectories());
    }

    [Fact]
    public void GetDirectories_RootUnticked_DropsRootAndItsExclusions()
    {
        var viewModel = new FoldersViewModel([_photos, _holidays], [_temp.Combine(@"Photos\Screenshots")]);

        viewModel.Roots[0].IsChecked = false;

        Assert.Equal([_holidays], viewModel.GetDirectories());
        Assert.Empty(viewModel.GetExcludedDirectories());
    }

    [Fact]
    public void RemoveFolderCommand_SelectedRoot_RemovesItAndItsExclusions()
    {
        var viewModel = new FoldersViewModel([_photos, _holidays], [_temp.Combine(@"Photos\Screenshots")]);
        viewModel.SelectedNode = viewModel.Roots[0];

        viewModel.RemoveFolderCommand.Execute(null);

        Assert.Equal([_holidays], viewModel.GetDirectories());
        Assert.Empty(viewModel.GetExcludedDirectories());
    }

    [Fact]
    public void RemoveFolderCommand_SubfolderSelected_CannotExecute()
    {
        var viewModel = new FoldersViewModel([_photos], []);
        viewModel.SelectedNode = Expand(viewModel.Roots[0], "2024");

        Assert.False(viewModel.RemoveFolderCommand.CanExecute(null));
    }

    [Fact]
    public void Roots_MissingFolder_IsLabelledNotFound()
    {
        var viewModel = new FoldersViewModel([_temp.Combine("Unplugged")], []);

        Assert.EndsWith("(not found)", viewModel.Roots[0].Name);
        Assert.Empty(viewModel.Roots[0].Children);
    }

    private static FolderNode Expand(FolderNode node, string childName)
    {
        node.IsExpanded = true;
        return node.Children.Single(c => c.Name == childName);
    }
}
