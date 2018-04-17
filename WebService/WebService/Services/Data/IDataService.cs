using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models.Bases;

namespace WebService.Services.Data
{
    public interface IDataService<T> where T : IModelWithID
    {
        Task CreateAsync(T item);

        Task AddItemToListProperty<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> propertyToAddItemTo,
            TValue itemToAdd);

        
        Task<TOut> GetPropertyAsync<TOut>(ObjectId id, Expression<Func<T, TOut>> propertyToSelect);

        Task<T> GetOneAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);

        Task<IEnumerable<T>> GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null);


        Task UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null);
        
        Task ReplaceAsync(T newItem);

        Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<T, TValue>> propertyToUpdate, TValue value);

        
        Task RemoveAsync(ObjectId id);

        Task RemoveItemFromList<TValue>(ObjectId id, Expression<Func<T, IEnumerable<TValue>>> popertyToRemoveItemFrom,
            TValue itemToRemove);
 }
}