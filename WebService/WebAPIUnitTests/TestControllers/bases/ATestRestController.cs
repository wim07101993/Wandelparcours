using System.Collections.Generic;
using System.Linq;
using WebService.Controllers.Bases;
using WebService.Helpers.Extensions;
using WebService.Models.Bases;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestControllers.bases
{
    public abstract class ATestRestController<T> : ARestControllerBase<T>, ITestController<T> where T : IModelWithID
    {
        protected ATestRestController(IDataService<T> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        public IEnumerable<T> GetAll()
            => DataService.GetAsync().Result.Clone();

        public T GetFirst()
            => DataService.GetAsync().Result.FirstOrDefault().Clone();
    }
}