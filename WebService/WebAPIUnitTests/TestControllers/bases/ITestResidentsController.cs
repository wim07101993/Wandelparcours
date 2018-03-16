using WebService.Controllers.Bases;
using WebService.Models;

namespace WebAPIUnitTests.TestControllers.bases
{
    public interface ITestResidentsController : ITestController<Resident>, IResidentsController
    {
    }
}