using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using PhotoCollage.Common.Photos;
using PhotoCollage.Common.Settings;
using PhotoCollageWeb.Server.Hubs;
using PhotoCollageWeb.Shared;

namespace PhotoCollageWeb.Server.Workers;

public class CollageWorker : BackgroundService
{
    private readonly ILogger<CollageWorker> logger;
    private readonly CollageSettings settings;
    private readonly IHubContext<CollageHub, ICollageClient> hub;
    private readonly ConcurrentQueue<Guid> photoIdQueue = new();
    private readonly IPhotoPathRepository photoPathRepo;
    private readonly IPhotoRepository photoRepo;

    public CollageWorker(
        ILogger<CollageWorker> appLogger,
        IOptions<CollageSettings> appOptions,
        IHubContext<CollageHub, ICollageClient> hubContext,
        IPhotoPathRepository photoPathRepository,
        IPhotoRepository photoRepository
        )
    {
        this.logger = appLogger;
        this.settings = appOptions.Value;
        this.hub = hubContext;
        this.photoPathRepo = photoPathRepository;
        this.photoRepo = photoRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.LoadPhotos();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (this.logger.IsEnabled(LogLevel.Information))
                {
                    this.logger.LogInformation("Collage worker ran at {time}", DateTimeOffset.Now);
                }

                var path = this.photoPathRepo.GetNextPath();
                var extension = Path.GetExtension(path);
                var bytes = File.ReadAllBytes(path);
                var photo = new PhotoData()
                {
                    Extension = extension,
                    Data = Convert.ToBase64String(bytes)
                };

                this.photoIdQueue.Enqueue(photo.Id);
                if (this.photoIdQueue.Count > this.settings.NumberOfPhotos
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
                await Task.Delay(TimeSpan.FromSeconds(speed), stoppingToken);
            }
        }
    }

    private void LoadPhotos()
    {
        try
        {
            this.photoRepo.LoadPhotoPaths();
        }
        catch (Exception ex)
        {
            this.logger.LogError("Error on Collage worker execution", ex);
        }
    }
}
