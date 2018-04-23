using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Models;


namespace WebService.Services.Data.Mongo
{
    public class LocationService : AMongoDataService<ResidentLocation>, ILocationService
    {
        public LocationService(IConfiguration config)
            : base(config["Database:ConnectionString"], config["Database:DatabaseName"],
                config["Database:LocationsCollectionName"])
        {
        }


        public async Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since, ObjectId residentId,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null)
        {
            var filter = since == default(DateTime)
                ? (Expression<Func<ResidentLocation, bool>>) (x => x.Id == residentId)
                : (x => x.Id == residentId && x.TimeStamp >= since);

            var foundItems = MongoCollection.Find(filter);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<ResidentLocation>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<ResidentLocation>(selector)
                .ToList();
        }

        public async Task<IEnumerable<ResidentLocation>> GetSinceAsync(DateTime since,
            IEnumerable<Expression<Func<ResidentLocation, object>>> propertiesToInclude = null)
        {
            var filter = since == default(DateTime)
                ? FilterDefinition<ResidentLocation>.Empty
                : new ExpressionFilterDefinition<ResidentLocation>(x => x.TimeStamp >= since);

            var foundItems = MongoCollection.Find(filter);

            if (propertiesToInclude == null)
                return await foundItems.ToListAsync();

            var selector = Builders<ResidentLocation>.Projection.Include(x => x.Id);
            selector = propertiesToInclude.Aggregate(selector, (current, property) => current.Include(property));

            return foundItems
                .Project<ResidentLocation>(selector)
                .ToList();
        }
    }
}