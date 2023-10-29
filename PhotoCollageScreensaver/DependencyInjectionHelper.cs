using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PhotoCollage.Common.Photos;
using PhotoCollage.Common.Photos.FileSystem;
using PhotoCollage.Common.Photos.InMemory;
using PhotoCollage.Common.Settings;
using PhotoCollageScreensaver.Collage.Presenters;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Setup;
using PhotoCollageScreensaver.Views;

namespace PhotoCollageScreensaver;

internal static class DependencyInjectionHelper
{
    internal static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        return services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var localDataDirectory = GetLocalDataDirectory();

        services.AddSingleton<ILogger, TextLogger>(provider => new TextLogger(localDataDirectory));
        services.AddSingleton<ErrorHandler>();
        services.AddSingleton<ISettingsRepository, FileSystemSettingsRepository>(provider => new FileSystemSettingsRepository(localDataDirectory));
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

        services.AddTransient<CollagePresenterCollage>();
        services.AddTransient<CollagePresenterFullscreen>();
        services.AddTransient<CollagePresenter>(provider =>
        {
            var settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return settingsRepo.Current.IsFullScreen
                ? provider.GetRequiredService<CollagePresenterFullscreen>()
                : provider.GetRequiredService<CollagePresenterCollage>();
        });

        services.AddTransient<SetupViewModel>();
        services.AddTransient<SetupWindow>();
    }

    private static string GetLocalDataDirectory()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"DigitalLagniappe\Screensavers\PhotoCollage");
}
