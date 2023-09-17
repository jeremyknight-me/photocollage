using System.IO;
using System.Text.Json;

namespace PhotoCollageScreensaver.Data;

internal class FileSystemSettingsRepository : ISettingsRepository
{
    private readonly string directoryPath;
    private readonly string filePath;

    public FileSystemSettingsRepository(string configurationFolderPath)
    {
        this.directoryPath = configurationFolderPath;
        this.filePath = Path.Combine(this.directoryPath, @"photo-collage.config");
        this.EnsureDirectoryExists();
        this.EnsureFileExists();
    }

    public CollageSettings Load()
    {
        var contents = File.ReadAllText(this.filePath);
        return contents.Trim().StartsWith("<?xml")
            ? this.LoadFromXml(contents) // provides fallback for upgrades from older version
            : this.LoadFromJson(contents);
    }

    public void Save(CollageSettings configuration)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(configuration, options);
        File.WriteAllText(this.filePath, json);
    }

    private CollageSettings LoadFromJson(string contents) => JsonSerializer.Deserialize<CollageSettings>(contents);

    private CollageSettings LoadFromXml(string contents)
    {
        contents = contents.Replace("ScreensaverConfiguration", "Configuration");
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CollageSettings));
        using TextReader reader = new StringReader(contents);
        return serializer.Deserialize(reader) as CollageSettings;
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
            this.Save(new CollageSettings());
        }
    }
}
