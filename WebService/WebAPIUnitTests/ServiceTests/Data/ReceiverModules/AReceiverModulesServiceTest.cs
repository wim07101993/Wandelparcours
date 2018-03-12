using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices.ReceiverModules;

namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    [TestClass]
    public abstract partial class AReceiverModulesServiceTest : IReceiverModulesServiceTest
    {
        public abstract ITestReceiverModulesService CreateNewDataService();
    }
}