using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestMocks.Mock;
using WebAPIUnitTests.TestMocks.Mongo;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mongo
{
    public partial class DataService
    {
        #region ONE CreateAsync(T item)

        [TestMethod]
        public void CreateNullItem()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = new MongoDataService().CreateAsync(null).Result;
                },
                "item",
                "the item to create cannot be null");
        }

        [TestMethod]
        public void CreateItem()
        {
            var id = ObjectId.GenerateNewId();
            var entity = new MockEntity
            {
                Id = id,
                S = "Anna",
                B = true
            };

            var dataService = new MongoDataService();

            dataService.CreateAsync(entity).Result
                .Should()
                .BeTrue("it is assigned in the create method of the service");

            dataService.GetAll()
                .First(x => x.S == entity.S && x.B == entity.B && x.I == entity.I)
                .Id
                .Should()
                .NotBe(id);
        }

        #endregion ONE
    }
}