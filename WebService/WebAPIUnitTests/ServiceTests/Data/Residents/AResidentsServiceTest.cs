using WebAPIUnitTests.TestServices.Residents;

namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public abstract partial class AResidentsServiceTest : IResidentsServiceTest
    {
        public abstract ITestResidentsService CreateNewDataService();
    }
}