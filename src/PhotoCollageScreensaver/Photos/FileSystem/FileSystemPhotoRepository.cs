namespace PhotoCollageScreensaver.Photos.FileSystem;

public sealed class FileSystemPhotoRepository : IPhotoRepository
{
    private readonly IPhotoPathRepository _photoPathRepo;
    private readonly ISettingsRepository _settingsRepo;

    public FileSystemPhotoRepository(
        IPhotoPathRepository photoPathRepository,
        ISettingsRepository settingsRepository)
    {
        _photoPathRepo = photoPathRepository;
        _settingsRepo = settingsRepository;
    }

    public void LoadPhotoPaths()
    {
        var directory = _settingsRepo.Current.Directory;
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"The photo directory '{directory}' does not exist or cannot be accessed.");
        }

        var paths = Directory.EnumerateFiles(
            directory,
            "*.*",
            new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            })
            .Where(f =>
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.GetRelativePath(directory, f))
            .ToArray();
        _photoPathRepo.LoadPaths(paths);
    }
}
