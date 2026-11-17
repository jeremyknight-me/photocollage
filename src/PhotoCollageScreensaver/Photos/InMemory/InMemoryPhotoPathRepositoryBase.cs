using System.Collections.Concurrent;

namespace PhotoCollageScreensaver.Photos.InMemory;

public abstract class InMemoryPhotoPathRepositoryBase : IPhotoPathRepository
{
    public bool HasPhotos => !Paths.IsEmpty;

    protected ConcurrentBag<string> DisplayedPaths { get; } = [];
    protected ConcurrentQueue<string> Paths { get; } = [];

    public string GetNextPath()
    {
        string path;
        do
        {
            path = GetNextPathIncludeReload();
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
        } while (!File.Exists(path));

        DisplayedPaths.Add(path);
        return path;
    }

    public abstract void LoadPaths(IEnumerable<string> paths);

    protected void LoadPathsIntoQueue(IEnumerable<string> paths)
    {
        Paths.Clear();
        foreach (var path in paths)
        {
            Paths.Enqueue(path);
        }
    }

    private string GetNextPathIncludeReload()
    {
        if (Paths.TryDequeue(out var path))
        {
            return path;
        }

        LoadPaths(DisplayedPaths);
        DisplayedPaths.Clear();
        return Paths.IsEmpty
            ? string.Empty
            : GetNextPathIncludeReload();
    }
}
