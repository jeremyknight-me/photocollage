using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollage.Common.Enums;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components
{
    public partial class PhotoFrame
    {
        [Parameter] public ImageData Image { get; set; }
        [Inject] protected IOptions<CollageSettings> Options { get; set; }
        protected CollageSettings Settings => this.Options.Value;

        protected string CssClasses {
            get
            {
                var classes = "photo-frame" + this.GetBorderClasses();
                return classes;
            }
            
        }

        protected string CssStyles
        {
            get
            {
                var size = this.Settings.MaximumSize;
                var half = size / 2;
                var styles = $"left:calc({this.Image.PositionLeft}vw - {half}px);top:calc({this.Image.PositionTop}vh - {half}px);transform:rotate({this.Image.Rotation}deg);z-index:{this.Image.Count}";
                return styles;
            }
        }

        private string GetBorderClasses() => this.Settings.PhotoBorderType switch
        {
            BorderType.Border => " bordered",
            BorderType.BorderHeader => " bordered",
            BorderType.BorderFooter => " bordered",
            _ => string.Empty,
        };
    }
}
