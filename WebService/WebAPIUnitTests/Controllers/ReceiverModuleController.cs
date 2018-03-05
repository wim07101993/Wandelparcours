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
    public class ReceiverModuleController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var mockReceiverModulesService = new MockReceiverModulesService();

            new WebService.Controllers.ReceiverModulesController(mockReceiverModulesService, new ConsoleLogger())
                .GetAsync().Result.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<ReceiverModule>>()
                .Subject.Count().Should().Be(mockReceiverModulesService.MockData.Count);
        }

        #endregion get


        #region create

        [TestMethod]
        public void CreateNewReceiverModule()
        {
            var id = new ObjectId().ToString();
            var resident = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => Task.FromResult(id));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [TestMethod]
        public void CreateReceiverModuleDoesNotExecute()
        {
            var resident = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => Task.FromResult<string>(null));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void CreateReceiverModuleThrowsException()
        {
            var resident = new ReceiverModule();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => throw new Exception());

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion create


        #region delete

        [TestMethod]
        public void DeleteExistingReceiverModule()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(true));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteNonExistingReceiverModule()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(false));

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeleteServiceException()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<ReceiverModule>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => throw new Exception());

            new WebService.Controllers.ReceiverModulesController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
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
                Mac = "bb:bb:bb:bb:bb:bb",
                Position = new Point {Y = 0.4, X = 0.6},
                IsActive = true
            };

            var updater = new ReceiverModuleUpdater
            {
                Value = receiverModule,
                PropertiesToUpdate = new[] {nameof(ReceiverModule.Mac)}
            };

            new WebService.Controllers.ReceiverModulesController(mockReceiverModulesService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void UpdateNonExistingReceiverModule()
        {
            var updater = new ReceiverModuleUpdater
            {
                Value = new ReceiverModule
                {
                    Mac = "bb:bb:bb:bb:bb:bb",
                    Position = new Point {Y = 0.4, X = 0.6},
                    IsActive = true
                },
                PropertiesToUpdate = new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)}
            };

            var dataService = new MockReceiverModulesService();
            new WebService.Controllers.ReceiverModulesController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
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
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UpdateWithoutPropertiesToUpdate()
        {
            var dataService = new MockReceiverModulesService();
            var updater = new ReceiverModuleUpdater
            {
                Value = new ReceiverModule {Id = dataService.MockData[0].Id, Mac = "Test", IsActive = true}
            };

            new WebService.Controllers.ReceiverModulesController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        #endregion update
    }
}