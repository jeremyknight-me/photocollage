using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components
{
    public partial class Photo
    {
        [Parameter] public PhotoData Image { get; set; }
        [Inject] protected IOptions<CollageSettings> Options { get; set; }
        protected CollageSettings Settings => this.Options.Value;
    }
}
