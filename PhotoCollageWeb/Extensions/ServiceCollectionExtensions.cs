using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhotoCollage.Common;

namespace PhotoCollageWeb.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment host)
        {
            services.Configure<CollageSettings>(options => configuration.GetSection("Settings").Bind(options));
        }
    }
}
