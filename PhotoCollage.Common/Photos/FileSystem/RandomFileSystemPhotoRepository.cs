using System.IO;
using System.Linq;
using PhotoCollage.Common.Settings;

namespace PhotoCollage.Common.Photos.FileSystem;

public sealed class RandomFileSystemPhotoRepository : FileSystemPhotoRepositoryBase
{
    private readonly List<string> displayedPhotos;
    private readonly object threadLock = new();

    public RandomFileSystemPhotoRepository(ISettingsRepository settingsRepository)
        : base(settingsRepository)
    {
        this.displayedPhotos = new List<string>();
    }

    public override string GetNextPhotoFilePath()
    {
        if (!this.PhotoFilePaths.TryDequeue(out var path))
        {
            this.ReloadPhotoQueue();
            this.PhotoFilePaths.TryDequeue(out path);
        }

        lock (this.threadLock)
        {
            this.displayedPhotos.Add(path);
        }

        return Path.Combine(this.RootDirectoryPath, path);
    }

    protected override IEnumerable<string> GetOrderedPaths(IEnumerable<string> paths)
        => RandomizePaths(paths);

    private static IEnumerable<string> RandomizePaths(IEnumerable<string> paths)
        => paths.OrderBy(item => Random.Shared.Next());

    private void ReloadPhotoQueue()
    {
        lock (this.threadLock)
        {
            var photosToQueue = RandomizePaths(this.displayedPhotos);
            this.LoadPhotoPathsIntoQueue(photosToQueue);
            this.displayedPhotos.Clear();
        }
    }
}
