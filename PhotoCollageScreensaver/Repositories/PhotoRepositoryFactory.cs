using PhotoCollageScreensaver.Contracts;

namespace PhotoCollageScreensaver.Repositories
{
    internal sealed class PhotoRepositoryFactory
    {
        private readonly Configuration configuration;

        public PhotoRepositoryFactory(Configuration configurationToUse)
        {
            this.configuration = configurationToUse;
        }

        public IPhotoRepository Make()
        {
            if (this.configuration.IsRandom)
            {
                return new RandomFileSystemPhotoRepository(this.configuration.Directory);
            }

            return new OrderedFileSystemPhotoRepository(this.configuration.Directory);
        }
    }
}
