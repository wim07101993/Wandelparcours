using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mongo;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.ReceiverModules
{
    public class TestMongoReceiverModulesService : ReceiverModulesService, ITestReceiverModulesService
    {
        public TestMongoReceiverModulesService() : base(new TestMongoConfiguration("mockReceiverModules"), new Throw())
        {
            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestReceiverModules);
        }

        public sealed override IMongoCollection<ReceiverModule> MongoCollection => base.MongoCollection;

        public ReceiverModule GetFirst()
            => MongoCollection.Find(FilterDefinition<ReceiverModule>.Empty).FirstOrDefault().Clone();

        public IEnumerable<ReceiverModule> GetAll()
            => MongoCollection.Find(FilterDefinition<ReceiverModule>.Empty).ToList();
        
    }
}