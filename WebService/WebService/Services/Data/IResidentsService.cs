using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IResidentsService : IDataService<Resident>
    {
        Task AddMediaAsync(ObjectId residentId, string title, byte[] data, EMediaType mediaType,
            string extension = null);

        Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType);


        Task<Resident> GetOneAsync(int tag, IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        Task<TValue> GetPropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToSelect);

        Task<int> GetHighestTagNumberAsync();


        Task UpdatePropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToUpdate, TValue value);


        Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType);

        Task RemoveSubItemAsync(ObjectId residentId, Expression<Func<Resident, IEnumerable<object>>> selector,
            object item);
    }
}