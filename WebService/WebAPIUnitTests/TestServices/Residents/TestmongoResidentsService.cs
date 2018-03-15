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
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.Residents
{
    public class TestMongoResidentsService : ResidentsService, ITestResidentsService
    {
        public TestMongoResidentsService() : base(new TestConfiguration(), new Throw())
        {
            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestResidents);
        }

        public sealed override IMongoCollection<Resident> MongoCollection => base.MongoCollection;

        public Resident GetFirst()
            => MongoCollection.Find(FilterDefinition<Resident>.Empty).FirstOrDefault().Clone();

        public IEnumerable<Resident> GetAll()
            => MongoCollection.Find(FilterDefinition<Resident>.Empty).ToList();


        private class TestConfiguration : IConfiguration
        {
            private IDictionary<string, string> Values { get; } = new Dictionary<string, string>
            {
                {"Database:ConnectionString", "mongodb://localhost:27017"},
                {"Database:DatabaseName", "toermalienTestDb"},
                {"Database:ResidentsCollectionName", "mockResidents"},
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