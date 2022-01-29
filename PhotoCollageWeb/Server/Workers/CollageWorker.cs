using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollage.Common.Data;
using PhotoCollageWeb.Server.Hubs;
using PhotoCollageWeb.Shared;

namespace PhotoCollageWeb.Server.Workers
{
    public class CollageWorker : BackgroundService
    {
        private readonly ILogger<CollageWorker> logger;
        private readonly CollageSettings settings;
        private readonly IHubContext<CollageHub, ICollageClient> hub;
        private readonly ConcurrentQueue<Guid> photoIdQueue = new ConcurrentQueue<Guid>();
        private IPhotoRepository photoRepository;

        public CollageWorker(
            ILogger<CollageWorker> appLogger,
            IOptions<CollageSettings> appOptions,
            IHubContext<CollageHub, ICollageClient> hubContext
            )
        {
            this.logger = appLogger;
            this.settings = appOptions.Value;
            this.hub = hubContext;
            this.photoRepository = new PhotoRepositoryFactory(this.settings).Make();
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

                    var path = this.photoRepository.GetNextPhotoFilePath();
                    var extension = Path.GetExtension(path);
                    var bytes = File.ReadAllBytes(path);
                    var photo = new PhotoData()
                    {
                        Extension = extension,
                        Data = Convert.ToBase64String(bytes)
                    };
                    this.photoIdQueue.Enqueue(photo.Id);

                    if (this.photoIdQueue.Count > (this.settings.NumberOfPhotos + 1)
                        && this.photoIdQueue.TryDequeue(out var result))
                    {
                        await this.hub.Clients.Group(CollageHub.ConnectedGroupName).ReceiveRemove(result);
                    }

                    await this.hub.Clients.Group(CollageHub.ConnectedGroupName).ReceivePhoto(photo);
                }
                catch (Exception ex)
                {
                    this.logger.LogError("Error on Collage worker execution", ex);
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
