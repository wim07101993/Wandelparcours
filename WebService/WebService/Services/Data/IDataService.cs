using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models.Bases;

namespace WebService.Services.Data
{
    /// <summary>
    /// IDataService that defines the methods to communicate with a database.
    /// </summary>
    public interface IDataService<T> where T : IModelWithID
    {
        /// <summary>
        /// GetAsync is supposed to return the <see cref="T"/> with the given id from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="id">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        Task<T> GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);

        /// <summary>
        /// GetAsync is supposed to return all the ts from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the ts in the database.</returns>
        Task<IEnumerable<T>> GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);

        /// <summary>
        /// CreateAsync is supposed to save the passed <see cref="T"/> to the database.
        /// <para/>
        /// If the item is created, the method should return the id of the new <see cref="T"/>, else null.
        /// </summary>
        /// <param name="item">is the <see cref="T"/> to save in the database</param>
        /// <returns>
        /// - the new id if the <see cref="T"/> was created in the database
        /// - null if the item was not created
        /// </returns>
        Task<string> CreateAsync(T item);

        /// <summary>
        /// RemoveAsync is supposed to remove the <see cref="T"/> with the given id from the database.
        /// </summary>
        /// <param name="id">is the id of the <see cref="T"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="T"/> was removed from the database
        /// - false if the item was not removed
        /// </returns>
        Task<bool> RemoveAsync(ObjectId id);

        /// <summary>
        /// UpdateAsync is supposed to update the <see cref="T"/> with the id of the given <see cref="T"/>.
        /// <para/>
        /// The updated properties are defined in the <see cref="propertiesToUpdate"/> parameter.
        /// If the <see cref="propertiesToUpdate"/> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newItem">is the <see cref="T"/> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated item</returns>
        Task<T> UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null);
    }
}