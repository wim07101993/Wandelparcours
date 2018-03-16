using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices.Media;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mongo;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.Residents
{
    public class TestMongoResidentsService : ResidentsService, ITestResidentsService
    {
        public TestMongoResidentsService() : base(new TestMongoConfiguration("mockResidents"),
            new Throw(), new TestMediaService())
        {
            if (!GetAll().Any())
                MongoCollection.InsertMany(TestData.TestResidents);
        }

        public sealed override IMongoCollection<Resident> MongoCollection => base.MongoCollection;

        public Resident GetFirst()
            => MongoCollection.Find(FilterDefinition<Resident>.Empty).FirstOrDefault().Clone();

        public IEnumerable<Resident> GetAll()
            => MongoCollection.Find(FilterDefinition<Resident>.Empty).ToList();
    }
}