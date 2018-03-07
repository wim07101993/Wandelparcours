using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IReceiverModuleService : IDataService<ReceiverModule>
    {
        /// <summary>
        /// GetAsync should return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <returns>The receiver module with the given mac</returns>
        Task<ReceiverModule> GetAsync(string mac);

        /// <summary>
        /// RemoveAsync is supposed to remove the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="ReceiverModule"/> was removed from the database
        /// - false if the item was not removed
        /// </returns>
        Task<bool> RemoveAsync(string mac);
    }
}