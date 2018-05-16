using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models.Bases;

namespace WebService.Services.Data
{
    /// <summary>
    /// Interface that describes a class that provides basic CRUD operations for a database.
    /// </summary>
    /// <typeparam name="T">Type of the item to preform the CRUD operation on.</typeparam>
    public interface IDataService<T>
        where T : IModelWithID
    {
        /// <summary>
        /// Creates a new <see cref="T"/> in the database.
        /// </summary>
        /// <param name="item">The <see cref="T"/> to create in the database</param>
        Task CreateAsync(T item);

        /// <summary>
        /// Adds a new <see cref="TValue"/> to a nested list in a <see cref="T"/> to the database.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to add the <see cref="TValue"/> to</param>
        /// <param name="propertyToAddItemTo">Property to add the <see cref="TValue"/> to</param>
        /// <param name="itemToAdd"><see cref="TValue"/> to add to the nested list</param>
        /// <typeparam name="TValue">The type of the nested item</typeparam>
        Task AddItemToListProperty<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo,
            TValue itemToAdd);

        
        /// <summary>
        /// Gets a property from a specific <see cref="T"/> in the database.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to get the property from</param>
        /// <param name="propertyToSelect">The property to get from the <see cref="T"/></param>
        /// <typeparam name="TOut">The type of the property to get</typeparam>
        /// <returns>The property from the <see cref="T"/> with id <see cref="id"/></returns>
        Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect);

        /// <summary>
        /// Gets a single <see cref="T"/> from the database by id.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to get</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The <see cref="T"/> with the given id</returns>
        Task<T> GetOneAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets all <see cref="T"/>s from the database.
        /// </summary>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>All <see cref="T"/>s in the database</returns>
        Task<IEnumerable<T>> GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);


        /// <summary>
        /// Updates a <see cref="T"/> by selecting the properties passed in <see cref="propertiesToUpdate"/>
        /// and replace the old properties in the database. The <see cref="T"/> to update is selected by comparing
        /// the id of <see cref="newItem"/>.
        /// </summary>
        /// <param name="newItem">The new <see cref="T"/> to get the to update properties from</param>
        /// <param name="propertiesToUpdate">The properties to update</param>
        Task UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null);

        /// <summary>
        /// Replaces a <see cref="T"/> in the databse. The <see cref="T"/> to replace is selected by comparing the
        /// id of <see cref="newItem"/>
        /// </summary>
        /// <param name="newItem"><see cref="T"/> to replace the old one with.</param>
        /// <returns></returns>
        Task ReplaceAsync(T newItem);

        /// <summary>
        /// Updates a property of a <see cref="T"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to update the property of.</param>
        /// <param name="propertyToUpdate">The property of to update.</param>
        /// <param name="value">The value to update the property to.</param>
        /// <typeparam name="TValue">The type of the property to update</typeparam>
        Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate, TValue value);


        /// <summary>
        /// Removes a <see cref="T"/> in the database by id.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to remove.</param>
        Task RemoveAsync(ObjectId id);

        /// <summary>
        /// Removes a <see cref="TValue"/> from a nested list of a specific <see cref="T"/> in the database.
        /// </summary>
        /// <param name="id">Id of the <see cref="T"/> to remove the <see cref="TValue"/> from</param>
        /// <param name="popertyToRemoveItemFrom">Property to remove the <see cref="TValue"/> from</param>
        /// <param name="itemToRemove"><see cref="TValue"/> to remove from the nested list</param>
        /// <typeparam name="TValue">Type of the nested item</typeparam>
        Task RemoveItemFromList<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> popertyToRemoveItemFrom,
            TValue itemToRemove);
    }
}