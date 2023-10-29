using PhotoCollage.Common;
using PhotoCollage.Common.Data;
using PhotoCollageWeb.Server.Workers;

namespace PhotoCollageWeb.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment host)
        {
            services.Configure<CollageSettings>(options => configuration.GetSection("Settings").Bind(options));

            services.AddSingleton<RandomFileSystemPhotoRepository>();
            services.AddSingleton<OrderedFileSystemPhotoRepository>();
            services.AddTransient<IPhotoRepository>(provider =>
            {
                var settingsRepo = provider.GetRequiredService<ISettingsRepository>();
                return settingsRepo.Current.IsRandom
                    ? provider.GetRequiredService<RandomFileSystemPhotoRepository>()
                    : provider.GetRequiredService<OrderedFileSystemPhotoRepository>();
            });

            services.AddHostedService<CollageWorker>();
        }
    }
}
