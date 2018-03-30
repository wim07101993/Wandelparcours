using System.Threading.Tasks;

namespace ModuleSettingsEditor.WPF.Services
{
    public interface IFileService<T>
    {
        Task<T> OpenAsync();
        Task SaveAsync(T value);

        Task<T> ImportAsync();
        Task ExportAsync(T settings);
    }
}