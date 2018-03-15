using WebAPIUnitTests.TestControllers;
using WebAPIUnitTests.TestControllers.bases;
using WebService.Controllers;
using WebService.Controllers.Bases;

namespace WebAPIUnitTests.ControllerTests.Residents
{
    public partial class ResidentsControllerTests : IResidentsControllerTests
    {
        public ITestResidentsController CreateNewController()
            => new TestResidentsController();
    }
}