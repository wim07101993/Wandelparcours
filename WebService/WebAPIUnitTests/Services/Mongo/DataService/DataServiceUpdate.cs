using System;
using System.Linq.Expressions;
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
        #region ONE UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)

        [TestMethod]
        public void UpdateNull()
        {
            var dataService = new MongoDataService();
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = dataService.UpdateAsync(null).Result;
                },
                "newItem",
                "the item to update cannot be null");
        }

        [TestMethod]
        public void UpdateWithUnknownEntity()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new MongoDataService().UpdateAsync(new MockEntity()).Result;
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateWithUnknownEntityAndSomeFields()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new MongoDataService()
                        .UpdateAsync(
                            new MockEntity(),
                            new Expression<Func<MockEntity, object>>[] {x => x.I})
                        .Result;
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateWithKnownEntity()
        {
            var dataService = new MongoDataService();
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id).Result;
            var entity = new MockEntity {Id = id};

            dataService
                .UpdateAsync(entity)
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            var newEntity = dataService.GetAsync(id).Result;

            newEntity
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            newEntity
                .B
                .Should()
                .Be(entity.B, "that is the updated value");

            newEntity
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            newEntity
                .S
                .Should()
                .Be(entity.S, "that is the updated value");
        }

        [TestMethod]
        public void UpdateOneField()
        {
            var dataService = new MongoDataService();
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id).Result;
            var entity = new MockEntity {Id = id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<MockEntity, object>>[] {x => x.I})
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            var newEntity = dataService.GetAsync(id).Result;

            newEntity
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            newEntity
                .B
                .Should()
                .Be(originalEntity.B, "that is the updated value");

            newEntity
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            newEntity
                .S
                .Should()
                .Be(originalEntity.S, "that is the updated value");
        }

        [TestMethod]
        public void UpdateMultipleFields()
        {
            var dataService = new MongoDataService();
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id).Result;
            var entity = new MockEntity {Id = id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<MockEntity, object>>[] {x => x.I, x => x.B})
                .Result
                .Should()
                .BeTrue("the entity should be able to update");

            var newEntity = dataService.GetAsync(id).Result;

            newEntity
                .Id
                .Should()
                .Be(originalEntity.Id, "the id of the item should not change");

            newEntity
                .B
                .Should()
                .Be(entity.B, "that is the updated value");

            newEntity
                .I
                .Should()
                .Be(entity.I, "that is the updated value");

            newEntity
                .S
                .Should()
                .Be(originalEntity.S, "that is the updated value");
        }

        #endregion ONE


        #region PROPERTY UpdatePropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToUpdate, object value)

        [TestMethod]
        public void UpdateUnknownEntity()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new MongoDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.B, true)
                        .Result;
                },
                "the given entity doesn't exist");
        }

        [TestMethod]
        public void UpdateKnownProperty()
        {
            var dataService = new MongoDataService();

            dataService
                .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, 123).Result
                .Should()
                .BeTrue("the item exists and value is assignable to the given property");
        }

        [TestMethod]
        public void UpdateKnownPropertyWithInvalidValue()
        {
            var dataService = new MongoDataService();

            ActionExtensions.ShouldCatchArgumentException(() =>
                {
                    var _ = dataService
                        .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, new {X = "not a real property"})
                        .Result;
                },
                "value",
                "the value cannot be assigned to the property");
        }

        [TestMethod]
        public void UpdateNullProperty()
        {
            var dataService = new MongoDataService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = dataService.UpdatePropertyAsync(dataService.GetFirst().Id, null, true).Result;
                },
                "propertyToUpdate",
                "the property to update cannot be null");
        }

        #endregion PROPERTY
    }
}