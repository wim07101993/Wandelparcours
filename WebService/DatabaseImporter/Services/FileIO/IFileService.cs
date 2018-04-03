using System.Threading.Tasks;
using DatabaseImporter.Models;

namespace DatabaseImporter.Services.FileIO
{
    public interface IFileService
    {
        Task<File<string>> ReadFileAsync(string extensionFilter = null);

        Task WriteFileAsync(string content, string extensionFilter = null);
    }
}