using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Services.Data.Mock;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
   public partial class ReceiverModulesService
    {
        [TestMethod]
        public void RemoveMockEntityWithNullID()
        {
            try
            {
                var _ = new MockReceiverModulesService().RemoveAsync(null).Result;

                Assert.Fail("there can't be any receiver modules with a null mac");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "mac")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the mac parameter, that is the parameter that is null");
            }
        }

        [TestMethod]
        public void RemoveMockEntityWithNonExistingID()
        {
            try
            {
                var _ = new MockReceiverModulesService().RemoveAsync("").Result;

                Assert.Fail($"There is no {typeof(ReceiverModule).Name} with the given mac, so it should not be removed");
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
            var dataService = new MockReceiverModulesService();

            dataService
                .RemoveAsync(dataService.MockData[0].Id).Result
                .Should()
                .BeTrue("the item exist");
        }

    }
}
