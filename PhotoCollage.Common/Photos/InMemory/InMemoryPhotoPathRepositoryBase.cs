using System.Collections.Concurrent;
using System.IO;
using PhotoCollage.Common.Settings;

namespace PhotoCollage.Common.Photos.InMemory;

public abstract class InMemoryPhotoPathRepositoryBase : IPhotoPathRepository
{
    private readonly ISettingsRepository settingsRepo;

    protected InMemoryPhotoPathRepositoryBase(ISettingsRepository settingsRepository)
    {
        this.settingsRepo = settingsRepository;
    }

    public bool HasPhotos => !this.Paths.IsEmpty;

    protected ConcurrentBag<string> DisplayedPaths { get; } = new();
    protected ConcurrentQueue<string> Paths { get; } = new();

    public string GetNextPath()
    {
        if (!this.Paths.TryDequeue(out var path))
        {
            this.LoadPaths(this.DisplayedPaths);
            this.DisplayedPaths.Clear();
            this.Paths.TryDequeue(out path);
        }

        this.DisplayedPaths.Add(path);
        return Path.Combine(this.settingsRepo.Current.Directory, path);
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
}
