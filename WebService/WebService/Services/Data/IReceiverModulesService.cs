using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Interface that descibes a class that provides basic CURD operations for the <see cref="ReceiverModule" /> in the
    /// database.
    /// </summary>
    public interface IReceiverModulesService : IDataService<ReceiverModule>
    {
        /// <summary>
        /// Gets a slingle <see cref="ReceiverModule"/> from the database by <see cref="ReceiverModule.Name"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="ReceiverModule"/> to get from the database</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The <see cref="ReceiverModule"/> with the given name.</returns>
        Task<ReceiverModule> GetOneAsync(string name,
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null);

        /// <summary>
        /// Removes a single <see cref="ReceiverModule"/> from the database by <see cref="ReceiverModule.Name"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="ReceiverModule"/> to remove from the database</param>
        Task RemoveAsync(string name);
    }
}