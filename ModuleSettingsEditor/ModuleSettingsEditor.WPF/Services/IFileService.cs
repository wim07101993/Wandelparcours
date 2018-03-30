using System.Threading.Tasks;

namespace ModuleSettingsEditor.WPF.Services
{
    public interface IFileService<T>
    {
        Task<T> OpenAsync(string path);
        Task SaveAsync(T value, string path);

        Task<T> ImportAsync(string drive);
        Task ExportAsync(T settings, string drive);
    }
}