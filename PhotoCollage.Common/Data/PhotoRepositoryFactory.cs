namespace PhotoCollage.Common.Data;

public sealed class PhotoRepositoryFactory
{
    private readonly CollageSettings configuration;

    public PhotoRepositoryFactory(CollageSettings configurationToUse)
    {
        this.configuration = configurationToUse;
    }

    public IPhotoRepository Make() => this.configuration.IsRandom
            ? new RandomFileSystemPhotoRepository(this.configuration.Directory)
            : new OrderedFileSystemPhotoRepository(this.configuration.Directory);
}
