using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using PhotoCollageScreensaver.Commands;

namespace PhotoCollageScreensaver.Setup;

public sealed class FoldersViewModel
{
    private readonly HashSet<string> _excluded;

    public FoldersViewModel(IEnumerable<string> directories, IEnumerable<string> excludedDirectories)
    {
        _excluded = new HashSet<string>(excludedDirectories.Select(NormalizePath), StringComparer.OrdinalIgnoreCase);
        foreach (var directory in directories)
        {
            AddRoot(directory);
        }

        AddFolderCommand = new RelayCommand(obj => RequestFolderFromUser());
        RemoveFolderCommand = new RelayCommand(obj => RemoveSelectedRoot(), obj => SelectedNode?.IsRoot == true);
    }

    public ObservableCollection<FolderNode> Roots { get; } = [];
    public FolderNode SelectedNode { get; set; }

    public ICommand AddFolderCommand { get; }
    public ICommand RemoveFolderCommand { get; }

    // Roots left fully unticked are dropped, as are exclusions that no longer sit inside a kept root.
    public List<string> GetDirectories()
        => Roots.Where(root => GetState(root) != false).Select(root => root.Path).ToList();

    public List<string> GetExcludedDirectories()
    {
        var directories = GetDirectories();
        return _excluded
            .Where(excluded => directories.Any(directory => IsUnder(excluded, directory)))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    // true: shown; false: excluded, itself or via a parent; null: shown, but some subfolder is excluded.
    public bool? GetState(FolderNode node)
    {
        if (_excluded.Contains(node.Path) || IsAncestorExcluded(node))
        {
            return false;
        }

        return _excluded.Any(excluded => IsUnder(excluded, node.Path)) ? null : true;
    }

    public bool IsAncestorExcluded(FolderNode node) => _excluded.Any(excluded => IsUnder(node.Path, excluded));

    public void Toggle(FolderNode node)
    {
        if (GetState(node) == false)
        {
            _excluded.Remove(node.Path);
        }
        else
        {
            _excluded.RemoveWhere(excluded => IsUnder(excluded, node.Path));
            _excluded.Add(node.Path);
        }

        foreach (FolderNode root in Roots)
        {
            root.Refresh();
        }
    }

    private void AddRoot(string directory)
    {
        var path = NormalizePath(directory);
        if (!Roots.Any(root => string.Equals(root.Path, path, StringComparison.OrdinalIgnoreCase)))
        {
            Roots.Add(new FolderNode(this, path, isRoot: true));
        }
    }

    private void RemoveSelectedRoot()
    {
        FolderNode root = SelectedNode;
        Roots.Remove(root);
        _excluded.RemoveWhere(excluded => string.Equals(excluded, root.Path, StringComparison.OrdinalIgnoreCase) || IsUnder(excluded, root.Path));
        SelectedNode = null;
    }

    private void RequestFolderFromUser()
    {
        var dialog = new FolderBrowserDialog
        {
            ShowNewFolderButton = false
        };
        if (SelectedNode is not null)
        {
            dialog.SelectedPath = SelectedNode.Path;
        }

        if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
        {
            AddRoot(dialog.SelectedPath);
        }
    }

    private static bool IsUnder(string path, string ancestor)
    {
        var prefix = Path.EndsInDirectorySeparator(ancestor) ? ancestor : ancestor + Path.DirectorySeparatorChar;
        return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string path) => Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
}
