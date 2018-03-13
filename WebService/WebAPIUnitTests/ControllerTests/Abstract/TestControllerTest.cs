using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestControllers;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices.Abstract;
using WebService.Controllers.Bases;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.Abstract
{
    [TestClass]
    public class TestControllerTest : ARestControllerTests<TestEntity>
    {
        public override ARestControllerBase<TestEntity> CreateNewController()
            => new TestController(new TestDataService(), new ConsoleLogger());
        
    }
}