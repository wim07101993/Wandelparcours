using System.Linq;
using System.Net;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.Residents;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.ResidentsControllerTests
{
    [TestClass]
    public class ResidentsControllerTestsTests : IResidentsControllerTests
    {
        #region CREATE

        [TestMethod]
        public void AddMediaNullId()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaBadId()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaNullData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void AddMediaNullFile()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddMediaAsync(id.ToString(), new MultiPartFile(), EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void AddMediaWithData()
        {
            // TODO create test
        }


        [TestMethod]
        public void AddMediaNullUrl()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image)
                .ShouldCatchException<WebArgumentNullException>("the url to add cannot be null");
        }

        [TestMethod]
        public void AddMediaWithUrl()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var count = dataService.GetAll().Count();


            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(id.ToString(), "dummy url", EMediaType.Image)
                .Wait();

            dataService
                .GetAll()
                .Count()
                .Should()
                .BeGreaterThan(count, "an item has been added");
        }


        [TestMethod]
        public void AddColorNullId()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync(null, null)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddColorBadId()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync("a", null)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddColorNullData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), null)
                .ShouldCatchArgumentException<WebArgumentNullException>("data", "the data to add cannot be null");
        }

        [TestMethod]
        public void AddColorBadData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), new byte[0])
                .ShouldCatchArgumentException<WebArgumentNullException>("data",
                    "the data to add must have a length of 3 bytes");
        }

        [TestMethod]
        public void AddColor()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var count = dataService.GetAll().Count();

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), new byte[3]).ShouldReturnStatus(HttpStatusCode.Created,
                    "when an item is created, that is the status-code");

            dataService
                .GetAll()
                .Count()
                .Should()
                .BeGreaterThan(count, "an item has been added");
        }

        #endregion CREATE


        #region READ

        [TestMethod]
        public void GetByBadTag()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetByTagAsync(123456, null)
                .ShouldCatchException<NotFoundException>("there is no resident with tag 123456");
        }

        [TestMethod]
        public void GetByTagNullProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetByTagAsync(tag, null).Result
                .Should()
                .BeEquivalentTo(dataService.GetFirst());
        }

        [TestMethod]
        public void GetByTagEmptyProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetByTagAsync(tag, null).Result
                .Should()
                .BeEquivalentTo(dataService.MockData.Select(x => new Resident {Id = x.Id}).First());
        }

        [TestMethod]
        public void GetByTagBadProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetByTagAsync(tag, new[] {"bad property"})
                .ShouldCatchArgumentException<WebArgumentException>("propertiesToInclude",
                    "the given property doesn't exist");
        }

        [TestMethod]
        public void GetByTag()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetByTagAsync(tag, new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}).Result
                .Should()
                .BeEquivalentTo(dataService.MockData
                    .Select(x => new Resident {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName})
                    .First());
        }


        [TestMethod]
        public void GetRandomElementFromPropertyWithBadTag()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(123456, nameof(Resident.Colors))
                .ShouldCatchException<NotFoundException>("there is no resident with tag 123456");
        }

        [TestMethod]
        public void GetRandomElementFromPropertyNullPropertyName()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(123456, null)
                .ShouldCatchException<NotFoundException>("the property to get cannot be null");
        }

        [TestMethod]
        public void GetRandomElementFromPropertyBadPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(tag, "bad property")
                .ShouldCatchArgumentException<WebArgumentException>("propertiesToInclude",
                    "the given property doesn't exist");
        }

        [TestMethod]
        public void GetRandomElementFromProperty()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            var color = new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(tag, nameof(Resident.Colors)).Result;

            dataService
                .GetFirst()
                .Colors
                .Should()
                .Contain(x => x.IsSameOrEqualTo(color));
        }


        [TestMethod]
        public void GetPropertyBadTag()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetPropertyAsync(123456, nameof(Resident.FirstName))
                .ShouldCatchException<NotFoundException>("there is no resident tag 123456");
        }

        [TestMethod]
        public void GetPropertyNullPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetPropertyAsync(tag, null)
                .ShouldCatchException<NotFoundException>("there must be a property name to get");
        }

        [TestMethod]
        public void GetPropertyBadPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetPropertyAsync(tag, "bad property")
                .ShouldCatchException<NotFoundException>("the property 'bad property' doesn't exist");
        }

        [TestMethod]
        public void GetProperty()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .GetPropertyAsync(tag, nameof(Resident.FirstName)).Result
                .Should()
                .BeEquivalentTo(dataService.GetFirst().FirstName);
        }

        #endregion READ


        #region DELETE

        [TestMethod]
        public void RemoveMediaNullResidentId()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Images.First();

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .RemoveMediaAsync(null, id.ToString(), EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void RemoveMediaBadResidentId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveMediaNullMediaId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveMediaBadMediaId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveMedia()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveColorNullResidentId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveColorBadResidentId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveColorNullColor()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveColorBadColor()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void RemoveColor()
        {
            throw new System.NotImplementedException();
        }

        #endregion DELETE
    }
}