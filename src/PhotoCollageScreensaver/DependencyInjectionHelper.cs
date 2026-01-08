using Microsoft.Extensions.DependencyInjection;
using PhotoCollageScreensaver.Collage.Presenters;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.Photos;
using PhotoCollageScreensaver.Photos.FileSystem;
using PhotoCollageScreensaver.Photos.InMemory;
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

        services.AddSingleton<ILogger, TextLogger>(provider =>
        {
            ISettingsRepository settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return new TextLogger(localDataDirectory, settingsRepo);
        });

        services.AddSingleton<ISettingsRepository, FileSystemSettingsRepository>(provider => new FileSystemSettingsRepository(localDataDirectory));
        services.AddSingleton<IPhotoRepository, FileSystemPhotoRepository>();

        services.AddSingleton<InMemoryRandomPhotoPathRepository>();
        services.AddSingleton<InMemoryOrderedPhotoPathRepository>();
        services.AddSingleton<IPhotoPathRepository>(provider =>
        {
            ISettingsRepository settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return settingsRepo.Current.IsRandom
                ? provider.GetRequiredService<InMemoryRandomPhotoPathRepository>()
                : provider.GetRequiredService<InMemoryOrderedPhotoPathRepository>();
        });

        services.AddTransient<CollagePresenterCollage>();
        services.AddTransient<CollagePresenterFullscreen>();
        services.AddSingleton<CollagePresenterFactory>();
        services.AddTransient<CollagePresenter>(provider =>
        {
            CollagePresenterFactory factory = provider.GetRequiredService<CollagePresenterFactory>();
            return factory.Create();
        });

        services.AddTransient<SetupViewModel>();
        services.AddTransient<SetupWindow>();
    }

    private static string GetLocalDataDirectory()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"DigitalLagniappe\Screensavers\PhotoCollage");
}
