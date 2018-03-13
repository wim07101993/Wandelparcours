using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;

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

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService.CreateAsync(null).Wait();
                },
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
                .First(x => x.S == entity.S && x.B == entity.B && x.I == entity.I)
                .Id
                .Should()
                .NotBe(id);
        }

        #endregion ONE
    }
}
