using System.Collections.Generic;

namespace DatabaseImporter.Services.Serialization
{
    public interface ISerializationService : IObjectReader, IObjectWriter
    {
        string Serialize<T>(IEnumerable<T> values);
        IEnumerable<T> Deserialize<T>(string json);
    }
}