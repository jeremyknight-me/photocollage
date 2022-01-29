using Microsoft.AspNetCore.SignalR;

namespace PhotoCollageWeb.Server.Hubs
{
    public class CollageHub : Hub<ICollageClient>
    {
    }
}
