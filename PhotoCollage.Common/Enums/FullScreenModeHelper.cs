namespace PhotoCollage.Common.Enums;

public static class FullScreenModeHelper
{
    public const string Stretched = "Stretched";
    public const string Centred = "Centred";

    public static IDictionary<FullScreenMode, KeyValuePair<string, string>> MakeDictionary()
        => new Dictionary<FullScreenMode, KeyValuePair<string, string>>
        {
                { FullScreenMode.Stretched, new KeyValuePair<string, string>("stretched", Stretched) },
                { FullScreenMode.Centred, new KeyValuePair<string, string> ("centred", Centred) }
        };
}
