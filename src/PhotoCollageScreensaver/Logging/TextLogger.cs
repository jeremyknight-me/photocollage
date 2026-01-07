namespace PhotoCollageScreensaver.Logging;

public class TextLogger : ILogger
{
    private readonly ISettingsRepository _settingsRepo;
    private readonly string _directory;

    public TextLogger(
        string localDataDirectory,
        ISettingsRepository settingsRepository)
    {
        _directory = Path.Combine(localDataDirectory, @"logs");
        _settingsRepo = settingsRepository;
    }

    public void Log(Exception exception)
    {
        List<string> lines = [$"\n{DateTime.Now}  ==>  {exception.Message}"];
        if (_settingsRepo.Current.UseVerboseLogging)
        {
            lines.Add("Stack Trace:");
            lines.Add(exception.StackTrace);
        }

        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }

        var fullPath = Path.Combine(_directory, $"log-{DateTime.Today:yyyy-MM-dd}.txt");
        File.AppendAllLines(fullPath, lines);
    }
}
