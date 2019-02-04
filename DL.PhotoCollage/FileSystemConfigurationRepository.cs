using System.IO;
using System.Xml.Serialization;

namespace DL.PhotoCollage
{
    public class FileSystemConfigurationRepository : IConfigurationRepository
    {
        private readonly string directoryPath;
        private readonly string filePath;

        public FileSystemConfigurationRepository(string configurationFolderPath)
        {
            this.directoryPath = configurationFolderPath;
            this.filePath = Path.Combine(this.directoryPath, @"photo-collage.config");
            this.EnsureDirectoryExists();
            this.EnsureFileExists();
        }

        public IConfiguration Load()
        {
            using (var fileStream = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
            {
                var serializer = new XmlSerializer(typeof(ScreensaverConfiguration));
                return serializer.Deserialize(fileStream) as ScreensaverConfiguration;
            }
        }

        public void Save(IConfiguration configuration)
        {
            using (var writer = new StreamWriter(this.filePath))
            {
                var serializer = new XmlSerializer(typeof(ScreensaverConfiguration));
                serializer.Serialize(writer, configuration);
            }
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(this.directoryPath))
            {
                Directory.CreateDirectory(this.directoryPath);
            }
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(this.filePath))
            {
                File.CreateText(this.filePath).Close();
                this.Save(new ScreensaverConfiguration());
            }
        }
    }
}
