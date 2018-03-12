using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public class MongoResidentsTest : AResidentsServiceTest
    {
        public override ITestResidentsService CreateNewDataService()
            => new TestMongoResidentsService();
    }
}