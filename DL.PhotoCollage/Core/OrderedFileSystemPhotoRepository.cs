using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DL.PhotoCollage.Core
{
    public sealed class OrderedFileSystemPhotoRepository : FileSystemPhotoRepositoryBase
    {
        public OrderedFileSystemPhotoRepository(string path)
            : base(path)
        {
        }

        protected override string GetNextPhotoFilePath()
        {
            string path;

            if (!this.PhotoFilePaths.TryDequeue(out path))
            {
                this.PhotoFilePaths.TryDequeue(out path);
            }

            this.PhotoFilePaths.Enqueue(path);
            return Path.Combine(this.RootDirectoryPath, path);
        }

        protected override IEnumerable<string> GetOrderedPaths(IEnumerable<string> paths)
        {
            return paths.OrderBy(x => x);
        }
    }
}
