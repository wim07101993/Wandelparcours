using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseImporter.Services.Serialization
{
    public interface IObjectWriter
    {
        Task WriteObjectToFileWithDialogAsync<T>(IEnumerable<T> values);
        Task WriteObjectToFileAsync<T>(string filePath, IEnumerable<T> values);
    }
}