namespace DL.PhotoCollage.Core
{
    public interface IPhotoRepository
    {
        bool HasPhotos { get; }

        string NextPhotoFilePath { get; }
    }
}
