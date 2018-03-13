using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Controllers.Bases;
using WebService.Models.Bases;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.Abstract
{
    [TestClass]
    public abstract partial class ARestControllerTests<T> : IRestControllerTest where T : IModelWithID
    {
        public ILogger Logger { get; set; } = new ConsoleLogger();

        public abstract ARestControllerBase<T> CreateNewController();
    }
}