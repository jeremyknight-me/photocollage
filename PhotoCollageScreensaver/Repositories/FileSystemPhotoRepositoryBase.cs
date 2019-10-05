using PhotoCollageScreensaver.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoCollageScreensaver.Repositories
{
    internal abstract class FileSystemPhotoRepositoryBase : IPhotoRepository
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
            var extensions = new HashSet<string> { ".jpg", ".jpeg", ".png" };
            var length = this.RootDirectoryPath.Length;
            var files = Directory.EnumerateFiles(this.RootDirectoryPath, "*", SearchOption.AllDirectories);
            var paths =
                from f in files
                where extensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
                select f.Substring(length).TrimStart(new[] { '\\' });
            var orderedPaths = this.GetOrderedPaths(paths);
            this.LoadPhotoPathsIntoQueue(orderedPaths);
        }
    }
}
