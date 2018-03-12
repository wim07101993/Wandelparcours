using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests
{
    public abstract partial class ADataService
    {
        #region Remove

        [TestMethod]
        public void RemoveUnknownItem()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService().RemoveAsync(ObjectId.GenerateNewId()).Result;
                },
                "the given id doesn't exist");
        }

        [TestMethod]
        public void RemoveKnownItem()
        {
            var dataService = CreateNewDataService();

            dataService
                .RemoveAsync(dataService.GetFirst().Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion Remove
    }
}
