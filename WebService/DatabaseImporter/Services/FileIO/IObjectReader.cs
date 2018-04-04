using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseImporter.Models;

namespace DatabaseImporter.Services.FileIO
{
    public interface IObjectReader
    {
        Task<File<IEnumerable<T>>> ReadObjectFromFileWithDialogAsync<T>();
        Task<File<IEnumerable<T>>> ReadObjectFromFileAsync<T>(string filePath);
    }
}