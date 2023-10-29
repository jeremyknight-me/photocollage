namespace PhotoCollageScreensaver.Logging;

internal class NullLogger : ILogger
{
    public void Log(Exception exception)
    {
    }

    //public void Log(string message)
    //{
    //}

    //public void Log(string message, string stackTrace)
    //{
    //}
}
