using System.Diagnostics;
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
        try
        {
            var contents = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(contents))
            {
                Current = new CollageSettings();
                return;
            }

            CollageSettings settings = contents.Trim().StartsWith("<?xml")
                ? LoadFromXml(contents) // provides fallback for upgrades from older version
                : LoadFromJson(contents);

            if (settings is null)
            {
                RecoverFromInvalidSettings(new InvalidDataException("The settings file contains no configuration."));
                return;
            }

            Current = settings;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or JsonException
            or InvalidOperationException)
        {
            RecoverFromInvalidSettings(exception);
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Current, _jsonSerializerOptions);

        // Write beside the destination so replacing it cannot leave a partial settings file after an interruption.
        var temporaryFilePath = Path.Combine(_directoryPath, $"{Path.GetFileName(_filePath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(temporaryFilePath, json);
            if (File.Exists(_filePath))
            {
                File.Replace(temporaryFilePath, _filePath, null);
            }
            else
            {
                File.Move(temporaryFilePath, _filePath);
            }
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
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
            File.Create(_filePath).Dispose();
            Save();
        }
    }

    private void RecoverFromInvalidSettings(Exception exception)
    {
        Current = new CollageSettings();
        Trace.TraceError($"Could not load settings from '{_filePath}': {exception}");

        try
        {
            BackupInvalidSettingsFile();
            Save();
        }
        catch (Exception recoveryException) when (recoveryException is IOException or UnauthorizedAccessException)
        {
            Trace.TraceError($"Could not recover settings at '{_filePath}': {recoveryException}");
        }
    }

    private void BackupInvalidSettingsFile()
    {
        if (File.Exists(_filePath))
        {
            var backupFilePath = Path.Combine(
                _directoryPath,
                $"{Path.GetFileName(_filePath)}.invalid-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}");
            File.Move(_filePath, backupFilePath);
        }
    }
}
