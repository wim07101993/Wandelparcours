using System;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    public abstract partial class ADataServiceTest
    {
        #region ONE UpdateAsync(T newItem, IEnumerable<Expression<Func<T, object>>> propertiesToUpdate = null)

        [TestMethod]
        public void UpdateNullItemAndNoProperties()
        {
            var dataService = CreateNewDataService();
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService.UpdateAsync(null).Wait();
                },
                "newItem",
                "the item to update cannot be null");
        }

        [TestMethod]
        public void UpdateNullItemAndEmptyProperties()
        {
            var dataService = CreateNewDataService();
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService
                        .UpdateAsync(null, new Expression<Func<TestEntity, object>>[] { }).Wait();
                },
                "newItem",
                "the item to update cannot be null");
        }

        [TestMethod]
        public void UpdateNullItemAndSomeProperties()
        {
            var dataService = CreateNewDataService();
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService
                        .UpdateAsync(null, new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B}).Wait();
                },
                "newItem",
                "the item to update cannot be null");
        }


        [TestMethod]
        public void UpdateUnknownItemAndNoProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    CreateNewDataService().UpdateAsync(new TestEntity()).Wait();
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateUnknownItemAndEmptyProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    CreateNewDataService()
                        .UpdateAsync(new TestEntity(), new Expression<Func<TestEntity, object>>[] { }).Wait();
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateUnKnownItemAndSomeProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    CreateNewDataService()
                        .UpdateAsync(
                            new TestEntity(),
                            new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B})
                        .Wait();
                },
                "the given entity doesn't exists");
        }


        [TestMethod]
        public void UpdateKnownItemAndNoProperties()
        {
            var dataService = CreateNewDataService();
            var originalEntity = dataService.GetFirst();
            var entity = new TestEntity {Id = originalEntity.Id };

            dataService
                .UpdateAsync(entity)
                .Wait();

            var newEntity = dataService.GetAsync(originalEntity.Id).Result;

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
        public void UpdateKnownItemAndEmptyProperties()
        {
            var dataService = CreateNewDataService();
            var originalEntity = dataService.GetFirst();
            var entity = new TestEntity {Id = originalEntity.Id};

            dataService
                .UpdateAsync(entity)
                .Wait();

            var newEntity = dataService.GetAsync(originalEntity.Id).Result;

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
                .Be(originalEntity.I, "that is the updated value");

            newEntity
                .S
                .Should()
                .Be(originalEntity.S, "that is the updated value");
        }

        [TestMethod]
        public void UpdateKnownItemAndSomeProperties()
        {
            var dataService = CreateNewDataService();
            var originalEntity = dataService.GetFirst();
            var entity = new TestEntity {Id = originalEntity.Id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B})
                .Wait();

            var newEntity = dataService.GetAsync(originalEntity.Id).Result;

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
        public void UpdatePropertyOfUnknownIdAndCorrectValue()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.B, true)
                        .Wait();
                },
                "the given entity doesn't exist");
        }

        [TestMethod]
        public void UpdateNullPropertyOfUnknownId()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), null, true)
                        .Wait();
                },
                "propertyToUpdate",
                "the property to update cannot be null");
        }

        [TestMethod]
        public void UpdatePropertyOfUnknownIdAndIncorrectValue()
        {
            ActionExtensions.ShouldCatchArgumentException(() =>
                {
                    CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.I, new {X = "not a real property"})
                        .Wait();
                },
                "value",
                "the value cannot be assigned to the property because it if of a wrong type");
        }


        [TestMethod]
        public void UpdatePropertyOfKnownIdAndCorrectValue()
        {
            var dataService = CreateNewDataService();

            dataService
                .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, 123).Wait();
        }

        [TestMethod]
        public void UpdateNullPropertyOfKnownId()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService.UpdatePropertyAsync(dataService.GetFirst().Id, null, false).Wait();
                },
                "propertyToUpdate",
                "the property to update cannot be null");
        }

        [TestMethod]
        public void UpdatePropertyOfKnownIdAndIncorrectValue()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchArgumentException(() =>
                {
                    dataService
                        .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, new {X = "not a real property"})
                        .Wait();
                },
                "value",
                "the value cannot be assigned to the property");
        }

        #endregion PROPERTY
    }
}