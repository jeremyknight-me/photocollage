using System.Text.Json.Serialization;

namespace PhotoCollageScreensaver;

public sealed class CollageSettings
{
    // Single folder from older settings files; moved into Directories on load and no longer written.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Directory { get; set; }

    public List<string> Directories { get; set; } = [];
    public List<string> ExcludedDirectories { get; set; } = [];
    public bool IsGrayscale { get; set; } = false;
    public bool IsRandom { get; set; } = true;
    public int MaximumRotation { get; set; } = 15;
    public int MaximumSize { get; set; } = 500;
    public bool IsFullScreen { get; set; } = false;
    public bool FullscreenMatchOrientation { get; set; } = false;
    public bool RotateBasedOnEXIF { get; set; } = false;
    public int NumberOfPhotos { get; set; } = 10;
    public double Opacity { get; set; } = 1.0;
    public BorderType PhotoBorderType { get; set; } = BorderType.Border;
    public ScreensaverSpeed Speed { get; set; } = ScreensaverSpeed.Medium;
    public bool UseVerboseLogging { get; set; } = false;

    internal void MigrateLegacyDirectory()
    {
        Directories ??= [];
        ExcludedDirectories ??= [];
        if (Directories.Count == 0)
        {
            Directories.Add(string.IsNullOrWhiteSpace(Directory)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                : Directory);
        }

        Directory = null;
    }
}
