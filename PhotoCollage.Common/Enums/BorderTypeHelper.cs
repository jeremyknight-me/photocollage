using System.Collections.Generic;

namespace PhotoCollage.Common.Enums
{
    public static class BorderTypeHelper
    {
        public const string None = "None";
        public const string Border = "Border";
        public const string BorderHeader = "Border with Header";
        public const string BorderFooter = "Border with Footer";

        public static IDictionary<BorderType, KeyValuePair<string, string>> MakeDictionary()
            => new Dictionary<BorderType, KeyValuePair<string, string>>
            {
                { BorderType.None, new KeyValuePair<string, string>("none", None) },
                { BorderType.Border, new KeyValuePair<string, string> ("border", Border) },
                { BorderType.BorderHeader, new KeyValuePair<string, string> ("header", BorderHeader) },
                { BorderType.BorderFooter, new KeyValuePair<string, string> ("footer", BorderFooter) }
            };
    }
}
