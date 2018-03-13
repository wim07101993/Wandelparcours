﻿using System;
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
                    var _ = dataService.UpdateAsync(null).Result;
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
                    var _ = dataService
                        .UpdateAsync(null, new Expression<Func<TestEntity, object>>[] { }).Result;
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
                    var _ = dataService
                        .UpdateAsync(null, new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B}).Result;
                },
                "newItem",
                "the item to update cannot be null");
        }


        [TestMethod]
        public void UpdateUnknownItemAndNoProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService().UpdateAsync(new TestEntity()).Result;
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateUnknownItemAndEmptyProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService()
                        .UpdateAsync(new TestEntity(), new Expression<Func<TestEntity, object>>[] { }).Result;
                },
                "the given entity doesn't exists");
        }

        [TestMethod]
        public void UpdateUnKnownItemAndSomeProperties()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService()
                        .UpdateAsync(
                            new TestEntity(),
                            new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B})
                        .Result;
                },
                "the given entity doesn't exists");
        }


        [TestMethod]
        public void UpdateKnownItemAndNoProperties()
        {
            var dataService = CreateNewDataService();
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id).Result;
            var entity = new TestEntity {Id = id};

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
        public void UpdateKnownItemAndEmptyProperties()
        {
            var dataService = CreateNewDataService();
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id, new Expression<Func<TestEntity, object>>[] { }).Result;
            var entity = new TestEntity {Id = id};

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
            var id = dataService.GetFirst().Id;
            var originalEntity = dataService.GetAsync(id).Result;
            var entity = new TestEntity {Id = id, B = false, I = 1234, S = "abcd"};

            dataService
                .UpdateAsync(entity, new Expression<Func<TestEntity, object>>[] {x => x.I, x => x.B})
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
        public void UpdatePropertyOfUnknownIdAndCorrectValue()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.B, true)
                        .Result;
                },
                "the given entity doesn't exist");
        }

        [TestMethod]
        public void UpdateNullPropertyOfUnknownId()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), null, true)
                        .Result;
                },
                "propertyToUpdate",
                "the property to update cannot be null");
        }

        [TestMethod]
        public void UpdatePropertyOfUnknownIdAndIncorrectValue()
        {
            ActionExtensions.ShouldCatchArgumentException(() =>
                {
                    var _ = CreateNewDataService()
                        .UpdatePropertyAsync(ObjectId.GenerateNewId(), x => x.I, new {X = "not a real property"})
                        .Result;
                },
                "value",
                "the value cannot be assigned to the property because it if of a wrong type");
        }


        [TestMethod]
        public void UpdatePropertyOfKnownIdAndCorrectValue()
        {
            var dataService = CreateNewDataService();

            dataService
                .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, 123).Result
                .Should()
                .BeTrue("the item exists and value is assignable to the given property");
        }

        [TestMethod]
        public void UpdateNullPropertyOfKnownId()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = dataService.UpdatePropertyAsync(dataService.GetFirst().Id, null, false).Result;
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
                    var _ = dataService
                        .UpdatePropertyAsync(dataService.GetFirst().Id, x => x.I, new {X = "not a real property"})
                        .Result;
                },
                "value",
                "the value cannot be assigned to the property");
        }

        #endregion PROPERTY
    }
}