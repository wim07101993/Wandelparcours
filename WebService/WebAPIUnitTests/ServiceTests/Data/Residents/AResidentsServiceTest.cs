using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public abstract class AResidentsServiceTest
    {
        public abstract ITestResidentsService CreateNewDataService();
    }
}