using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseImporter.Services.FileIO
{
    public interface IObjectWriter
    {
        Task WriteObjectToFileAsync<T>(IEnumerable<T> value);
    }
}