using System.Text.Json;

namespace PhotoCollageScreensaver;

internal class FileSystemSettingsRepository : ISettingsRepository
{
    private readonly string directoryPath;
    private readonly string filePath;

    private CollageSettings current = null;

    public FileSystemSettingsRepository(string configurationFolderPath)
    {
        this.directoryPath = configurationFolderPath;
        this.filePath = Path.Combine(this.directoryPath, @"photo-collage.config");
        this.EnsureDirectoryExists();
        this.EnsureFileExists();
    }

    public CollageSettings Current
    {
        get
        {
            if (this.current is null)
            {
                this.Load();
            }

            return this.current;
        }
        private set => this.current = value;
    }

    public void Load()
    {
        var contents = File.ReadAllText(this.filePath);
        this.Current = contents.Trim().StartsWith("<?xml")
            ? this.LoadFromXml(contents) // provides fallback for upgrades from older version
            : this.LoadFromJson(contents);
    }

    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(this.Current, options);
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
            this.Save();
        }
    }
}
