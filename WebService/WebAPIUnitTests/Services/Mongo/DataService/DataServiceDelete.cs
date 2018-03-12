using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestMocks.Mongo;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mongo
{
    public partial class DataService
    {
        #region Remove

        [TestMethod]
        public void RemoveUnknownItem()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new MongoDataService().RemoveAsync(ObjectId.GenerateNewId()).Result;
                },
                "the given id doesn't exist");
        }

        [TestMethod]
        public void RemoveKnownItem()
        {
            var dataService = new MongoDataService();

            dataService
                .RemoveAsync(dataService.GetFirst().Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion Remove
    }
}