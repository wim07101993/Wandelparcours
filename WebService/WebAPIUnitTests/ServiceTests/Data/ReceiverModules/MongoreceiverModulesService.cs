using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices.ReceiverModules;

namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    [TestClass]
    public class MongoreceiverModulesService : AReceiverModulesServiceTest
    {
        public override ITestReceiverModulesService CreateNewDataService()
            => new TestMongoReceiverModulesService();
    }
}