using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.Mocks;
using WebService.Helpers.Extensions;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockDataService
    {
        #region Get

        [TestMethod]
        public void GetWithAllProperties()
        {
            var dataService = new Mocks.MockDataService();

            dataService.GetAsync().Result
                .Should()
                .BeEquivalentTo(dataService.MockData, "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetWithOnlyID()
        {
            var dataService = new Mocks.MockDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<MockEntity, object>>[] {x => x.Id})
                .Result
                .ToList();

            var properties = typeof(MockEntity)
                .GetProperties()
                .Where(x => x.Name != nameof(MockEntity.Id))
                .ToList();

            for (var i = 0; i < mockEntities.Count; i++)
            {
                dataService.MockData[i]
                    .Id
                    .Should()
                    .Be(mockEntities[i].Id,
                        "it should be the same object and the object id the only field that is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(mockEntities[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(),
                            "only the id property is asked in the selector");
            }
        }

        [TestMethod]
        public void GetWithSomeFields()
        {
            var dataService = new Mocks.MockDataService();

            var mockEntities = dataService
                .GetAsync(new Expression<Func<MockEntity, object>>[] {x => x.Id, x => x.S})
                .Result
                .ToList();

            var properties = typeof(MockEntity)
                .GetProperties()
                .Where(x => x.Name != nameof(MockEntity.Id) && x.Name != nameof(MockEntity.S))
                .ToList();

            for (var i = 0; i < mockEntities.Count; i++)
            {
                dataService.MockData[i]
                    .Id
                    .Should()
                    .Be(mockEntities[i].Id, "it should be the same object and the object id is never \"not-passed\"");

                dataService.MockData[i]
                    .S
                    .Should()
                    .Be(mockEntities[i].S, "it is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(mockEntities[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(), "it is not asked in the selector");
            }
        }

        [TestMethod]
        public void GetWithEmptySelector()
        {
            var dataService = new Mocks.MockDataService();

            dataService
                .GetAsync(new Expression<Func<MockEntity, object>>[] { }).Result
                .Should()
                .BeEquivalentTo(dataService.MockData, "get should return all the data stored in the db");
        }

        #endregion Get


        #region Create

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateNullMockEntity()
        {
            try
            {
                var _ = new Mocks.MockDataService().CreateAsync(null).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        public void CreateEmptyMockEntity()
        {
            new Mocks.MockDataService().CreateAsync(new MockEntity()).Result
                .Should()
                .BeTrue("it is assigned in the create method of the service");
        }

        [TestMethod]
        public void CreateNormalMockEntity()
        {
            new Mocks.MockDataService().CreateAsync(new MockEntity {S = "Anna", B = true}).Result
                .Should()
                .BeTrue("it is assigned in the create method of the service");
        }

        #endregion Create


        #region Remove

        [TestMethod]
        public void RemoveMockEntityWithNonExistingID()
        {
            var dataService = new Mocks.MockDataService();

            dataService
                .RemoveAsync(new ObjectId()).Result
                .Should()
                .BeFalse("the item doesn't exist");
        }

        [TestMethod]
        public void RemoveMockEntityWithExistingID()
        {
            var dataService = new Mocks.MockDataService();

            dataService
                .RemoveAsync(dataService.MockData[0].Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion Remove


        #region Update

        // TODO update testing

        #endregion Update
    }
}