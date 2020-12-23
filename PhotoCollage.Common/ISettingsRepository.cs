namespace PhotoCollage.Common
{
    public interface ISettingsRepository
    {
        CollageSettings Load();
        void Save(CollageSettings configuration);
    }
}
