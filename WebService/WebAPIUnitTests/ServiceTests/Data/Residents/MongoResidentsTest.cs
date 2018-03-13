using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    [TestClass]
    public class MongoResidentsTest : AResidentsServiceTest
    {
        public override ITestResidentsService CreateNewDataService()
            => new TestMongoResidentsService();
    }
}