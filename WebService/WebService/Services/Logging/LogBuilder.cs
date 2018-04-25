namespace WebService.Services.Logging
{
    public class LogBuilder : ILogBuilder
    {
        public string BuildLogEntry(string tag, ELogLevel logLevel, string message)
            => $"{logLevel.ToString().ToUpper()}:{tag}->{message}";
    }
}