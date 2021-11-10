using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollage.Common.Enums;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components;

public partial class PhotoFrame
{
    //private bool shouldRender = true;
    //private bool previousIsRemoved = false;

    [Parameter] public PhotoData Image { get; set; }
    [Inject] protected IOptions<CollageSettings> Options { get; set; }
    protected CollageSettings Settings => this.Options.Value;

    protected string CssClasses => "photo-frame" + this.GetBorderClasses() + this.GetRemovedClasses();

    //protected override void OnParametersSet()
    //{
    //    this.shouldRender = this.previousIsRemoved != this.Image.IsRemoved;
    //    this.previousIsRemoved = this.Image.IsRemoved;
    //}

    //protected override bool ShouldRender() => this.Image.IsRemoved;

    private string GetBorderClasses() => this.Settings.PhotoBorderType switch
    {
        BorderType.Border => " bordered",
        BorderType.BorderHeader => " bordered",
        BorderType.BorderFooter => " bordered",
        _ => string.Empty,
    };

    private string GetRemovedClasses() => this.Image.IsRemoved ? " removed" : string.Empty;
}
