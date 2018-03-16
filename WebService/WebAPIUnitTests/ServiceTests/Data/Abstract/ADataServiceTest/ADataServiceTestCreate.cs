using System.Linq;
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
        #region ONE CreateAsync(T item)

        [TestMethod]
        public void CreateNullItem()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchException<WebArgumentNullException>(() => { dataService.CreateAsync(null).Wait(); },
                "item",
                "the item to create cannot be null");

            dataService
                .GetAll()
                .Should()
                .NotContainNulls("that should not happen");
        }

        [TestMethod]
        public void CreateItem()
        {
            var entity = new TestEntity
            {
                S = "Anna",
                B = true
            };

            var dataService = CreateNewDataService();
            var originalCount = dataService.GetAll().Count();

            dataService.CreateAsync(entity).Wait();

            dataService
                .GetAll()
                .Count()
                .Should()
                .NotBe(originalCount, "an item should have been added");
        }

        [TestMethod]
        public void CreateItemWithId()
        {
            var id = ObjectId.GenerateNewId();
            var entity = new TestEntity
            {
                Id = id,
                S = "Anna",
                B = true
            };

            var dataService = CreateNewDataService();

            dataService.CreateAsync(entity).Wait();

            dataService.GetAll()
                .Where(x => x.S == entity.S && x.B == entity.B && x.I == entity.I)
                .Should()
                .NotContain(x => x.Id == id, "the id should have changed");
        }

        #endregion ONE
    }
}