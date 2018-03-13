using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mongo;

namespace WebAPIUnitTests.TestServices.ReceiverModules
{
    public class TestMongoReceiverModulesService : ReceiverModulesService, ITestReceiverModulesService
    {
        public TestMongoReceiverModulesService() : base(new TestConfiguration())
        {
            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestReceiverModules);
        }

        public sealed override IMongoCollection<ReceiverModule> MongoCollection => base.MongoCollection;

        public ReceiverModule GetFirst()
            => MongoCollection.Find(FilterDefinition<ReceiverModule>.Empty).FirstOrDefault().Clone();

        public IEnumerable<ReceiverModule> GetAll()
            => MongoCollection.Find(FilterDefinition<ReceiverModule>.Empty).ToList();


        private class TestConfiguration : IConfiguration
        {
            private IDictionary<string, string> Values { get; } = new Dictionary<string, string>
            {
                {"Database:ConnectionString", "mongodb://localhost:27017"},
                {"Database:DatabaseName", "toermalienTestDb"},
                {"Database:ReceiverModulesCollectionName", "mockReceiverModules"},
            };

            public IConfigurationSection GetSection(string key) => throw new NotImplementedException();

            public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();

            public IChangeToken GetReloadToken() => throw new NotImplementedException();

            public string this[string key]
            {
                get => Values[key];
                set => Values[key] = value;
            }
        }
    }
}