namespace DL.PhotoCollage
{
    public interface IConfigurationRepository
    {
        ScreensaverConfiguration Load();

        void Save(ScreensaverConfiguration configuration);
    }
}
