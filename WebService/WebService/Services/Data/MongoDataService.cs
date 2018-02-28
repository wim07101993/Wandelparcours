using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers;
using WebService.Models;

namespace WebService.Services.Data
{
    public class MongoDataService : IDataService
    {
        #region FIELDS

        private readonly IMongoCollection<Resident> _collection;

        #endregion FIELDS


        #region CONSTRUCTORS

        public MongoDataService(IConfiguration config)
        {
            _collection = new MongoClient(config["Database:ConnectionString"])
                .GetDatabase(config["Database:DatabaseName"])
                .GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
        }

        #endregion CONSTRUCTORS

        public IEnumerable<Resident> GetResidents(
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var foundItems = _collection.Find(FilterDefinition<Resident>.Empty);

            var properties = propertiesToInclude?.ToList();
            if (EnumerableExtensions.IsNullOrEmpty(properties))
                return foundItems.ToList();

            var selector = Builders<Resident>.Projection.Include(x => x.ID);

            // ReSharper disable once PossibleNullReferenceException
            foreach (var property in properties)
                selector.Include(property);

            return foundItems.Project<Resident>(selector).ToList();
        }

        public bool CreateResident(Resident resident)
        {
            resident.ID = ObjectId.GenerateNewId();
            _collection.InsertOne(resident);

            var newItem = _collection
                .Find(x => x.ID == resident.ID)
                .FirstOrDefault();

            return newItem != null;
        }
    }
}