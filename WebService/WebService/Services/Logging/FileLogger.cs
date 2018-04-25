using System;
using System.IO;

namespace WebService.Services.Logging
{
    public class FileLogger : ILogger
    {
        private readonly ILogBuilder _logBuilder;


        public FileLogger(ILogBuilder logBuilder)
        {
            _logBuilder = logBuilder;
        }

        public FileLogger()
        {
            _logBuilder = new LogBuilder();
        }


        private static string FilePath => $"{DateTime.Now:yyyy-MMMM-dd}.log";


        public void Log<T>(T sender, ELogLevel logLevel, Exception exception)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            try
            {
                if (!File.Exists(FilePath))
                    File.Create(FilePath);

                var log = _logBuilder.BuildLogEntry(typeof(T).Name, logLevel, exception.Message);

                File.AppendAllLines(FilePath, new[] {log});
            }
            catch (IOException)
            {
                // IGNORED
            }
        }

        public void Log<T>(T sender, ELogLevel logLevel, string message)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            try
            {
                if (!File.Exists(FilePath))
                    File.Create(FilePath);

                var log = _logBuilder.BuildLogEntry(typeof(T).Name, logLevel, message);

                File.AppendAllLines(FilePath, new[] {log});
            }
            catch (IOException)
            {
                // IGNORED
            }
        }
    }
}