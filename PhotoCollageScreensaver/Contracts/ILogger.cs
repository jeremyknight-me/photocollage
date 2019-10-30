namespace PhotoCollageScreensaver.Contracts
{
    internal interface ILogger
    {
        void Log(string message);
        void Log(string message, string stackTrace);
    }
}
