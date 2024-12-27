namespace PhotoCollageScreensaver;

public interface ISettingsRepository
{
    CollageSettings Current { get; }

    void Load();
    void Save();
}
