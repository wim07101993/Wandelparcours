using System;
using System.Linq;
using System.Linq.Expressions;
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
        public void GetOneWithNullMac()
        {
            try
            {
                var _ = new MockReceiverModulesService().GetAsync(null).Result;

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
        public void GetOneWithUnknownMacAndNoPropertiesToInclude()
        {
            try
            {
                var _ = new MockReceiverModulesService()
                    .GetAsync("")
                    .Result;

                Assert.Fail("the mac could not be found so an exception should be thrown");
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
        public void GetOneWithUnknownMacAndMacPropertiesToInclude()
        {
            try
            {
                var _ = new MockReceiverModulesService()
                    .GetAsync("", new Expression<Func<ReceiverModule, object>>[] {x => x.Mac}).Result;

                Assert.Fail("the mac could not be found so an exception should be thrown");
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
        public void GetOneWithUnknownMacAndSomeFields()
        {
            try
            {
                var _ = new MockReceiverModulesService()
                    .GetAsync("", new Expression<Func<ReceiverModule, object>>[] {x => x.IsActive, x => x.Mac})
                    .Result;

                Assert.Fail("the mac could not be found so an exception should be thrown");
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
        public void GetOneWithOutPropertiesToInclude()
        {
            var dataService = new MockReceiverModulesService();

            dataService
                .GetAsync(dataService.MockData[0].Mac).Result
                .Should()
                .Be(dataService.MockData[0], "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetOneWithMacPropertyToInclude()
        {
            var dataService = new MockReceiverModulesService();

            var mockEntity = dataService
                .GetAsync(dataService.MockData[0].Mac, new Expression<Func<ReceiverModule, object>>[] {x => x.Mac})
                .Result;

            var emptyEntity = new ReceiverModule();

            mockEntity
                .Mac
                .Should()
                .Be(dataService.MockData[0].Mac, "it should be the same object and the object mac is always passed");

            mockEntity
                .IsActive
                .Should()
                .Be(emptyEntity.IsActive, "it is not asked in the selector");

            mockEntity
                .Position
                .Should()
                .Be(emptyEntity.Position, "it is not asked in the selector");
        }

        [TestMethod]
        public void GetOneWithSomeFields()
        {
            var dataService = new MockReceiverModulesService();

            var mockEntity = dataService
                .GetAsync(dataService.MockData[0].Mac,
                    new Expression<Func<ReceiverModule, object>>[] {x => x.IsActive, x => x.Mac})
                .Result;

            var emptyEntity = new ReceiverModule();

            mockEntity
                .Mac
                .Should()
                .Be(dataService.MockData[0].Mac, "it should be the same object and the object mac is always passed");

            mockEntity
                .IsActive
                .Should()
                .Be(dataService.MockData[0].IsActive, "it is asked in the selector");

            mockEntity
                .Position
                .Should()
                .Be(emptyEntity.Position, "it is asked in the selector");
        }
    }
}