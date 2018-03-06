using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockReceiverModulesService
    {
        #region Get

        [TestMethod]
        public void GetWitMacAddress()
        {
            var dataService = new WebService.Services.Data.Mock.MockReceiverModulesService();
            var mac = dataService.MockData[0].Mac;

            dataService.GetAsync(mac).Result
                .Should()
                .BeEquivalentTo(dataService.MockData[0], "it is the object asked from the database");
        }

        [TestMethod]
        public void GetWithNonExistingMacAddress()
        {
            var dataService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            dataService.GetAsync("abc").Result
                .Should()
                .BeNull("there is no item with that id");
        }

        #endregion Get


        #region Remove

        [TestMethod]
        public void RemoveWithMacAddress()
        {
            var dataService = new WebService.Services.Data.Mock.MockReceiverModulesService();
            var mac = dataService.MockData[0].Mac;

            dataService.RemoveAsync(mac).Result
                .Should()
                .BeTrue("the item with the given mac address exists and should be removed");
        }

        [TestMethod]
        public void RemoveWithNonExistingMacAddress()
        {
            var dataService = new WebService.Services.Data.Mock.MockReceiverModulesService();

            dataService.RemoveAsync("abc").Result
                .Should()
                .BeFalse("there is no item with that id");
        }

        #endregion Remove
    }
}