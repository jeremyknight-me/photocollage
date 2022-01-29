using PhotoCollage.Common;
using PhotoCollageWeb.Server.Workers;

namespace PhotoCollageWeb.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment host)
        {
            services.Configure<CollageSettings>(options => configuration.GetSection("Settings").Bind(options));
            services.AddHostedService<CollageWorker>();
        }
    }
}
