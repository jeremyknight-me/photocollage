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
        var length = _settingsRepo.Current.Directory.Length;
        var paths = Directory.EnumerateFiles(_settingsRepo.Current.Directory, "*.*", SearchOption.AllDirectories)
            .Where(f =>
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .Select(f => f[length..].TrimStart(['\\']))
            .ToArray();
        _photoPathRepo.LoadPaths(paths);
    }
}
