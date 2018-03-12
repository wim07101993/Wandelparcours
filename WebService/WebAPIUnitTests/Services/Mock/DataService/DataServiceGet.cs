using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestMocks;
using WebAPIUnitTests.TestMocks.Mock;
using WebService.Helpers.Exceptions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
    public partial class DataService
    {
        #region ALL GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)

        [TestMethod]
        public void GetWithOutPropertiesToInclude()
        {
            var dataService = new MockDataService();

            dataService.GetAsync().Result
                .Should()
                .BeEquivalentTo(dataService.MockData, "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetWithIdPropertyToInclude()
        {
            var dataService = new MockDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<MockEntity, object>>[] {x => x.Id})
                .Result
                .ToList();

            var emptyEntity = new MockEntity();

            for (var i = 0; i < mockEntities.Count; i++)
            {
                mockEntities[i]
                    .Id
                    .Should()
                    .Be(dataService.MockData[i].Id, "it should be the same object and the object id is always passed");

                mockEntities[i]
                    .S
                    .Should()
                    .Be(emptyEntity.S, "it is not asked in the selector");

                mockEntities[i]
                    .I
                    .Should()
                    .Be(emptyEntity.I, "it is not asked in the selector");

                mockEntities[i]
                    .B
                    .Should()
                    .Be(emptyEntity.B, "it is not asked in the selector");
            }
        }

        [TestMethod]
        public void GetWithSomeFields()
        {
            var dataService = new MockDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<MockEntity, object>>[] {x => x.S, x => x.I})
                .Result
                .ToList();

            var emptyEntity = new MockEntity();

            for (var i = 0; i < mockEntities.Count; i++)
            {
                mockEntities[i]
                    .Id
                    .Should()
                    .Be(dataService.MockData[i].Id, "it should be the same object and the object id is always passed");

                mockEntities[i]
                    .S
                    .Should()
                    .Be(dataService.MockData[i].S, "it is asked in the selector");

                mockEntities[i]
                    .I
                    .Should()
                    .Be(dataService.MockData[i].I, "it is asked in the selector");

                mockEntities[i]
                    .B
                    .Should()
                    .Be(emptyEntity.B, "it is not asked in the selector");
            }
        }

        #endregion ALL


        #region ONE GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)

        [TestMethod]
        public void GetOneWithUnknownIdAndNoPropertiesToInclude()
        {
            try
            {
                var _ = new MockDataService()
                    .GetAsync(ObjectId.GenerateNewId())
                    .Result;

                Assert.Fail("the id could not be found so an exception should be thrown");
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
        public void GetOneWithUnknownIdAndIdPropertiesToInclude()
        {
            try
            {
                var _ = new MockDataService()
                    .GetAsync(ObjectId.GenerateNewId(), new Expression<Func<MockEntity, object>>[] {x => x.Id}).Result;

                Assert.Fail("the id could not be found so an exception should be thrown");
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
        public void GetOneWithUnknownIdAndSomeFields()
        {
            try
            {
                var _ = new MockDataService()
                    .GetAsync(ObjectId.GenerateNewId(), new Expression<Func<MockEntity, object>>[] {x => x.S, x => x.I})
                    .Result;

                Assert.Fail("the id could not be found so an exception should be thrown");
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
            var dataService = new MockDataService();

            dataService
                .GetAsync(dataService.MockData[0].Id).Result
                .Should()
                .Be(dataService.MockData[0], "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetOneWithIdPropertyToInclude()
        {
            var dataService = new MockDataService();

            var mockEntity = dataService
                .GetAsync(dataService.MockData[0].Id, new Expression<Func<MockEntity, object>>[] {x => x.Id})
                .Result;

            var emptyEntity = new MockEntity();

            mockEntity
                .Id
                .Should()
                .Be(dataService.MockData[0].Id, "it should be the same object and the object id is always passed");

            mockEntity
                .S
                .Should()
                .Be(emptyEntity.S, "it is not asked in the selector");

            mockEntity
                .I
                .Should()
                .Be(emptyEntity.I, "it is not asked in the selector");

            mockEntity
                .B
                .Should()
                .Be(emptyEntity.B, "it is not asked in the selector");
        }

        [TestMethod]
        public void GetOneWithSomeFields()
        {
            var dataService = new MockDataService();

            var mockEntity = dataService
                .GetAsync(dataService.MockData[0].Id, new Expression<Func<MockEntity, object>>[] {x => x.S, x => x.I})
                .Result;

            var emptyEntity = new MockEntity();

            mockEntity
                .Id
                .Should()
                .Be(dataService.MockData[0].Id, "it should be the same object and the object id is always passed");

            mockEntity
                .S
                .Should()
                .Be(dataService.MockData[0].S, "it is asked in the selector");

            mockEntity
                .I
                .Should()
                .Be(dataService.MockData[0].I, "it is asked in the selector");

            mockEntity
                .B
                .Should()
                .Be(emptyEntity.B, "it is not asked in the selector");
        }

        #endregion ONE


        #region PROPERTY GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)

        [TestMethod]
        public void GetPropertyWithUnKnownId()
        {
            try
            {
                var _ = new MockDataService()
                    .GetPropertyAsync(ObjectId.GenerateNewId(), x => x.B)
                    .Result;

                Assert.Fail("the id could not be found so an exception should be thrown");
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
        public void GetPropertyWithoutSelector()
        {
            var dataService = new MockDataService();

            try
            {
                var _ = dataService.GetPropertyAsync(dataService.MockData[0].Id, null).Result;

                Assert.Fail("the property could not be found so an exception should be thrown");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "propertyToSelect")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the item parameter, that is the parameter that is null");
            }
        }

        [TestMethod]
        public void GetProperty()
        {
            var dataService = new MockDataService();

            dataService
                .GetPropertyAsync(dataService.MockData[0].Id, x => x.B).Result
                .Should()
                .Be(dataService.MockData[0].B, "the id should exist and that is the asked property");

            dataService
                .GetPropertyAsync(dataService.MockData[0].Id, x => x.I).Result
                .Should()
                .Be(dataService.MockData[0].I, "the id should exist and that is the asked property");

            dataService
                .GetPropertyAsync(dataService.MockData[0].Id, x => x.S).Result
                .Should()
                .Be(dataService.MockData[0].S, "the id should exist and that is the asked property");
        }

        #endregion PROPERTY
    }
}