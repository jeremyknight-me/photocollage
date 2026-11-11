using System.IO.Enumeration;

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
        CollageSettings settings = _settingsRepo.Current;
        var directories = settings.Directories
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Select(NormalizePath)
            .Where(Directory.Exists)
            .ToArray();
        if (directories.Length == 0)
        {
            throw new DirectoryNotFoundException($"None of the photo directories exist or can be accessed: {string.Join(", ", settings.Directories)}");
        }

        HashSet<string> excluded = new(settings.ExcludedDirectories.Select(NormalizePath), StringComparer.OrdinalIgnoreCase);
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase); // overlapping folders must not show a photo twice
        var paths = directories
            .SelectMany(directory => EnumeratePhotos(directory, excluded))
            .Where(seen.Add)
            .ToArray();
        _photoPathRepo.LoadPaths(paths);
    }

    private static IEnumerable<string> EnumeratePhotos(string directory, HashSet<string> excluded)
        => new FileSystemEnumerable<string>(
            directory,
            (ref FileSystemEntry entry) => entry.ToFullPath(),
            new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            })
        {
            ShouldIncludePredicate = (ref FileSystemEntry entry) => !entry.IsDirectory && IsPhoto(entry.FileName),
            ShouldRecursePredicate = (ref FileSystemEntry entry) => !excluded.Contains(entry.ToFullPath())
        };

    private static bool IsPhoto(ReadOnlySpan<char> fileName)
    {
        ReadOnlySpan<char> extension = Path.GetExtension(fileName);
        return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".png", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string path) => Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
}
