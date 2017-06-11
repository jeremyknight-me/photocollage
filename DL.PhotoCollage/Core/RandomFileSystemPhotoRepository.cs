using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DL.PhotoCollage.Core
{
    public sealed class RandomFileSystemPhotoRepository : FileSystemPhotoRepositoryBase
    {
        private readonly List<string> displayedPhotos;

        private readonly object threadLock = new object();

        public RandomFileSystemPhotoRepository(string path)
            : base(path)
        {
            this.displayedPhotos = new List<string>();
        }

        protected override string GetNextPhotoFilePath()
        {
            string path;

            if (!this.PhotoFilePaths.TryDequeue(out path))
            {
                this.ReloadPhotoQueue();
                this.PhotoFilePaths.TryDequeue(out path);
            }

            lock (this.threadLock)
            {
                this.displayedPhotos.Add(path);
            }

            return Path.Combine(this.RootDirectoryPath, path);
        }

        protected override IEnumerable<string> GetOrderedPaths(IEnumerable<string> paths)
        {
            return RandomizePaths(paths);
        }

        private static IEnumerable<string> RandomizePaths(IEnumerable<string> paths)
        {
            var random = new Random();
            return paths.OrderBy(item => random.Next());
        }

        private void ReloadPhotoQueue()
        {
            lock (this.threadLock)
            {
                IEnumerable<string> photosToQueue = RandomizePaths(this.displayedPhotos);
                this.LoadPhotoPathsIntoQueue(photosToQueue);
                this.displayedPhotos.Clear();
            }
        }
    }
}
