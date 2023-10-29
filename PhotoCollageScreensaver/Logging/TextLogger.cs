using System.IO;
using PhotoCollage.Common.Settings;

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
        if (this.settingsRepo.Current.UseVerboseLogging)
        {
            this.Log(exception.Message, exception.StackTrace);
        }
        else
        {
            this.Log(exception.Message);
        }
    }

    private void Log(string message)
    {
        var fullPath = this.FullFilePath;
        File.AppendAllText(fullPath, this.GetLogEntry(message));
    }

    private void Log(string message, string stackTrace)
    {
        var fullPath = this.FullFilePath;
        var lines = new List<string>()
            {
                this.GetLogEntry(message),
                "Stack Trace:",
                stackTrace
            };
        File.AppendAllLines(fullPath, lines);
    }

    private string GetLogEntry(string message)
    {
        var date = DateTime.Now.ToString();
        return string.Concat("\n", date, "  ==>  ", message);
    }

    private string FullFilePath => Path.Combine(this.directory, $"log-{DateTime.Today:yyyy-MM-dd}.txt");
}
