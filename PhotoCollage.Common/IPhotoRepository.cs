namespace PhotoCollage.Common;

public interface IPhotoRepository
{
    bool HasPhotos
    {
        get;
    }
    string GetNextPhotoFilePath();
}
