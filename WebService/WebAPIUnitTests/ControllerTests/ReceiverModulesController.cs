using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests
{
    [TestClass]
    public class ReceiverModulesController
    {
        #region get
        
        [TestMethod]
        public void GetSingleItem()
        {
            //var receiverModule = new ReceiverModule
            //{
            //    Mac = "aa:aa:aa:aa:aa:aa",
            //    IsActive = false
            //};

            //var dataService = new Mock<IReceiverModulesService>();
            //dataService
            //    .Setup(x => x.GetAsync(receiverModule.Mac))
            //    .Returns(() => Task.FromResult(receiverModule));

            //new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
            //    .GetAsync(receiverModule.Mac).Result
            //    .Should()
            //    .BeOfType<OkObjectResult>("all controller methods should return a status code as confirmation")
            //    .Subject
            //    .Value
            //    .Should()
            //    .BeOfType<ReceiverModule>("the client asked for a resident");
        }

        #endregion get
        

        #region delete

        [TestMethod]
        public void DeleteExistingReceiverModule()
        {
            const string mac = "aa:aa:aa:aa:aa:aa";

            var dataService = new Mock<IReceiverModulesService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => Task.FromResult(true));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac)
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if the item is removed, a 200 code should be returned");
        }

        [TestMethod]
        public void DeleteNonExistingReceiverModule()
        {
            const string mac = "aa:aa:aa:aa:aa:aa";

            var dataService = new Mock<IReceiverModulesService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => Task.FromResult(false));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac)
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound,
                    "if the receiver is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void DeleteServiceException()
        {
            const string mac = "aa:aa:aa:aa:aa:aa";

            var dataService = new Mock<IReceiverModulesService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => throw new Exception());

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac)
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the service throws an exception, a 500 error should be returned");
        }

        #endregion delete
    }
}