using System.Collections.Generic;

namespace WebService.Services.Logging
{
    public interface ILoggerCollection : ILogger, ICollection<ILogger>
    {
    }
}