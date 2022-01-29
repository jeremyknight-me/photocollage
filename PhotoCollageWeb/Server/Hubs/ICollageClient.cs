using PhotoCollage.Common;
using PhotoCollageWeb.Shared;

namespace PhotoCollageWeb.Server.Hubs
{
    public interface ICollageClient
    {
        Task ReceivePhoto(PhotoData photo);
        Task ReceiveRemove(Guid photoId);
        Task ReceiveConnected(CollageSettings settings);
    }
}
