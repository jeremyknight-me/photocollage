namespace PhotoCollageScreensaver.Photos.InMemory;

public sealed class InMemoryOrderedPhotoPathRepository : InMemoryPhotoPathRepositoryBase
{
    public InMemoryOrderedPhotoPathRepository(ISettingsRepository settingsRepository)
        : base(settingsRepository)
    {
    }

    public override void LoadPaths(IEnumerable<string> paths)
        => LoadPathsIntoQueue(paths);
}
