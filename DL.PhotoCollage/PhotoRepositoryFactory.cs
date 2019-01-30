namespace DL.PhotoCollage
{
    public class PhotoRepositoryFactory
    {
        private readonly IConfiguration configuration;

        public PhotoRepositoryFactory(IConfiguration configurationToUse)
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
