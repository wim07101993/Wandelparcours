using System.Collections.Generic;

namespace WebService.Services.Logging
{
    /// <inheritdoc >
    /// <see cref="ICollection{ILogger}"/>
    /// </inheritdoc>
    /// <summary>
    /// ILoggerCollection is an interface that implements the <see cref="ILogger"/> and the <see cref="ICollection{ILogger}"/> Interfaces.
    /// <para>
    /// It is supposed to log messages using all the loggers it contains.
    /// </para>
    /// </summary>
    public interface ILoggerCollection : ILogger, ICollection<ILogger>
    {
    }
}
