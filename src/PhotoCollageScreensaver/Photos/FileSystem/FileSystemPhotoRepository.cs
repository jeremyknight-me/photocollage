using System.Collections.Concurrent;

namespace PhotoCollageScreensaver.Photos.FileSystem;

public sealed class FileSystemPhotoRepository : IPhotoRepository
{
    private readonly IPhotoPathRepository photoPathRepo;
    private readonly ISettingsRepository settingsRepo;

    public FileSystemPhotoRepository(
        IPhotoPathRepository photoPathRepository,
        ISettingsRepository settingsRepository)
    {
        this.photoPathRepo = photoPathRepository;
        this.settingsRepo = settingsRepository;
    }

    public void LoadPhotoPaths()
    {
        var files = Directory.EnumerateFiles(this.settingsRepo.Current.Directory, "*", SearchOption.AllDirectories);
        var paths = this.GetPathsWithExtension(files);
        this.photoPathRepo.LoadPaths(paths);
    }

    private IEnumerable<string> GetPathsWithExtension(IEnumerable<string> files)
    {
        var extensions = new HashSet<string> { ".jpg", ".jpeg", ".png" };
        var length = this.settingsRepo.Current.Directory.Length;
        var paths = new ConcurrentQueue<string>();
        var exceptions = new ConcurrentQueue<Exception>();
        Parallel.ForEach(files, file =>
        {
            try
            {
                var fileExtension = Path.GetExtension(file);
                if (extensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    var path = file.Remove(0, length).TrimStart(new[] { '\\' });
                    paths.Enqueue(path);
                }
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(ex);
            }
        });

        return exceptions.IsEmpty
            ? paths
            : throw new AggregateException(exceptions);
    }
}
