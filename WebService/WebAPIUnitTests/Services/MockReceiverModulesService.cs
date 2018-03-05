using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers;
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

            Assert.AreEqual(mockReceiverModulesService.GetAsync().Result, mockReceiverModulesService.MockData);
        }

        [TestMethod]
        public void GetReceiverModulesWithOnlyID()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var receiverModules = mockReceiverModulesService
                .GetAsync(new Expression<Func<ReceiverModule, object>>[] { x => x.Id })
                .Result
                .ToList();

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Id))
                .ToList();

            for (var i = 0; i < receiverModules.Count; i++)
            {
                Assert.AreEqual(receiverModules[i].Id, mockReceiverModulesService.MockData[i].Id);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(receiverModules[i]), property.PropertyType.GetDefault());
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
                Assert.AreEqual(receiverModules[i].Id, mockReceiverModulesService.MockData[i].Id);
                Assert.AreEqual(receiverModules[i].Mac, mockReceiverModulesService.MockData[i].Mac);
                Assert.AreEqual(receiverModules[i].IsActive, mockReceiverModulesService.MockData[i].IsActive);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(receiverModules[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetReceiverModulesWithEmptySelector()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            Assert.AreEqual(
                mockReceiverModulesService.GetAsync(new Expression<Func<ReceiverModule, object>>[] { }).Result,
                mockReceiverModulesService.MockData);
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

            var id = mockReceiverModulesService.CreateAsync(new ReceiverModule()).Result;
            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newReceiverModule = mockReceiverModulesService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            Assert.IsNotNull(newReceiverModule);
            Assert.IsNull(newReceiverModule.Mac);
            Assert.IsNull(newReceiverModule.Position);
            Assert.AreEqual(newReceiverModule.IsActive, default(bool));
        }

        [TestMethod]
        public void CreateNormalReceiverModule()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            var residtent = new ReceiverModule
            {
                IsActive = true,
                Mac = "aa:aa:aa:aa:aa:aa",
                Position = new Point
                {
                    X = 6,
                    Y = 4,
                }
            };

            var id = mockReceiverModulesService.CreateAsync(residtent).Result;

            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newReceiverModule = mockReceiverModulesService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            Assert.IsNotNull(newReceiverModule);
            Assert.AreEqual(newReceiverModule, residtent);

            Assert.IsNotNull(newReceiverModule.Mac);
            Assert.IsNotNull(newReceiverModule.Position);
            Assert.AreNotEqual(newReceiverModule.IsActive, default(bool));
        }

        #endregion Create


        #region RemoveReceiverModule

        [TestMethod]
        public void RemoveReceiverModuleWithNonExistingID()
        {
            var mockReceiverModulesService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            Assert.IsFalse(mockReceiverModulesService.RemoveAsync(new ObjectId()).Result);
        }

        #endregion RemoveReceiverModule
    }
}
