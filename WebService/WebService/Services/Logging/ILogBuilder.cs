namespace WebService.Services.Logging
{
    public interface ILogBuilder
    {
        string BuildLogEntry(string tag, ELogLevel logLevel, string message);
    }
}