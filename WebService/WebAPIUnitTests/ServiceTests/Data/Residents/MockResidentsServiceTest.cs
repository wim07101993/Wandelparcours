using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    [TestClass]
    public class MockResidentsServiceTest : AResidentsServiceTest
    {
        public override ITestResidentsService CreateNewDataService()
            => new TestResidentsService();
    }
}