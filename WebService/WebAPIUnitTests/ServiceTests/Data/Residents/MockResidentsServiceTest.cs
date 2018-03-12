using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public class MockResidentsServiceTest : AResidentsServiceTest
    {
        public override ITestResidentsService CreateNewDataService()
            => new TestResidentsService();
    }
}