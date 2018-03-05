using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockReceiverModulesService
    {
        #region Get

        [TestMethod]
        public void GetReceiverModulesWithAllProperties()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            mockReceiverModulesService
                .GetAsync().Result
                .Should()
                .BeEquivalentTo(mockReceiverModulesService.MockData, "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetReceiverModulesWithOnlyID()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var receiverModules = mockReceiverModulesService
                .GetAsync(new Expression<Func<ReceiverModule, object>>[] {x => x.Id})
                .Result
                .ToList();

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Id))
                .ToList();

            for (var i = 0; i < receiverModules.Count; i++)
            {
                mockReceiverModulesService.MockData[i]
                    .Id
                    .Should()
                    .Be(receiverModules[i].Id,
                        "it should be the same object and the object id the only field that is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(receiverModules[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(), "only the id property is asked in the selector");
            }
        }

        [TestMethod]
        public void GetReceiverModulesWithSomeFields()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var receiverModules = mockReceiverModulesService
                .GetAsync(new Expression<Func<ReceiverModule, object>>[]
                {
                    x => x.Id,
                    x => x.Mac,
                    x => x.IsActive
                })
                .Result
                .ToList();

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Id) &&
                            x.Name != nameof(ReceiverModule.Mac) &&
                            x.Name != nameof(ReceiverModule.IsActive))
                .ToList();

            for (var i = 0; i < receiverModules.Count; i++)
            {
                mockReceiverModulesService.MockData[i]
                    .Id
                    .Should()
                    .Be(receiverModules[i].Id,
                        "it should be the same object and the object id is never \"not-passed\"");

                mockReceiverModulesService.MockData[i]
                    .Mac
                    .Should()
                    .Be(receiverModules[i].Mac, "it is asked in the selector");

                mockReceiverModulesService.MockData[i]
                    .IsActive
                    .Should()
                    .Be(receiverModules[i].IsActive, "it is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(receiverModules[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(), "it is not asked in the selector");
            }
        }

        [TestMethod]
        public void GetReceiverModulesWithEmptySelector()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            mockReceiverModulesService
                .GetAsync(new Expression<Func<ReceiverModule, object>>[] { }).Result
                .Should()
                .BeEquivalentTo(mockReceiverModulesService.MockData, "get should return all the data stored in the db");
        }

        #endregion Get


        #region Create

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateNullReceiverModule()
        {
            try
            {
                var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

                var _ = mockReceiverModulesService.CreateAsync(null).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        public void CreateEmptyReceiverModule()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var receiverModule = new ReceiverModule();

            var id = mockReceiverModulesService.CreateAsync(receiverModule).Result;
            id
                .Should()
                .NotBeNullOrEmpty("it is assigned in the create method of the service");

            var newReceiverModule = mockReceiverModulesService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            newReceiverModule
                .Should()
                .NotBeNull("it is created in the create method of the service");

            // ReSharper disable PossibleNullReferenceException
            newReceiverModule
                .Id
                .Should()
                .NotBe(default(ObjectId), "a new object id is created in the service");
            // ReSharper restore PossibleNullReferenceException

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Id));

            foreach (var property in properties)
                property
                    .GetValue(newReceiverModule)
                    .Should()
                    .Be(property.GetValue(receiverModule),
                        $"it should be equal to the {property.Name} of the receiverModule");
        }

        [TestMethod]
        public void CreateNormalReceiverModule()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var receiverModule = new ReceiverModule
            {
                IsActive = true,
                Mac = "aa:aa:aa:aa:aa:aa",
                Position = new Point
                {
                    X = 0.4,
                    Y = 0.56,
                }
            };

            var id = mockReceiverModulesService.CreateAsync(receiverModule).Result;
            id
                .Should()
                .NotBeNullOrEmpty("it is assigned in the create method of the service");

            var newReceiverModule = mockReceiverModulesService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            newReceiverModule
                .Should()
                .NotBeNull("it is created in the create method of the service");

            // ReSharper disable PossibleNullReferenceException
            newReceiverModule
                .Id
                .Should()
                .NotBe(default(ObjectId), "a new object id is created in the service");
            // ReSharper restore PossibleNullReferenceException

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Id));

            foreach (var property in properties)
                property
                    .GetValue(newReceiverModule)
                    .Should()
                    .Be(property.GetValue(receiverModule),
                        $"it should be equal to the {property.Name} of the receiverModule");
        }

        #endregion Create


        #region RemoveReceiverModule

        [TestMethod]
        public void RemoveReceiverModuleWithNonExistingID()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            mockReceiverModulesService
                .RemoveAsync(new ObjectId()).Result
                .Should()
                .BeFalse("the item doesn't exist");
        }

        [TestMethod]
        public void RemoveReceiverModuleWithExistingID()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            mockReceiverModulesService
                .RemoveAsync(mockReceiverModulesService.MockData[0].Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion RemoveReceiverModule
    }
}