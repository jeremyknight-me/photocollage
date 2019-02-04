namespace DL.PhotoCollage
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
