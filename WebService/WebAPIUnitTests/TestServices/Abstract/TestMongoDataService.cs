using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebAPIUnitTests.TestModels;
using WebService.Services.Data.Mongo;

namespace WebAPIUnitTests.TestServices.Abstract
{
    public class TestMongoDataService : AMongoDataService<TestEntity>, ITestDataService<TestEntity>
    {
        public TestMongoDataService()
        {
            // create a new client and get the database from it
            var db = new MongoClient("mongodb://localhost:27017").GetDatabase("toermalienTestDb");

            // get the residents mongo collection
            MongoCollection = db.GetCollection<TestEntity>("mockEntities");

            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestEntities);
        }

        public sealed override IMongoCollection<TestEntity> MongoCollection { get; }

        public TestEntity GetFirst()
            => MongoCollection.Find(FilterDefinition<TestEntity>.Empty).FirstOrDefault();

        public IEnumerable<TestEntity> GetAll()
            => MongoCollection.Find(FilterDefinition<TestEntity>.Empty).ToList();
    }
}