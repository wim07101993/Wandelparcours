using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

namespace WebAPIUnitTests.Services.Mongo
{
    [TestClass]
    public partial class ReceiverModulesService
    {
        [TestMethod]
        public void CreateNewItem()
        {
            var id = ObjectId.GenerateNewId();
            new WebService.Services.Data.Mock.MockReceiverModulesService()
                .CreateNewItem(id)
                .Id
                .Should()
                .Be(id);
        }
    }
}