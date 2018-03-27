using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class LocationService : ILocationService
    {
        public Task<object> GetPropertyAsync(ObjectId id, Expression<Func<Location, object>> propertyToSelect)
        {
            throw new NotImplementedException();
        }

        public Task<Location> GetAsync(ObjectId id, IEnumerable<Expression<Func<Location, object>>> propertiesToInclude = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Location>> GetAsync(IEnumerable<Expression<Func<Location, object>>> propertiesToInclude = null)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Location item)
        {
            throw new NotImplementedException();
        }

        public Task AddItemToListProperty(ObjectId id, Expression<Func<Location, IEnumerable<object>>> propertyToAddItemTo, object itemToAdd)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Location newItem, IEnumerable<Expression<Func<Location, object>>> propertiesToUpdate = null)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePropertyAsync<TValue>(ObjectId id, Expression<Func<Location, TValue>> propertyToUpdate, TValue value)
        {
            throw new NotImplementedException();
        }
    }
}