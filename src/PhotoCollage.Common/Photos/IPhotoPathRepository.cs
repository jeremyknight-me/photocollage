namespace PhotoCollage.Common.Photos;

public interface IPhotoPathRepository
{
    bool HasPhotos { get; }
    string GetNextPath();
    void LoadPaths(IEnumerable<string> paths);
}
