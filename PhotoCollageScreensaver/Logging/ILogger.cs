namespace PhotoCollageScreensaver.Logging;

internal interface ILogger
{
    void Log(string message);
    void Log(string message, string stackTrace);
}
