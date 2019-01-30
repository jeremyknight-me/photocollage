using System;
using System.IO;

namespace DL.PhotoCollage
{
    public class TextLogger : ILogger
    {
        private readonly string directory;

        public TextLogger(string directoryPath)
        {
            this.directory = Path.Combine(directoryPath, @"logs");
        }

        public void Log(string message)
        {
            this.EnsureDirectory();
            string fullPath = this.GetFullFilePath();

            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(GetLogEntry(message));
                writer.Flush();
            }
        }

        public void Log(string message, string stackTrace)
        {
            this.EnsureDirectory();
            string fullPath = this.GetFullFilePath();

            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(GetLogEntry(message));
                writer.WriteLine("Stack Trace:");
                writer.WriteLine(stackTrace);
                writer.Flush();
            }
        }

        private void EnsureDirectory()
        {
            if (!Directory.Exists(this.directory))
            {
                Directory.CreateDirectory(this.directory);
            }
        }

        private string GetLogEntry(string message)
        {
            string date = DateTime.Now.ToString();
            return string.Concat(date, "  ==>  ", message);
        }

        private string GetFileName()
        {
            return "log-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
        }

        private string GetFullFilePath()
        {
            return Path.Combine(this.directory, GetFileName());
        }
    }
}