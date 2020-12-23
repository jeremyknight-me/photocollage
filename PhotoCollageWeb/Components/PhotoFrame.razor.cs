using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollage.Common.Enums;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components
{
    public partial class PhotoFrame
    {
        [Parameter] public PhotoData Image { get; set; }
        [Parameter] public bool IsRemoved { get; set; } = false;
        [Inject] protected IOptions<CollageSettings> Options { get; set; }
        protected CollageSettings Settings => this.Options.Value;

        protected string CssClasses => "photo-frame" + this.GetBorderClasses() + this.GetRemovedClasses();

        private string GetBorderClasses() => this.Settings.PhotoBorderType switch
        {
            BorderType.Border => " bordered",
            BorderType.BorderHeader => " bordered",
            BorderType.BorderFooter => " bordered",
            _ => string.Empty,
        };

        private string GetRemovedClasses() => this.IsRemoved ? " removed" : string.Empty;
    }
}
