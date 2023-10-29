using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using PhotoCollage.Common.Data;
using PhotoCollageScreensaver.Data;
using PhotoCollageScreensaver.Logging;
using PhotoCollageScreensaver.ViewModels;
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

        services.AddSingleton<CollagePresenterCollage>();
        services.AddSingleton<CollagePresenterFullscreen>();
        services.AddTransient<CollagePresenter>(provider =>
        {
            var settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return settingsRepo.Current.IsFullScreen
                ? provider.GetRequiredService<CollagePresenterFullscreen>()
                : provider.GetRequiredService<CollagePresenterCollage>();
        });

        services.AddSingleton<RandomFileSystemPhotoRepository>();
        services.AddSingleton<OrderedFileSystemPhotoRepository>();
        services.AddTransient<IPhotoRepository>(provider =>
        {
            var settingsRepo = provider.GetRequiredService<ISettingsRepository>();
            return settingsRepo.Current.IsRandom
                ? provider.GetRequiredService<RandomFileSystemPhotoRepository>()
                : provider.GetRequiredService<OrderedFileSystemPhotoRepository>();
        });
        services.AddTransient<ScreensaverController>();

        services.AddTransient<SetupViewModel>();
        services.AddTransient<SetupWindow>();
    }

    private static string GetLocalDataDirectory()
        => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"DigitalLagniappe\Screensavers\PhotoCollage");
}
