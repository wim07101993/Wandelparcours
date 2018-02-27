using System;

namespace WebService.Services.Logging
{
    /// <inheritdoc cref="ILogger"/>
    /// <summary>
    /// ConsoleLogger is a class that implements the ILogger Interface.
    /// <para>
    /// It logs messages to the console. The log messages are built with an <see cref="ILogBuilder"/> instance.
    /// If the logbuilder is injected in the constructor, an instance of the <see cref="LogBuilder"/> class is used.
    /// </para>
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        #region FIELDS

        /// <summary>
        /// _logBuilder is the instance to create the messages to log.
        /// </summary>
        private readonly ILogBuilder _logBuilder;

        #endregion FIELDS


        #region CONSTRUCTOR

        /// <summary>
        /// ConsoleLogger is the constructor to create an instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="logBuilder">is the instance to create the messages to log</param>
        public ConsoleLogger(ILogBuilder logBuilder)
        {
            // set the field with the given value
            _logBuilder = logBuilder;
        }

        /// <summary>
        /// ConsoleLogger is the constructor to create an instance of the <see cref="ConsoleLogger"/> class.
        /// <para/>
        /// Becaus there is no <see cref="ILogBuilder"/> injected, the default is used (<see cref="LogBuilder"/>).
        /// </summary>
        public ConsoleLogger()
        {
            // set the field with a new instance of the LogBuilder class
            _logBuilder = new LogBuilder();
        }

        #endregion CONSTRUCTOR


        #region METHODS

        /// <inheritdoc cref="ILogger.Log{T}(T,ELogLevel,Exception)"/>
        /// <summary>
        /// Log&lt;T&gt; logs the message of an exception to the console.
        /// <para/>
        /// If the logLevel is <see cref="ELogLevel.Debug"/>, the messages are only shown in the debug build of the app.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the exception</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="exception">is the exception to log the message of</param>
        public void Log<T>(T sender, ELogLevel logLevel, Exception exception)
        {
            // if the app is in debug mode and the loglevel is debug, do nothing
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            // log the exception-message to the console
            Console.WriteLine(_logBuilder.BuildLogEntry(typeof(T).Name, logLevel, exception.Message));
        }

        /// <inheritdoc cref="ILogger.Log{T}(T,ELogLevel,string)"/>
        /// <summary>
        /// Log&lt;T&gt; logs a message to the console.
        /// <para/>
        /// If the logLevel is <see cref="ELogLevel.Debug"/>, the messages are only shown in the debug build of the app.
        /// </summary>
        /// <typeparam name="T">is the type of the sender that wants to log the message</typeparam>
        /// <param name="sender">is that wants to log the exception</param>
        /// <param name="logLevel">is the importance of the log</param>
        /// <param name="message">is the message to log</param>
        public void Log<T>(T sender, ELogLevel logLevel, string message)
        {
            // if the app is in debug mode and the loglevel is debug, do nothing
#if DEBUG
            if (logLevel == ELogLevel.Debug)
                return;
#endif
            // log the exception-message to the console
            Console.WriteLine(_logBuilder.BuildLogEntry(typeof(T).Name, logLevel, message));
        }

        #endregion METHODS
    }
}