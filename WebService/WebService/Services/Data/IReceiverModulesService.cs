using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IReceiverModulesService : IDataService<ReceiverModule>
    {
        /// <summary>
        /// GetAsync is supposed to return the <see cref="ReceiverModule"/> with the given MAC-address from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The MAC-address is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="mac">is the MAC-address of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        Task<ReceiverModule> GetAsync(string mac, IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null);

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