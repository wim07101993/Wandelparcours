using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebAPIUnitTests.TestMocks.Mock;
using WebService.Services.Data.Mongo;

namespace WebAPIUnitTests.TestMocks.Mongo
{
    public class MongoDataService : AMongoDataService<MockEntity>, IMockDataService
    {
        public MongoDataService()
        {
            // create a new client and get the database from it
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("toermalienTestDb");

            // get the residents mongo collection
            MongoCollection = db.GetCollection<MockEntity>("mockEntities");

            if (!GetAll().Any())
                MongoCollection.InsertMany(MockData.MockEntities);
        }

        public sealed override IMongoCollection<MockEntity> MongoCollection { get; }

        public MockEntity GetFirst()
            => MongoCollection.Find(FilterDefinition<MockEntity>.Empty).FirstOrDefault();

        public IEnumerable<MockEntity> GetAll()
            => MongoCollection.Find(FilterDefinition<MockEntity>.Empty).ToList();
    }
}