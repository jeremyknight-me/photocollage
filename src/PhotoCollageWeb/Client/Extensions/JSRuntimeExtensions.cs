using Microsoft.JSInterop;
using PhotoCollageWeb.Client.Models;

namespace PhotoCollageWeb.Client.Extensions
{
    internal static class JSRuntimeExtensions
    {
        public static async Task AddPhoto(this IJSRuntime js, PhotoSettings photoSettings)
            => await js.InvokeVoidAsync("jk.addPhoto", photoSettings);

        public static async Task RemovePhoto(this IJSRuntime js, Guid photoId)
            => await js.InvokeVoidAsync("jk.removePhoto", photoId);
    }
}
