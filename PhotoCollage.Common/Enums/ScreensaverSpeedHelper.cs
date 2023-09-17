namespace PhotoCollage.Common.Enums;

public static class ScreensaverSpeedHelper
{
    public const string SuperSlow = "Super Slow";
    public const string ReallySlow = "Really Slow";
    public const string Slow = "Slow";
    public const string Medium = "Medium";
    public const string Fast = "Fast";

    public static IDictionary<ScreensaverSpeed, string> MakeDictionary()
        => new Dictionary<ScreensaverSpeed, string>
        {
                { ScreensaverSpeed.SuperSlow, SuperSlow },
                { ScreensaverSpeed.ReallySlow, ReallySlow },
                { ScreensaverSpeed.Slow, Slow },
                { ScreensaverSpeed.Medium, Medium },
                { ScreensaverSpeed.Fast, Fast }
        };
}
