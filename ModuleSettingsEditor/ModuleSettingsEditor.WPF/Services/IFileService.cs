using System.Threading.Tasks;

namespace ModuleSettingsEditor.WPF.Services
{
    public interface IFileService<T>
    {
        string ExportDir { get; }

        Task<T> OpenAsync(string path);
        Task SaveAsync(T value, string path);

        Task<T> ImportAsync(string drive);
        Task ExportAsync(T value, string drive);
    }
}