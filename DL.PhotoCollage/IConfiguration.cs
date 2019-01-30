namespace DL.PhotoCollage
{
    public interface IConfiguration
    {
        string Directory { get; set; }
        bool IsGrayscale { get; set; }
        bool IsRandom { get; set; }
        int MaximumSize { get; set; }
        int NumberOfPhotos { get; set; }
        BorderType PhotoBorderType { get; set; }
        //bool ShowPhotoDate { get; set; }
        ScreensaverSpeed Speed { get; set; }
        bool UseVerboseLogging { get; set; }
    }
}
