namespace DL.PhotoCollage.Core
{
    public interface IConfigurationRepository
    {
        ScreensaverConfiguration Load();

        void Save(ScreensaverConfiguration configuration);
    }
}
