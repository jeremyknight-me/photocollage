namespace PhotoCollage.Common.Photos;

public interface IPhotoRepository
{
    bool HasPhotos { get; }
    string GetNextPhotoFilePath();
}
