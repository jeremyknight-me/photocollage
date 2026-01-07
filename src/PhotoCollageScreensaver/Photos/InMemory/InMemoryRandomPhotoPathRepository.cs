namespace PhotoCollageScreensaver.Photos.InMemory;

public sealed class InMemoryRandomPhotoPathRepository : InMemoryPhotoPathRepositoryBase
{
    public InMemoryRandomPhotoPathRepository(ISettingsRepository settingsRepository)
        : base(settingsRepository)
    {
    }

    public override void LoadPaths(IEnumerable<string> paths)
    {
        IOrderedEnumerable<string> randomizedPaths = paths.OrderBy(item => Random.Shared.Next());
        LoadPathsIntoQueue(randomizedPaths);
    }
}
