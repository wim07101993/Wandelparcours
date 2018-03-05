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
    public class ResidentsController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var mockResidentsService = new MockResidentsService();

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync().Result.Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<Resident>>()
                .Subject.Count().Should().Be(mockResidentsService.MockData.Count);
        }
        
        #endregion get


        #region create

        [TestMethod]
        public void CreateNewResident()
        {
            var id = new ObjectId().ToString();
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => Task.FromResult(id));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [TestMethod]
        public void CreateResidentDoesNotExecute()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => Task.FromResult<string>(null));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void CreateResidentThrowsException()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion create


        #region delete

        [TestMethod]
        public void DeleteExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(true));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteNonExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(false));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeleteServiceException()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion delete


        #region update

        [TestMethod]
        public void UpdateWithNormalConditions()
        {
            var mockResidentsService = new MockResidentsService();
            var resident = new Resident {Id = mockResidentsService.MockData[0].Id, FirstName = "Test", LastName = null};

            var updater = new ResidentUpdater
            {
                Value = resident,
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void UpdateNonExistingResident()
        {
            var updater = new ResidentUpdater
            {
                Value = new Resident {Id = new ObjectId(), FirstName = "Test", LastName = null},
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            var dataService = new MockResidentsService();
            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [TestMethod]
        public void UpdateWithoutResident()
        {
            var updater = new ResidentUpdater
            {
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            var mockResidentsService = new MockResidentsService();
            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UpdateWithoutPropertiesToUpdate()
        {
            var dataService = new MockResidentsService();
            var updater = new ResidentUpdater
            {
                Value = new Resident {Id = dataService.MockData[0].Id, FirstName = "Test", LastName = null}
            };

            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .UpdateAsync(updater).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        #endregion update
    }
}