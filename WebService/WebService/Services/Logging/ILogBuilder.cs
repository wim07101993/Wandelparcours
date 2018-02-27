namespace WebService.Services.Logging
{
    /// <summary>
    /// ILogBuilder has just one method that is supposed to build a log message.
    /// </summary>
    public interface ILogBuilder
    {
        /// <summary>
        /// BuildLogEntry is supposed to build a log message using a tag, logLevel and message.
        /// </summary>
        /// <param name="tag">is the tag to identify the log</param>
        /// <param name="logLevel">is the level to describe the importance of the log</param>
        /// <param name="message">is the message given to the log</param>
        /// <returns>{logLevel}:{tag}->{message}</returns>
        string BuildLogEntry(string tag, ELogLevel logLevel, string message);
    }
}
