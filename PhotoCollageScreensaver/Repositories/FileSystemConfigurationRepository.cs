using PhotoCollageScreensaver.Contracts;
using System.IO;
using System.Text.Json;

namespace PhotoCollageScreensaver.Repositories
{
    internal class FileSystemConfigurationRepository : IConfigurationRepository
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

        public Configuration Load()
        {
            string json = File.ReadAllText(this.filePath);
            var config = JsonSerializer.Deserialize<Configuration>(json);
            return config;
        }

        public void Save(Configuration configuration)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            var json = JsonSerializer.Serialize(configuration, options);
            File.WriteAllText(this.filePath, json);
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
                this.Save(new Configuration());
            }
        }
    }
}
