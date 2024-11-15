using PhotoCollage.Common.Settings;

namespace PhotoCollage.Common.Photos.InMemory;

public sealed class InMemoryOrderedPhotoPathRepository : InMemoryPhotoPathRepositoryBase
{
    public InMemoryOrderedPhotoPathRepository(ISettingsRepository settingsRepository)
        : base(settingsRepository)
    {
    }

    public override void LoadPaths(IEnumerable<string> paths)
        => this.LoadPathsIntoQueue(paths);
}
