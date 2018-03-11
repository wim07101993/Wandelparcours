using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestMocks;
using WebService.Helpers.Exceptions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
    public partial class DataService
    {
        #region Remove

        [TestMethod]
        public void RemoveMockEntityWithNonExistingID()
        {
            try
            {
                var _ = new MockDataService().RemoveAsync(ObjectId.GenerateNewId()).Result;

                Assert.Fail("There is no entity with the given id, so it should not be removed");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => x is NotFoundException)
                    .Should()
                    .BeTrue(
                        "at least one of the exceptions should be a not found exception since the item not in the database");
            }
        }

        [TestMethod]
        public void RemoveMockEntityWithExistingID()
        {
            var dataService = new MockDataService();

            dataService
                .RemoveAsync(dataService.MockData[0].Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion Remove
    }
}