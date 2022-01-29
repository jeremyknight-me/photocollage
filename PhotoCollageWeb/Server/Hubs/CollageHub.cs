using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;

namespace PhotoCollageWeb.Server.Hubs
{
    public class CollageHub : Hub<ICollageClient>
    {
        public const string ConnectedGroupName = "connected";
        private readonly CollageSettings settings;

        public CollageHub(IOptions<CollageSettings> appOptions)
        {
            this.settings = appOptions.Value;
        }

        public override async Task OnConnectedAsync()
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, ConnectedGroupName);
            await this.Clients.Caller.ReceiveConnected(this.settings);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, ConnectedGroupName);
        }
    }
}
