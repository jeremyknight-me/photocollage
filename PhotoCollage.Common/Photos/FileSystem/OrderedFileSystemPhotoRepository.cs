using System.IO;
using PhotoCollage.Common.Settings;

namespace PhotoCollage.Common.Photos.FileSystem;

public sealed class OrderedFileSystemPhotoRepository : FileSystemPhotoRepositoryBase
{
    public OrderedFileSystemPhotoRepository(ISettingsRepository settingsRepository)
        : base(settingsRepository)
    {
    }

    public override string GetNextPhotoFilePath()
    {
        if (!this.PhotoFilePaths.TryDequeue(out var path))
        {
            this.PhotoFilePaths.TryDequeue(out path);
        }

        this.PhotoFilePaths.Enqueue(path);
        return Path.Combine(this.RootDirectoryPath, path);
    }

    protected override IEnumerable<string> GetOrderedPaths(IEnumerable<string> paths) => paths;
}
