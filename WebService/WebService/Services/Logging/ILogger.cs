using System;

namespace WebService.Services.Logging
{
    /// <summary>
    /// Represents a type used to perform logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log&lt;T&gt; is supposed to log the message of an exception to the file.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the exception</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="exception">is the exception to log the message of</param>
        void Log<T>(T sender, ELogLevel logLevel, Exception exception);

        /// <summary>
        /// Log&lt;T&gt; is supposed to log a message to the file.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the message</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="message">is the message to log</param>
        void Log<T>(T sender, ELogLevel logLevel, string message);
    }
}