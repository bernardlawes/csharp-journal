using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace csharp_journal
{
    public class ChronoLogger
    {
        private readonly string _logFilePath;
        private readonly object _fileLock = new();
        private string _lastText = null;

        public ChronoLogger(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
                throw new ArgumentException("Log file path must not be null or empty.", nameof(logFilePath));

            _logFilePath = logFilePath;
            string? directory = Path.GetDirectoryName(_logFilePath);

            if (directory == null)
                throw new ArgumentException("Please provide a fully qualified path for the logfile.  The parent folder must exist: ", nameof(logFilePath));

            try
            {
                Directory.CreateDirectory(directory);

                // Attempt to write a test file to verify write access
                string testFilePath = Path.Combine(directory, Path.GetRandomFileName());
                File.WriteAllText(testFilePath, "write_test");
                File.Delete(testFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot write to the log directory: {directory}", ex);
            }
        }

        public void Log(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            lock (_fileLock)
            {
                if (text == _lastText) return; // Optional: Skip repeat entries

                _lastText = text;

                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);

                var logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {text}";
                File.AppendAllLines(_logFilePath, new[] { logLine });
            }
        }
    }
}
