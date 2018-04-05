using DatabaseImporter.Helpers;

namespace DatabaseImporter.Services.Data
{
    public interface IDataServiceSelector
    {
        IDataService GetService(ESource source);
        IDataService GetService(EDestination destination);
    }
}