using FluentAssertions;
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
            ActionExtensions.ShouldCatchArgumentNullException(
                () => CreateNewDataService().RemoveAsync(null).Wait(),
                "mac",
                "the mac address cannot be null");
        }

        [TestMethod]
        public void RemoveUnknownMac()
        {
            ActionExtensions.ShouldCatchNotFoundException(() => CreateNewDataService().RemoveAsync("").Wait(),
                "there is no module with that mac in the database");
        }

        [TestMethod]
        public void RemoveMac()
        {
            var dataService = CreateNewDataService();
            var mac = dataService.GetFirst().Mac;

            dataService.RemoveAsync(mac);

            dataService
                .GetAll()
                .Should()
                .NotContain(x => x.Mac == mac);
        }
    }
}