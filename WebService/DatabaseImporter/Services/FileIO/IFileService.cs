using System.Threading.Tasks;
using DatabaseImporter.Models;

namespace DatabaseImporter.Services.FileIO
{
    public interface IFileService
    {
        Task<File<string>> ReadFileWithDialogAsync(string extensionFilter = null);
        Task<File<string>> ReadFileAsync(string filePath);

        Task WriteFileWithDialogsAsync(string content, string extensionFilter = null);
        Task WriteFileAsync(string filePath, string content);
    }
}