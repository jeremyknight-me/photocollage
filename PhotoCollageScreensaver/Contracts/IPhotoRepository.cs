namespace PhotoCollageScreensaver.Contracts
{
    internal interface IPhotoRepository
    {
        bool HasPhotos { get; }
        string GetNextPhotoFilePath();
    }
}
