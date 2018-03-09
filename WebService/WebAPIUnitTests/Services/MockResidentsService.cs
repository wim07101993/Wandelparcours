using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public partial class MockResidentsService
    {
        [TestMethod]
        public void CreateNewItem()
        {
            var id = ObjectId.GenerateNewId();
            new WebService.Services.Data.Mock.MockResidentsService()
                .CreateNewItem(id)
                .Id
                .Should()
                .Be(id);
        }
    }
}