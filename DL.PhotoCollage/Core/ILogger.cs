namespace DL.PhotoCollage.Core
{
    public interface ILogger
    {
        void Log(string message);

        void Log(string message, string stackTrace);
    }
}
