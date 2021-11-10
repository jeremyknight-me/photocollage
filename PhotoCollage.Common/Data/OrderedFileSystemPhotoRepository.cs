using System.IO;

namespace PhotoCollage.Common.Data;

internal sealed class OrderedFileSystemPhotoRepository : FileSystemPhotoRepositoryBase
{
    public OrderedFileSystemPhotoRepository(string path)
        : base(path)
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
