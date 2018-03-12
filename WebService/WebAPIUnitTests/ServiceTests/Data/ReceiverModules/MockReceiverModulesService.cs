using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices.ReceiverModules;

namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    [TestClass]
    public class MockReceiverModulesService : AReceiverModulesServiceTest
    {
        public override ITestReceiverModulesService CreateNewDataService()
            => new TestReceiverModulesService();
    }
}