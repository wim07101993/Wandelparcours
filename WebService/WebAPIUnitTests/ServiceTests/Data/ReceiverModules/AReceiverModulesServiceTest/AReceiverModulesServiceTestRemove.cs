using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    public abstract partial class AReceiverModulesServiceTest
    {
        [TestMethod]
        public void RemoveNullMac()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = CreateNewDataService().RemoveAsync(null).Result;
                },
                "mac",
                "the mac address cannot be null");
        }

        [TestMethod]
        public void RemoveUnknownMac()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = CreateNewDataService().RemoveAsync("").Result;
                },
                "there is no module with that mac in the database");
        }

        [TestMethod]
        public void RemoveMac()
        {
            var dataService = CreateNewDataService();
            dataService.RemoveAsync(dataService.GetFirst().Mac);
        }
    }
}