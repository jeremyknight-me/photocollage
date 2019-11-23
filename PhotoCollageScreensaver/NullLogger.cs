using PhotoCollageScreensaver.Contracts;

namespace PhotoCollageScreensaver
{
    internal class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }

        public void Log(string message, string stackTrace)
        {
        }
    }
}
