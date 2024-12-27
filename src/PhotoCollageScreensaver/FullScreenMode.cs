namespace PhotoCollageScreensaver;

public enum FullScreenMode
{
    Stretched,
    Centered
}

public static class FullScreenModeHelper
{
    public const string Stretched = "Stretched";
    public const string Centered = "Centered";

    public static IDictionary<FullScreenMode, KeyValuePair<string, string>> MakeDictionary()
        => new Dictionary<FullScreenMode, KeyValuePair<string, string>>
        {
            {
                FullScreenMode.Stretched,
                new KeyValuePair<string, string>(Stretched.ToLower(), Stretched)
            },
            {
                FullScreenMode.Centered,
                new KeyValuePair<string, string> (Centered.ToLower(), Centered)
            }
        };
}
