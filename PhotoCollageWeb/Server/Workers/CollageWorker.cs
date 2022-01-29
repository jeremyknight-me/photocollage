using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollageWeb.Server.Hubs;
using PhotoCollageWeb.Shared;

namespace PhotoCollageWeb.Server.Workers
{
    public class CollageWorker : BackgroundService
    {
        private readonly ILogger<CollageWorker> logger;
        private readonly CollageSettings settings;
        private readonly IHubContext<CollageHub, ICollageClient> hub;

        public CollageWorker(
            ILogger<CollageWorker> appLogger,
            IOptions<CollageSettings> appOptions,
            IHubContext<CollageHub, ICollageClient> hubContext
            )
        {
            this.logger = appLogger;
            this.settings = appOptions.Value;
            this.hub = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (this.logger.IsEnabled(LogLevel.Information))
                    {
                        this.logger.LogInformation("Collage worker ran at {time}", DateTimeOffset.Now);
                    }

                    await this.hub.Clients.All.ReceivePhoto(new PhotoData());
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    var speed = 2 * (int)this.settings.Speed;
                    await Task.Delay(TimeSpan.FromSeconds(speed));
                }
            }
        }
    }
}
