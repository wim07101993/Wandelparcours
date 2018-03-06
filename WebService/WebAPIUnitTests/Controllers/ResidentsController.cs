using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using WebService.Helpers.Extensions;
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
                .GetAsync().Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<IEnumerable<Resident>>("a collection of residents is asked").Subject
                .Count()
                .Should()
                .Be(mockResidentsService.MockData.Count, "all the items in the db should be returned");
        }

        [TestMethod]
        public void GetByIdNormal()
        {
            var mockResidentsService = new MockResidentsService();
            var id = mockResidentsService.MockData[0].Id.ToString();
            var selector = new[]
            {
                "firstName",
                "lastName",
                "birthday"
            };

            var resident = new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(id, selector).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<Resident>("a collection of residents is asked").Subject;

            resident
                .FirstName
                .Should()
                .Be(mockResidentsService.MockData[0].FirstName, "that is the name of the resident with the given id");

            resident
                .LastName
                .Should()
                .Be(mockResidentsService.MockData[0].LastName,
                    "that is the last name of the resident with the given id");

            resident
                .Birthday
                .Should()
                .Be(mockResidentsService.MockData[0].Birthday,
                    "that is the birthday of the resident with the given id");

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x =>
                    x.Name != nameof(Resident.FirstName) &&
                    x.Name != nameof(Resident.LastName) &&
                    x.Name != nameof(Resident.Birthday) &&
                    x.Name != nameof(Resident.Id));

            foreach (var property in properties)
                property
                    .GetValue(resident)
                    .Should()
                    .BeEquivalentTo(property.PropertyType.GetDefault(), "this property was not asked to return");
        }

        [TestMethod]
        public void GetByIdBadIdFormat()
        {
            var mockResidentsService = new MockResidentsService();
            var selector = new[]
            {
                "firstName",
                "lastName",
                "birthday"
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync("bad id", selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound, "a bad id cannot be found");
        }

        [TestMethod]
        public void GetByIdNonExistingID()
        {
            var mockResidentsService = new MockResidentsService();
            var selector = new[]
            {
                "firstName",
                "lastName",
                "birthday"
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(ObjectId.GenerateNewId().ToString(), selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound, "a bad id cannot be found");
        }

        [TestMethod]
        public void GetByIdBadProperties()
        {
            var mockResidentsService = new MockResidentsService();
            var id = mockResidentsService.MockData[0].Id.ToString();
            var selector = new[]
            {
                "firstName",
                "lastame",
                "birthday"
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(id, selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.BadRequest, "bad properties cannot be found");
        }

        [TestMethod]
        public void GetByIdNoProperties()
        {
            var mockResidentsService = new MockResidentsService();
            var id = mockResidentsService.MockData[0].Id.ToString();
            var resident = new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(id, null).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<Resident>("a collection of residents is asked").Subject;

            var properties = typeof(Resident).GetProperties();

            foreach (var property in properties)
                property
                    .GetValue(resident)
                    .Should()
                    .BeEquivalentTo(
                        property.GetValue(mockResidentsService.MockData[0]),
                        $"that is the {property.Name} of the resident");
        }

        #endregion get


        #region create

        [TestMethod]
        public void CreateNewResident()
        {
            var id = new ObjectId().ToString();
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService
                .Setup(x => x.CreateAsync(resident))
                .Returns(() => Task.FromResult(id));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created,
                    "the creation should be executed and the corresponding response is 201 in http");
        }

        [TestMethod]
        public void CreateResidentDoesNotExecute()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService
                .Setup(x => x.CreateAsync(resident))
                .Returns(() => Task.FromResult<string>(null));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void CreateResidentThrowsException()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService
                .Setup(x => x.CreateAsync(resident))
                .Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident)
                .Result
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
        public void DeleteExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(true));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if the item is removed, a 200 code should be returned");
        }

        [TestMethod]
        public void DeleteNonExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(false));

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound,
                    "if the receiver is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void DeleteNonParsableId()
        {
            new WebService.Controllers.ResidentsController(
                    new Mock<IDataService<Resident>>().Object,
                    new ConsoleLogger())
                .DeleteAsync("abc").Result
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
            var id = new ObjectId();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
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
            var mockResidentsService = new MockResidentsService();
            var resident = new Resident
            {
                Id = mockResidentsService.MockData[0].Id,
                FirstName = "Test",
                LastName = null
            };

            var updater = new ResidentUpdater
            {
                Value = resident,
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if an item is updated, a 200 status should be returned");
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
                .UpdateAsync(updater).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created, "if an item is not found, a 404 not found error should be returned");
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
            var dataService = new MockResidentsService();
            var updater = new ResidentUpdater
            {
                Value = new Resident {Id = dataService.MockData[0].Id, FirstName = "Test", LastName = null}
            };

            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
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