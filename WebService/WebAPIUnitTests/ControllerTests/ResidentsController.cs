using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests
{
    [TestClass]
    public class ResidentsController
    {
        #region get
        
        [TestMethod]
        public void GetByTagNormal()
        {
            var mockResidentsService = new MockResidentsService();
            var tag = mockResidentsService.MockData[0].Tags.ToList()[0];
            var selector = new[]
            {
                "firstName",
                "lastName",
                "birthday"
            };

            var resident = new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(tag, selector).Result
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
        public void GetByTagNonExistingID()
        {
            var mockResidentsService = new MockResidentsService();
            var selector = new[]
            {
                "firstName",
                "lastName",
                "birthday"
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(-5, selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int)HttpStatusCode.NotFound, "a non existing tag cannot be found");
        }

        [TestMethod]
        public void GetByTagBadProperties()
        {
            var mockResidentsService = new MockResidentsService();
            var tag = mockResidentsService.MockData[0].Tags.ToList()[0];
            var selector = new[]
            {
                "firstName",
                "lastame",
                "birthday"
            };

            new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(tag, selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int)HttpStatusCode.BadRequest, "bad properties cannot be found");
        }

        [TestMethod]
        public void GetByTagNoProperties()
        {
            var mockResidentsService = new MockResidentsService();
            var tag = mockResidentsService.MockData[0].Tags.ToList()[0];
            var resident = new WebService.Controllers.ResidentsController(mockResidentsService, new ConsoleLogger())
                .GetAsync(tag, null).Result
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
    }
}