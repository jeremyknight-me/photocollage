using System.Text.Json;

namespace PhotoCollageScreensaver;

internal class FileSystemSettingsRepository : ISettingsRepository
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 2,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly string _directoryPath;
    private readonly string _filePath;

    public FileSystemSettingsRepository(string configurationFolderPath)
    {
        _directoryPath = configurationFolderPath;
        _filePath = Path.Combine(_directoryPath, @"photo-collage.config");
        EnsureDirectoryExists();
        EnsureFileExists();
    }

    public CollageSettings Current
    {
        get
        {
            if (field is null)
            {
                Load();
            }

            return field;
        }
        private set => field = value;
    }

    public void Load()
    {
        var contents = File.ReadAllText(_filePath);
        Current = contents.Trim().StartsWith("<?xml")
            ? LoadFromXml(contents) // provides fallback for upgrades from older version
            : LoadFromJson(contents);
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Current, _jsonSerializerOptions);
        File.WriteAllText(_filePath, json);
    }

    private static CollageSettings LoadFromJson(string contents) => JsonSerializer.Deserialize<CollageSettings>(contents, _jsonSerializerOptions);

    private static CollageSettings LoadFromXml(string contents)
    {
        contents = contents.Replace("ScreensaverConfiguration", "Configuration");
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CollageSettings));
        using TextReader reader = new StringReader(contents);
        return serializer.Deserialize(reader) as CollageSettings;
    }

    private void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(_filePath))
        {
            File.CreateText(_filePath).Close();
            Save();
        }
    }
}
