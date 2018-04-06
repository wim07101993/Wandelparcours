using System.Collections.Generic;

namespace DatabaseImporter.Services.Data
{
    public interface IFileDataService : IDataService
    {
        string ExtensionFilter { get; }

        string Serialize<T>(IEnumerable<T> values);
        IEnumerable<T> Deserialize<T>(string str);
    }


    public interface IJsonFileService : IFileDataService
    {
    }

    public interface IXmlFileService : IFileDataService
    {
    }

    public interface ICsvFileService : IFileDataService
    {
    }
}