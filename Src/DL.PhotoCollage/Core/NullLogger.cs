namespace DL.PhotoCollage.Core
{
    public class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }

        public void Log(string message, string stackTrace)
        {
        }
    }
}
