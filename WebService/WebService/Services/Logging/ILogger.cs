using System;

namespace WebService.Services.Logging
{
    public interface ILogger
    {
        void Log<T>(T sender, ELogLevel logLevel, Exception exception);

        void Log<T>(T sender, ELogLevel logLevel, string message);
    }
}