using System.Collections.Concurrent;

namespace PhotoCollageScreensaver.Photos.InMemory;

public abstract class InMemoryPhotoPathRepositoryBase : IPhotoPathRepository
{
    private readonly ISettingsRepository _settingsRepo;

    protected InMemoryPhotoPathRepositoryBase(ISettingsRepository settingsRepository)
    {
        _settingsRepo = settingsRepository;
    }

    public bool HasPhotos => !Paths.IsEmpty;

    protected ConcurrentBag<string> DisplayedPaths { get; } = [];
    protected ConcurrentQueue<string> Paths { get; } = [];

    public string GetNextPath()
    {
        string path, fullPath;
        do
        {
            path = GetNextPathIncludeReload();
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            fullPath = Path.Combine(_settingsRepo.Current.Directory, path);
        } while (!File.Exists(fullPath));

        DisplayedPaths.Add(path);
        return fullPath;
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
