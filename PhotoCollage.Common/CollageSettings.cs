using PhotoCollage.Common.Enums;

namespace PhotoCollage.Common;

public class CollageSettings
{
    public string Directory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    public bool IsGrayscale { get; set; } = false;
    public bool IsRandom { get; set; } = true;
    public int MaximumRotation { get; set; } = 15;
    public int MaximumSize { get; set; } = 500;
    public bool IsFullScreen { get; set; } = false;
    public bool RotateBasedOnEXIF { get; set; } = false;
    public int NumberOfPhotos { get; set; } = 10;
    public double Opacity { get; set; } = 1.0;
    public BorderType PhotoBorderType { get; set; } = BorderType.Border;
    public ScreensaverSpeed Speed { get; set; } = ScreensaverSpeed.Medium;
    public FullScreenMode PhotoFullScreenMode { get; set; } = FullScreenMode.Centered;
    public bool UseVerboseLogging { get; set; } = false;
}
