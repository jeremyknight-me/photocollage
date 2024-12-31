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
        var length = this.settingsRepo.Current.Directory.Length;
        var paths = Directory.EnumerateFiles(this.settingsRepo.Current.Directory, "*.*", SearchOption.AllDirectories)
            .Where(f =>
                f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .Select(f => f[length..].TrimStart(['\\']))
            .ToArray();
        this.photoPathRepo.LoadPaths(paths);
    }
}
