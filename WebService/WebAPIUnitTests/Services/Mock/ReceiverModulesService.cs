using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.Services.Mock
{
    [TestClass]
    public partial class ReceiverModulesService
    {
        [TestMethod]
        public void CreateNewItem()
        {
            var id = ObjectId.GenerateNewId();
            new MockReceiverModulesService()
                .CreateNewItem(id)
                .Id
                .Should()
                .Be(id);
        }
    }
}