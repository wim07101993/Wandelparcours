using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace WebAPIUnitTests.Services.Mongo
{
    [TestClass]
    public partial class ResidentsService
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