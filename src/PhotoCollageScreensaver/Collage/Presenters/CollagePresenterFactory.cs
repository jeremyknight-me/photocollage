using Microsoft.Extensions.DependencyInjection;

namespace PhotoCollageScreensaver.Collage.Presenters;

public sealed class CollagePresenterFactory
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IServiceProvider _serviceProvider;

    public CollagePresenterFactory(
        ISettingsRepository settingsRepository,
        IServiceProvider serviceProvider)
    {
        _settingsRepository = settingsRepository;
        _serviceProvider = serviceProvider;
    }

    public CollagePresenter Create()
    {
        return _settingsRepository.Current.IsFullScreen
            ? _serviceProvider.GetRequiredService<CollagePresenterFullscreen>()
            : _serviceProvider.GetRequiredService<CollagePresenterCollage>();
    }
}
