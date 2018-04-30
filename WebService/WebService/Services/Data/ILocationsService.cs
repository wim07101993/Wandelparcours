using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface ILocationsService : IDataService<ResidentLocation>
    {
        Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since, ObjectId residentId,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null);

        Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null);
    }
}