using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DL.PhotoCollage.Core
{
    public abstract class FileSystemPhotoRepositoryBase : IPhotoRepository
    {
        protected FileSystemPhotoRepositoryBase(string path)
        {
            this.PhotoFilePaths = new ConcurrentQueue<string>();
            this.RootDirectoryPath = path;
            this.LoadPathsFromFileSystem();
        }

        public bool HasPhotos
        {
            get { return !this.PhotoFilePaths.IsEmpty; }
        }

        public string NextPhotoFilePath
        {
            get { return this.GetNextPhotoFilePath(); }
        }

        protected ConcurrentQueue<string> PhotoFilePaths { get; private set; }

        protected string RootDirectoryPath { get; private set; }

        protected abstract string GetNextPhotoFilePath();

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
            var paths = new ConcurrentBag<string>();
            IEnumerable<string> photoFileExtensions = new List<string> { "*.jpg", "*.jpeg", "*.png" };

            Parallel.ForEach(
                photoFileExtensions,
                extension =>
                {
                    foreach (string path in Directory.EnumerateFiles(this.RootDirectoryPath, extension, SearchOption.AllDirectories))
                    {
                        string shortPath = path.Replace(this.RootDirectoryPath, string.Empty).TrimStart(new[] {'\\'});
                        paths.Add(shortPath);
                    }
                });

            var orderedPaths = this.GetOrderedPaths(paths);
            this.LoadPhotoPathsIntoQueue(orderedPaths);
        }
    }
}
