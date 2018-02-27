namespace WebService.Services.Logging
{
    /// <inheritdoc cref="ILogBuilder"/>
    /// <summary>
    /// LogBuilder is a class that implements the ILogBuilder interface.
    /// <para>
    /// It has just one method that builds a log message.
    /// </para>
    /// </summary>
    public class LogBuilder : ILogBuilder
    {
        /// <summary>
        /// BuildLogEntry builds a log message using a tag, logLevel and message.
        /// </summary>
        /// <param name="tag">is the tag to identify the log</param>
        /// <param name="logLevel">is the level to describe the importance of the log</param>
        /// <param name="message">is the message given to the log</param>
        /// <returns>{logLevel}:{tag}->{message}</returns>
        public string BuildLogEntry(string tag, ELogLevel logLevel, string message)
            => $"{logLevel.ToString().ToUpper()}:{tag}->{message}";
    }
}
