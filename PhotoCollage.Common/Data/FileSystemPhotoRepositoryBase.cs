using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCollage.Common.Data
{
    internal abstract class FileSystemPhotoRepositoryBase : IPhotoRepository
    {
        protected FileSystemPhotoRepositoryBase(string path)
        {
            this.PhotoFilePaths = new ConcurrentQueue<string>();
            this.RootDirectoryPath = path;
            this.LoadPathsFromFileSystem();
        }

        public bool HasPhotos => !this.PhotoFilePaths.IsEmpty;

        public abstract string GetNextPhotoFilePath();

        protected ConcurrentQueue<string> PhotoFilePaths { get; private set; }

        protected string RootDirectoryPath { get; private set; }

        protected abstract IEnumerable<string> GetOrderedPaths(IEnumerable<string> paths);

        protected void LoadPhotoPathsIntoQueue(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                this.PhotoFilePaths.Enqueue(path);
            }
        }

        private void LoadPathsFromFileSystem()
        {
            var files = Directory.EnumerateFiles(this.RootDirectoryPath, "*", SearchOption.AllDirectories);
            var paths = this.GetPathsWithExtension(files);
            var orderedPaths = this.GetOrderedPaths(paths);
            this.LoadPhotoPathsIntoQueue(orderedPaths);
        }

        private IEnumerable<string> GetPathsWithExtension(IEnumerable<string> files)
        {
            var extensions = new HashSet<string> { ".jpg", ".jpeg", ".png" };
            var length = this.RootDirectoryPath.Length;
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
}
