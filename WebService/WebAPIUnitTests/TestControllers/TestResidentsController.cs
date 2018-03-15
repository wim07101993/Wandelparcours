using System.Collections.Generic;
using System.Linq;
using WebAPIUnitTests.TestServices.Residents;
using WebService.Controllers;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestControllers
{
    public class TestResidentsController : ResidentsController, ITestController<Resident>
    {
        public TestResidentsController()
            : base(new TestResidentsService(), new ConsoleLogger())
        {
        }

        public IEnumerable<Resident> GetAll()
            => DataService.GetAsync().Result.Clone();

        public Resident GetFirst()
            => DataService.GetAsync().Result.FirstOrDefault().Clone();
    }
}