using System.Threading.Tasks;

namespace DatabaseImporter.Services.FileService
{
    public interface IFileService<T>
    {
        Task<T> OpenJsonAsync(string path);
        Task SaveJsonAsync(T value, string path);

        Task<T> OpenCsvAsync(string path);
        Task SaveCsvAsync(T value, string path);
    }
}