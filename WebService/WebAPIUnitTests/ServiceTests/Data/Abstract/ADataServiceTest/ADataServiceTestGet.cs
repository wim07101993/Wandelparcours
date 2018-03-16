using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Exceptions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    public abstract partial class ADataServiceTest
    {
        #region ALL GetAsync(IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)

        [TestMethod]
        public void GetAllWithoutPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            dataService.GetAsync().Result
                .Should()
                .BeEquivalentTo(dataService.GetAll(), "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetAllWithEmptyPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<TestEntity, object>>[] { })
                .Result
                .ToList();

            var emptyEntity = new TestEntity();

            foreach (var entity in mockEntities)
            {
                entity
                    .Id
                    .Should()
                    .Be(dataService.GetAsync(entity.Id).Result.Id,
                        "it should be the same object and the object id is always passed");

                entity
                    .S
                    .Should()
                    .Be(emptyEntity.S, "it is not asked in the selector");

                entity
                    .I
                    .Should()
                    .Be(emptyEntity.I, "it is not asked in the selector");

                entity
                    .B
                    .Should()
                    .Be(emptyEntity.B, "it is not asked in the selector");
            }
        }

        [TestMethod]
        public void GetAllWithSomePropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<TestEntity, object>>[] {x => x.S, x => x.I})
                .Result
                .ToList();

            var emptyEntity = new TestEntity();

            foreach (var entity in mockEntities)
            {
                var foundEntity = dataService.GetAsync(entity.Id).Result;

                entity
                    .Id
                    .Should()
                    .Be(foundEntity.Id, "it should be the same object and the object id is always passed");

                entity
                    .S
                    .Should()
                    .Be(foundEntity.S, "it is asked in the selector");

                entity
                    .I
                    .Should()
                    .Be(foundEntity.I, "it is asked in the selector");

                entity
                    .B
                    .Should()
                    .Be(emptyEntity.B, "it is not asked in the selector");
            }
        }

        #endregion ALL

        #region ONE GetAsync(ObjectId id, IEnumerable<Expression<Func<T, object>>> propertiesToInclude = null)

        [TestMethod]
        public void GetOneWithUnknownIdAndNullPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService().GetAsync(ObjectId.GenerateNewId()).Wait(),
                "the given id doesn't exist");
        }

        [TestMethod]
        public void GetOneWithUnknownIdAndEmptyPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService()
                    .GetAsync(ObjectId.GenerateNewId(), new Expression<Func<TestEntity, object>>[] { })
                    .Wait(),
                "there is no entity with the given id");
        }

        [TestMethod]
        public void GetOneWithUnknownIdAndSomePropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService()
                    .GetAsync(ObjectId.GenerateNewId(),
                        new Expression<Func<TestEntity, object>>[] {x => x.S, x => x.I})
                    .Wait(),
                "there is no entity with the given id");
        }

        [TestMethod]
        public void GetOneWithKnownIdAndNoPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            dataService
                .GetAsync(dataService.GetFirst().Id).Result
                .Should()
                .BeEquivalentTo(dataService.GetFirst(), "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetOneWithKnownIdAndEmptyPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var mockEntity = dataService
                .GetAsync(dataService.GetFirst().Id, new Expression<Func<TestEntity, object>>[] { })
                .Result;

            var emptyEntity = new TestEntity();

            mockEntity
                .Id
                .Should()
                .Be(dataService.GetFirst().Id, "it should be the same object and the object id is always passed");

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
        public void GetOneWithKnownIdAndSomePropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var mockEntity = dataService
                .GetAsync(dataService.GetFirst().Id,
                    new Expression<Func<TestEntity, object>>[] {x => x.S, x => x.I})
                .Result;

            var emptyEntity = new TestEntity();

            mockEntity
                .Id
                .Should()
                .Be(dataService.GetFirst().Id, "it should be the same object and the object id is always passed");

            mockEntity
                .S
                .Should()
                .Be(dataService.GetFirst().S, "it is asked in the selector");

            mockEntity
                .I
                .Should()
                .Be(dataService.GetFirst().I, "it is asked in the selector");

            mockEntity
                .B
                .Should()
                .Be(emptyEntity.B, "it is not asked in the selector");
        }

        #endregion ONE

        #region PROPERTY GetPropertyAsync(ObjectId id, Expression<Func<T, object>> propertyToSelect)

        [TestMethod]
        public void GetPropertyWithUnknownIdAndNoProperty()
        {
            ActionExtensions.ShouldCatchException<WebArgumentNullException>(
                () => CreateNewDataService()
                    .GetPropertyAsync(ObjectId.GenerateNewId(), null)
                    .Wait(),
                "propertyToSelect",
                "there must be a property to select");
        }

        [TestMethod]
        public void GetKnownPropertyWithUnknownId()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService()
                    .GetPropertyAsync(ObjectId.GenerateNewId(), x => x.B)
                    .Wait(),
                "the given id doesn't exist");
        }

        [TestMethod]
        public void GetNullPropertyWithKnownId()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchException<WebArgumentNullException>(
                () => dataService.GetPropertyAsync(dataService.GetFirst().Id, null).Wait(),
                "propertyToSelect",
                "the selector cannot be null");
        }

        [TestMethod]
        public void GetPropertyWithKnownId()
        {
            var dataService = CreateNewDataService();

            dataService
                .GetPropertyAsync(dataService.GetFirst().Id, x => x.B).Result
                .Should()
                .Be(dataService.GetFirst().B, "the id should exist and that is the asked property");

            dataService
                .GetPropertyAsync(dataService.GetFirst().Id, x => x.I).Result
                .Should()
                .Be(dataService.GetFirst().I, "the id should exist and that is the asked property");

            dataService
                .GetPropertyAsync(dataService.GetFirst().Id, x => x.S).Result
                .Should()
                .Be(dataService.GetFirst().S, "the id should exist and that is the asked property");
        }

        #endregion PROPERTY
    }
}