using System.ComponentModel;

namespace DL.PhotoCollage.Core
{
    public enum BorderType
    {
        [Description("None")]
        None,
        [Description("Border")]
        Border,
        [Description("Border with Header")]
        BorderHeader,
        [Description("Border with Footer")]
        BorderFooter
    }
}
