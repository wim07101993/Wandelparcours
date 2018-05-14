using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Interface that describes a class that provides basic CRUD operations for <see cref="User" /> in a database.
    /// </summary>
    public interface IUsersService : IDataService<User>
    {
        /// <summary>
        /// Checks whether the the password of corresponds with the passed id.
        /// </summary>
        /// <param name="id">Id of the <see cref="User"/> to check the password for</param>
        /// <param name="password">Password of the <see cref="User"/> to check</param>
        /// <returns>True if the password is correct, else false.</returns>
        Task<bool> CheckCredentialsAsync(ObjectId id, string password);
        
        /// <summary>
        /// Updates the password of a <see cref="User"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="User"/> to update the password of</param>
        /// <param name="password">The new password</param>
        Task UpdatePasswordAsync(ObjectId id, string password);

        /// <summary>
        /// Gets a <see cref="User"/> from the database by name
        /// </summary>
        /// <param name="userName">Name of the <see cref="User"/> to get.</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns></returns>
        Task<User> GetByNameAsync(string userName,
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets a property of a <see cref="User"/> by name.
        /// </summary>
        /// <param name="userName">Name of the <see cref="User"/> to get the property of.</param>
        /// <param name="propertyToSelect">The property to get</param>
        /// <typeparam name="T">Type of the property to get</typeparam>
        /// <returns>The value of the property</returns>
        Task<T> GetPropertyByNameAsync<T>(string userName, Expression<Func<User, T>> propertyToSelect = null);
    }
}