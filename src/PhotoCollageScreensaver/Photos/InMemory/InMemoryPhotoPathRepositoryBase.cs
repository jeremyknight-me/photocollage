using System.Collections.Concurrent;

namespace PhotoCollageScreensaver.Photos.InMemory;

public abstract class InMemoryPhotoPathRepositoryBase : IPhotoPathRepository
{
    private readonly ISettingsRepository settingsRepo;

    protected InMemoryPhotoPathRepositoryBase(ISettingsRepository settingsRepository)
    {
        this.settingsRepo = settingsRepository;
    }

    public bool HasPhotos => !this.Paths.IsEmpty;

    protected ConcurrentBag<string> DisplayedPaths { get; } = [];
    protected ConcurrentQueue<string> Paths { get; } = [];

    public string GetNextPath()
    {
        string path, fullPath;
        do
        {
            path = this.GetNextPathIncludeReload();
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            fullPath = Path.Combine(this.settingsRepo.Current.Directory, path);
        } while (!File.Exists(fullPath));

        this.DisplayedPaths.Add(path);
        return fullPath;
    }

    public abstract void LoadPaths(IEnumerable<string> paths);

    protected void LoadPathsIntoQueue(IEnumerable<string> paths)
    {
        this.Paths.Clear();
        foreach (var path in paths)
        {
            this.Paths.Enqueue(path);
        }
    }

    private string GetNextPathIncludeReload()
    {
        if (this.Paths.TryDequeue(out var path))
        {
            return path;
        }

        this.LoadPaths(this.DisplayedPaths);
        this.DisplayedPaths.Clear();
        return this.Paths.IsEmpty
            ? string.Empty
            : this.GetNextPathIncludeReload();
    }
}
