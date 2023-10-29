namespace PhotoCollageScreensaver.Logging;

public interface ILogger
{
    void Log(Exception exception);
    //void Log(string message);
    //void Log(string message, string stackTrace);
}
