using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.Mocks;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services
{
    public partial class MockDataService
    {
        #region ONE CreateAsync(T item)

        [TestMethod]
        public void CreateNullMockEntity()
        {
            try
            {
                var _ = new Mocks.MockDataService().CreateAsync(null).Result;

                Assert.Fail("cannot create element null");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "item")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the item parameter, that is the parameter that is null");
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
            var id = ObjectId.GenerateNewId();
            var entity = new MockEntity
            {
                Id = id,
                S = "Anna",
                B = true
            };

            var dataService = new Mocks.MockDataService();

            dataService.CreateAsync(entity).Result
                .Should()
                .BeTrue("it is assigned in the create method of the service");

            dataService.MockData
                .First(x => x.S == entity.S && x.B == entity.B && x.I == entity.I)
                .Id
                .Should()
                .NotBe(id);
        }

        #endregion ONE
    }
}