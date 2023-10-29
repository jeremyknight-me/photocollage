namespace PhotoCollage.Common.Settings;

public interface ISettingsRepository
{
    CollageSettings Current { get; }

    void Load();
    void Save();
}
