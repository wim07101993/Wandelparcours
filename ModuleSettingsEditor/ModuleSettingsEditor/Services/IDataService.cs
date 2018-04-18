using System.Threading.Tasks;

namespace ModuleSettingsEditor.Services
{
    public interface IDataService<T>
    {
        Task<T> GetAsync();
        Task SaveAsync(T value);
    }
}
