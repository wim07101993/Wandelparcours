using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mongo;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.Media
{
    public class TestMongoMediaService : MediaService, ITestMediaService
    {
        public TestMongoMediaService() : base(new TestMongoConfiguration("mockMedia"), new Throw())
        {
            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestMediaData);
        }

        public sealed override IMongoCollection<MediaData> MongoCollection => base.MongoCollection;

        public MediaData GetFirst()
            => MongoCollection.Find(FilterDefinition<MediaData>.Empty).FirstOrDefault().Clone();

        public IEnumerable<MediaData> GetAll()
            => MongoCollection.Find(FilterDefinition<MediaData>.Empty).ToList();
    }
}