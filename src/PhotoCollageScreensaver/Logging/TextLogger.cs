using System.IO;

namespace PhotoCollageScreensaver.Logging;

public class TextLogger : ILogger
{
    private readonly ISettingsRepository settingsRepo;
    private readonly string directory;

    public TextLogger(
        string localDataDirectory,
        ISettingsRepository settingsRepository)
    {
        this.directory = Path.Combine(localDataDirectory, @"logs");
        this.settingsRepo = settingsRepository;
    }

    public void Log(Exception exception)
    {
        var lines = new List<string> { $"\n{DateTime.Now}  ==>  {exception.Message}" };
        if (this.settingsRepo.Current.UseVerboseLogging)
        {
            lines.Add("Stack Trace:");
            lines.Add(exception.StackTrace);
        }

        if (!Directory.Exists(this.directory))
        {
            Directory.CreateDirectory(this.directory);
        }

        var fullPath = Path.Combine(this.directory, $"log-{DateTime.Today:yyyy-MM-dd}.txt");
        File.AppendAllLines(fullPath, lines);
    }
}
