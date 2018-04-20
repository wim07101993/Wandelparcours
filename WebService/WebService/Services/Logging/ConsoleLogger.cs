using System;

namespace WebService.Services.Logging
{
    public class ConsoleLogger : ILogger
    {
        private readonly ILogBuilder _logBuilder;


        public ConsoleLogger(ILogBuilder logBuilder)
        {
            _logBuilder = logBuilder;
        }

        public ConsoleLogger()
        {
            _logBuilder = new LogBuilder();
        }


        public void Log<T>(T sender, ELogLevel logLevel, Exception exception)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            Console.WriteLine(_logBuilder.BuildLogEntry(typeof(T).Name, logLevel, exception.Message));
        }

        public void Log<T>(T sender, ELogLevel logLevel, string message)
        {
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            Console.WriteLine(_logBuilder.BuildLogEntry(typeof(T).Name, logLevel, message));
        }
    }
}