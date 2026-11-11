namespace PhotoCollageScreensaver.Photos.InMemory;

public sealed class InMemoryOrderedPhotoPathRepository : InMemoryPhotoPathRepositoryBase
{
    public override void LoadPaths(IEnumerable<string> paths)
        => LoadPathsIntoQueue(paths);
}
