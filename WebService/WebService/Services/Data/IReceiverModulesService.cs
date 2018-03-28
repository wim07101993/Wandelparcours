using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebService.Helpers.Exceptions;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IReceiverModulesService : IDataService<ReceiverModule>
    {
        /// <summary>
        /// GetOneAsync is supposed to return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>The receiver module with the given mac</returns>
        /// <exception cref="System.ArgumentNullException">when the mac address is null</exception>
        /// <exception cref="NotFoundException">when there is no item found with the given mac address</exception>
        Task<ReceiverModule> GetAsync(string mac, IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null);

        /// <summary>
        /// Remove is supposed to remove the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <exception cref="System.ArgumentNullException">when the mac is null</exception>
        /// <exception cref="Exception">when the query was not acknowledged</exception>
        /// <exception cref="NotFoundException">when there was no item removed</exception>
        Task RemoveAsync(string mac);
    }
}