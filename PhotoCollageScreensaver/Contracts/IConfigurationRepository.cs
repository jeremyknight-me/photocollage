namespace PhotoCollageScreensaver.Contracts
{
    internal interface IConfigurationRepository
    {
        Configuration Load();
        void Save(Configuration configuration);
    }
}
