using PhotoCollageScreensaver.Contracts;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoCollageScreensaver
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
            string fullPath = this.GetFullFilePath();
            File.AppendAllText(fullPath, this.GetLogEntry(message));
        }

        public void Log(string message, string stackTrace)
        {
            string fullPath = this.GetFullFilePath();
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
