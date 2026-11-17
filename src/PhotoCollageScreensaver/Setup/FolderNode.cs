using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoCollageScreensaver.Setup;

public sealed class FolderNode : INotifyPropertyChanged
{
    private static readonly EnumerationOptions _subfolderOptions = new() { IgnoreInaccessible = true };

    private readonly FoldersViewModel _owner;
    private bool _isExpanded;
    private bool _childrenLoaded;

    // Stands in for unloaded subfolders so the tree shows an expander before they are read from disk.
    private FolderNode()
    {
    }

    public FolderNode(FoldersViewModel owner, string path, bool isRoot)
    {
        _owner = owner;
        Path = path;
        IsRoot = isRoot;
        Name = isRoot
            ? Directory.Exists(path) ? path : $"{path} (not found)"
            : System.IO.Path.GetFileName(path);

        if (HasSubfolders(path))
        {
            Children.Add(new FolderNode());
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Path { get; }
    public string Name { get; }
    public bool IsRoot { get; }
    public ObservableCollection<FolderNode> Children { get; } = [];

    public bool? IsChecked
    {
        get => _owner is null ? false : _owner.GetState(this);
        set => _owner?.Toggle(this);
    }

    public bool IsEnabled => _owner is not null && !_owner.IsAncestorExcluded(this);

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            if (value && !_childrenLoaded)
            {
                LoadChildren();
            }

            NotifyPropertyChanged();
        }
    }

    public void Refresh()
    {
        NotifyPropertyChanged(nameof(IsChecked));
        NotifyPropertyChanged(nameof(IsEnabled));
        if (_childrenLoaded)
        {
            foreach (FolderNode child in Children)
            {
                child.Refresh();
            }
        }
    }

    private void LoadChildren()
    {
        _childrenLoaded = true;
        Children.Clear();
        foreach (var subfolder in GetSubfolders(Path).Order(StringComparer.OrdinalIgnoreCase))
        {
            Children.Add(new FolderNode(_owner, subfolder, isRoot: false));
        }
    }

    private static bool HasSubfolders(string path) => GetSubfolders(path).Any();

    private static IEnumerable<string> GetSubfolders(string path)
    {
        try
        {
            return Directory.Exists(path)
                ? Directory.EnumerateDirectories(path, "*", _subfolderOptions).ToArray()
                : [];
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
