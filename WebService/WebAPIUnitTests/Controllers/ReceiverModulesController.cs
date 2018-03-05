using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Data.Mock;
using WebService.Services.Logging;

namespace WebAPIUnitTests.Controllers
{
    [TestClass]
    public class ReceiverModulesController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var mockReceiverModulesService = new MockReceiverModulesService();

            new WebService.Controllers.ReceiverModulesController(mockReceiverModulesService, new ConsoleLogger())
                .GetAsync().Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<IEnumerable<ReceiverModule>>("a collection of receiverModules is asked").Subject
                .Count()
                .Should()
                .Be(mockReceiverModulesService.MockData.Count, "all the items in the db should be returned");
        }

        [TestMethod]
        public void GetSingleItem()
        {
            var receiverModule = new ReceiverModule
            {
                Mac = "aa:aa:aa:aa:aa:aa",
                IsActive = false
            };

            var dataService = new Mock<IReceiverModuleService>();
            dataService
                .Setup(x => x.GetAsync(receiverModule.Mac))
                .Returns(() => Task.FromResult(receiverModule));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .GetAsync(receiverModule.Mac).Result
                .Should()
                .BeOfType<OkObjectResult>("all controller methods should return a status code as confirmation")
                .Subject
                .Value
                .Should()
                .BeOfType<ReceiverModule>("the client asked for a resident");
        }

        #endregion get


        #region create

        [TestMethod]
        public void CreateNewReceiverModule()
        {
            var id = new ObjectId().ToString();
            var receiverModule = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService
                .Setup(x => x.CreateAsync(receiverModule))
                .Returns(() => Task.FromResult(id));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(receiverModule).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created,
                    "the creation should be executed and the corresponding response is 201 in http");
        }

        [TestMethod]
        public void CreateReceiverModuleDoesNotExecute()
        {
            var receiverModule = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.CreateAsync(receiverModule)).Returns(() => Task.FromResult<string>(null));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(receiverModule).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the creation is not succesfull, a 500 error should be returned");
        }

        [TestMethod]
        public void CreateReceiverModuleThrowsException()
        {
            var receiverModule = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.CreateAsync(receiverModule)).Returns(() => throw new Exception());

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(receiverModule).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the service throws an exception, a 500 erro should be returned");
        }

        #endregion create


        #region delete

        [TestMethod]
        public void DeleteExistingReceiverModule()
        {
            const string mac = "aa:aa:aa:aa:aa:aa";

            var dataService = new Mock<IReceiverModuleService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => Task.FromResult(true));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac).Result
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

            var dataService = new Mock<IReceiverModuleService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => Task.FromResult(false));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac).Result
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

            var dataService = new Mock<IReceiverModuleService>();
            dataService.Setup(x => x.RemoveAsync(mac)).Returns(() => throw new Exception());

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(mac).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the service throws an exception, a 500 error should be returned");
        }

        #endregion delete


        #region update

        [TestMethod]
        public void UpdateWithNormalConditions()
        {
            var mockReceiverModulesService = new MockReceiverModulesService();
            var receiverModule = new ReceiverModule
            {
                Id = mockReceiverModulesService.MockData[0].Id,
                Mac = "aa:aa:aa:aa:aa:aa",
                IsActive = false
            };

            var updater = new ReceiverModuleUpdater
            {
                Value = receiverModule,
                PropertiesToUpdate = new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)}
            };

            new WebService.Controllers.ReceiverModulesController(mockReceiverModulesService, new ConsoleLogger())
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if an item is updated, a 200 status should be returned");
        }

        [TestMethod]
        public void UpdateNonExistingReceiverModule()
        {
            var updater = new ReceiverModuleUpdater
            {
                Value = new ReceiverModule {Mac = "aa:aa:aa:aa:aa:aa", IsActive = false},
                PropertiesToUpdate = new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)}
            };

            var dataService = new MockReceiverModulesService();
            new WebService.Controllers.ReceiverModulesController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created, "if an item is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void UpdateWithoutReceiverModule()
        {
            var updater = new ReceiverModuleUpdater
            {
                PropertiesToUpdate = new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)}
            };

            var mockReceiverModulesService = new MockReceiverModulesService();
            new WebService.Controllers.ReceiverModulesController(mockReceiverModulesService, new ConsoleLogger())
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.BadRequest,
                    "if no item to update is sent, the request is bad and a 400 bad request should be returned");
        }

        [TestMethod]
        public void UpdateWithoutPropertiesToUpdate()
        {
            var dataService = new MockReceiverModulesService();
            var updater = new ReceiverModuleUpdater
            {
                Value = new ReceiverModule
                {
                    Id = dataService.MockData[0].Id,
                    Mac = "aa:aa:aa:aa:aa:aa",
                    IsActive = false
                }
            };

            new WebService.Controllers.ReceiverModulesController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK,
                    "if the service says it everything is ok, we should return a 200 ok status");
        }

        #endregion update
    }
}