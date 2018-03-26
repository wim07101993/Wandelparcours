using System.Collections.Generic;
using System.Linq;
using WebAPIUnitTests.TestControllers.bases;
using WebAPIUnitTests.TestServices.ReceiverModules;
using WebService.Controllers;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestControllers
{
    public class TestReceiverModuleController : ReceiverModulesController, ITestController<ReceiverModule>
    {
        public TestReceiverModuleController()
            : base(new TestReceiverModulesService(), new ConsoleLogger())
        {
        }

        public IEnumerable<ReceiverModule> GetAll()
            => DataService.GetAsync().Result.Clone();

        public ReceiverModule GetFirst()
            => DataService.GetAsync().Result.FirstOrDefault().Clone();
    }
}