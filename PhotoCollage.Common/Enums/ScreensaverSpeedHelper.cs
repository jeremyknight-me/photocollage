using System.Collections.Generic;

namespace PhotoCollage.Common.Enums
{
    public static class ScreensaverSpeedHelper
    {
        public const string Slow = "Slow";
        public const string Medium = "Medium";
        public const string Fast = "Fast";

        public static IDictionary<ScreensaverSpeed, string> MakeDictionary()
            => new Dictionary<ScreensaverSpeed, string>
            {
                { ScreensaverSpeed.Slow, Slow },
                { ScreensaverSpeed.Medium, Medium },
                { ScreensaverSpeed.Fast, Fast }                
            };
    }
}
