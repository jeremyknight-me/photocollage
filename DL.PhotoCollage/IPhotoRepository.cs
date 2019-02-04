namespace DL.PhotoCollage
{
    public interface IPhotoRepository
    {
        bool HasPhotos { get; }

        string NextPhotoFilePath { get; }
    }
}
