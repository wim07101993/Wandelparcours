using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestMocks.Mock;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
    public partial class DataService
    {
        #region ONE UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)

        [TestMethod]
        public void UpdateNull()
        {
            try
            {
                var _ = new MockDataService().UpdateAsync(null);
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "newItem")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the newItem parameter, that is the parameter that is null");
            }
        }

        [TestMethod]
        public void UpdateWithUnknownEntity()
        {
            try
            {
                var _ = new MockDataService().UpdateAsync(new MockEntity());
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => x is NotFoundException)
                    .Should()
                    .BeTrue(
                        "because at least one exception should be a not found exception since the entity doesn't exist");
            }
        }

        [TestMethod]
        public void UpdateWithKnownEntity()
        {
            var dataService = new MockDataService();
            var originalEntity = dataService.MockData[0].Clone();
            var entity = new MockEntity {Id = dataService.MockData[0].Id};

            dataService
                .UpdateAsync(entity)
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            dataService.MockData[0]
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            dataService.MockData[0]
                .B
                .Should()
                .Be(entity.B, "that is the updated value");

            dataService.MockData[0]
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            dataService.MockData[0]
                .S
                .Should()
                .Be(entity.S, "that is the updated value");
        }

        [TestMethod]
        public void UpdateOneField()
        {
            var dataService = new MockDataService();
            var originalEntity = dataService.MockData[0].Clone();
            var entity = new MockEntity {Id = dataService.MockData[0].Id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<MockEntity, object>>[] {x => x.I})
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            dataService.MockData[0]
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            dataService.MockData[0]
                .B
                .Should()
                .Be(originalEntity.B, "that is the updated value");

            dataService.MockData[0]
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            dataService.MockData[0]
                .S
                .Should()
                .Be(originalEntity.S, "that is the updated value");
        }

        [TestMethod]
        public void UpdateMultipleFields()
        {
            var dataService = new MockDataService();
            var originalEntity = dataService.MockData[0].Clone();
            var entity = new MockEntity {Id = dataService.MockData[0].Id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<MockEntity, object>>[] {x => x.I, x => x.B})
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            dataService.MockData[0]
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            dataService.MockData[0]
                .B
                .Should()
                .Be(entity.B, "that is the updated value");

            dataService.MockData[0]
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            dataService.MockData[0]
                .S
                .Should()
                .Be(originalEntity.S, "that is the updated value");
        }

        #endregion ONE


        #region PROPERTY UpdatePropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToUpdate, object value)

        [TestMethod]
        public void UpdateUnknownEntity()
        {
            try
            {
                var _ = new MockDataService()
                    .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.B, true)
                    .Result;

                Assert.Fail("cannot update non-existing entity");
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
        public void UpdateKnownProperty()
        {
            var dataService = new MockDataService();

            dataService
                .UpdatePropertyAsync(dataService.MockData[0].Id, x => x.I, 123).Result
                .Should()
                .BeTrue("the item exists and value is assignable to the given property");
        }

        [TestMethod]
        public void UpdateKnownPropertyWithInvalidValue()
        {
            var dataService = new MockDataService();

            try
            {
                var _ = dataService
                    .UpdatePropertyAsync(dataService.MockData[0].Id, x => x.I, new {X = "not a real property"})
                    .Result;

                Assert.Fail("cannot update non-existing entity");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentException)?.ParamName == "value")
                    .Should()
                    .BeTrue(
                        "at least one of the exceptions should be an argument exception and it's parameter name value since that is the wrong argument");
            }
        }

        [TestMethod]
        public void UpdateNullProperty()
        {
            var dataService = new MockDataService();

            try
            {
                var _ = dataService.UpdatePropertyAsync(dataService.MockData[0].Id, null, true).Result;

                Assert.Fail("cannot update non-existing entity");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "propertyToUpdate")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the propertyToUpdate parameter, that is the parameter that is null");
            }
        }

        #endregion PROPERTY
    }
}