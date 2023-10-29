using PhotoCollage.Common.Photos;
using PhotoCollage.Common.Photos.FileSystem;
using PhotoCollage.Common.Photos.InMemory;
using PhotoCollage.Common.Settings;
using PhotoCollageWeb.Server.Workers;

namespace PhotoCollageWeb.Server.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CollageSettings>(options => configuration.GetSection("Settings").Bind(options));
        services.AddSingleton<ISettingsRepository, AppSettingsRepository>();
        services.AddSingleton<IPhotoRepository, FileSystemPhotoRepository>();

        services.AddSingleton<InMemoryRandomPhotoPathRepository>();
        services.AddSingleton<InMemoryOrderedPhotoPathRepository>();
        services.AddSingleton<IPhotoPathRepository>(provider =>
        {
            var settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return settingsRepo.Current.IsRandom
                ? provider.GetRequiredService<InMemoryRandomPhotoPathRepository>()
                : provider.GetRequiredService<InMemoryOrderedPhotoPathRepository>();
        });

        services.AddHostedService<CollageWorker>();
    }
}
