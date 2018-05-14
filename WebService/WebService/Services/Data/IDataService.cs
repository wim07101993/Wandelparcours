using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models.Bases;

namespace WebService.Services.Data
{
    public interface IDataService<T>
        where T : IModelWithID
    {
        /// <summary>
        /// Creates an item of type T in the database.
        /// </summary>
        /// <param name="item">The item to create in the database</param>
        Task CreateAsync(T item);

        /// <summary>
        /// Adds an item to a nested list in an item to the database.
        /// </summary>
        /// <param name="id">Id of the item to add the nested item to</param>
        /// <param name="propertyToAddItemTo">Property to add the nested item to</param>
        /// <param name="itemToAdd">Item to add to the nested list</param>
        /// <typeparam name="TValue">The type of the nested item</typeparam>
        Task AddItemToListProperty<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo,
            TValue itemToAdd);

        
        /// <summary>
        /// Gets a property from a specific item in the database.
        /// </summary>
        /// <param name="id">Id of the item to get the property from</param>
        /// <param name="propertyToSelect">The property to get from the item</param>
        /// <typeparam name="TOut">The type of the property to get</typeparam>
        /// <returns>The property from the item with id <see cref="id"/></returns>
        Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect);

        /// <summary>
        /// Gets a single entity from the database by id.
        /// </summary>
        /// <param name="id">Id of the item to get</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The entity with the given id</returns>
        Task<T> GetOneAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets all entities from the database.
        /// </summary>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>All entities in the database</returns>
        Task<IEnumerable<T>> GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);


        /// <summary>
        /// Updates an entity by selecting the properties passed in <see cref="propertiesToUpdate"/>
        /// and replace the old properties in the database. The item to update is selected by comparing the id of
        /// <see cref="newItem"/>.
        /// </summary>
        /// <param name="newItem">The new item to get the to update properties from</param>
        /// <param name="propertiesToUpdate">The properties to update</param>
        Task UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null);

        /// <summary>
        /// Replaces an item in the databse. The item to replace is selected by comparing the id of <see cref="newItem"/>
        /// </summary>
        /// <param name="newItem">Item to replace the old one with.</param>
        /// <returns></returns>
        Task ReplaceAsync(T newItem);

        /// <summary>
        /// Updates a property of an item.
        /// </summary>
        /// <param name="id">Id of the item to update the property of.</param>
        /// <param name="propertyToUpdate">The property of to update.</param>
        /// <param name="value">The value to update the property to.</param>
        /// <typeparam name="TValue">The type of the property to update</typeparam>
        Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate, TValue value);


        /// <summary>
        /// Removes an item in the database by id.
        /// </summary>
        /// <param name="id">Id of the item to remove.</param>
        Task RemoveAsync(ObjectId id);

        /// <summary>
        /// Removes an item from a nested list of a specific item in the database.
        /// </summary>
        /// <param name="id">Id of the item to remove the nested item from</param>
        /// <param name="popertyToRemoveItemFrom">Property to remove the nested item from</param>
        /// <param name="itemToRemove">Item to remove from the nested list</param>
        /// <typeparam name="TValue">Type of the nested item</typeparam>
        Task RemoveItemFromList<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> popertyToRemoveItemFrom,
            TValue itemToRemove);
    }
}