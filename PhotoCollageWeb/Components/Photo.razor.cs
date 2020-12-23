using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components
{
    public partial class Photo
    {
        [Parameter] public ImageData Image { get; set; }
        [Inject] protected IOptions<CollageSettings> Options { get; set; }
        protected CollageSettings Settings => this.Options.Value;
        protected string CssClasses => string.Empty;

        protected string CssStyles
        {
            get
            {
                var size = this.Settings.MaximumSize;
                var styles = $"max-height:{size}px;max-width:{size}px;";

                if (this.Settings.IsGrayscale)
                {
                    styles += "filter:grayscale(1);";
                }

                return styles;
            }
        }
    }
}
