using Microsoft.Extensions.Options;
using PhotoCollage.Common.Settings;

namespace PhotoCollageWeb.Server;

internal sealed class AppSettingsRepository : ISettingsRepository
{
    private readonly IOptionsMonitor<CollageSettings> monitor;

    public AppSettingsRepository(IOptionsMonitor<CollageSettings> settingsOptionsMonitor)
    {
        this.monitor = settingsOptionsMonitor;
    }

    public CollageSettings Current => this.monitor.CurrentValue;

    public void Load() { }
    public void Save() { }
}
