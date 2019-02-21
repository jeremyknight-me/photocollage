namespace DL.PhotoCollage
{
    public interface IConfigurationRepository
    {
        IConfiguration Load();
        void Save(IConfiguration configuration);
    }
}
