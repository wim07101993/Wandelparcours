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
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaBadId()
        {
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaNullData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void AddMediaNullFile()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(dataService, new ConsoleLogger())
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

            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image)
                .ShouldCatchException<ArgumentNullException>("the url to add cannot be null");
        }

        [TestMethod]
        public void AddMediaWithUrl()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var count = dataService.GetAll().Count();


            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(id.ToString(), "dummy url", EMediaType.Image)
                .Wait();

            dataService
                .GetAll()
                .Count()
                .Should()
                .BeGreaterThan(count, "an item has been added");
        }


        [TestMethod]
        public void AddNullColor()
        {
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync(null, null)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddColor()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var count = dataService.GetAll().Count();

            new ResidentsController(dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), new Color()).ShouldReturnStatus(HttpStatusCode.Created,
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
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .GetByTagAsync(123456, null)
                .ShouldCatchException<NotFoundException>("there is no resident with tag 123456");
        }

        [TestMethod]
        public void GetByTagNullProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetByTagAsync(tag, null).Result
                .Should()
                .BeEquivalentTo(dataService.GetFirst());
        }

        [TestMethod]
        public void GetByTagEmptyProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetByTagAsync(tag, null).Result
                .Should()
                .BeEquivalentTo(dataService.MockData.Select(x => new Resident {Id = x.Id}).First());
        }

        [TestMethod]
        public void GetByTagBadProperties()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetByTagAsync(tag, new[] {"bad property"})
                .ShouldCatchArgumentException<ArgumentException>("propertiesToInclude",
                    "the given property doesn't exist");
        }

        [TestMethod]
        public void GetByTag()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetByTagAsync(tag, new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}).Result
                .Should()
                .BeEquivalentTo(dataService.MockData
                    .Select(x => new Resident {Id = x.Id, FirstName = x.FirstName, LastName = x.LastName})
                    .First());
        }


        [TestMethod]
        public void GetRandomElementFromPropertyWithBadTag()
        {
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(123456, nameof(Resident.Colors))
                .ShouldCatchException<NotFoundException>("there is no resident with tag 123456");
        }

        [TestMethod]
        public void GetRandomElementFromPropertyNullPropertyName()
        {
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(123456, null)
                .ShouldCatchException<NotFoundException>("the property to get cannot be null");
        }

        [TestMethod]
        public void GetRandomElementFromPropertyBadPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(tag, "bad property")
                .ShouldCatchArgumentException<ArgumentException>("propertiesToInclude",
                    "the given property doesn't exist");
        }

        [TestMethod]
        public void GetRandomElementFromProperty()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            var color = new ResidentsController(dataService, new ConsoleLogger())
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
            new ResidentsController(new TestResidentsService(), new ConsoleLogger())
                .GetPropertyAsync(123456, nameof(Resident.FirstName))
                .ShouldCatchException<NotFoundException>("there is no resident tag 123456");
        }

        [TestMethod]
        public void GetPropertyNullPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetPropertyAsync(tag, null)
                .ShouldCatchException<NotFoundException>("there must be a property name to get");
        }

        [TestMethod]
        public void GetPropertyBadPropertyName()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
                .GetPropertyAsync(tag, "bad property")
                .ShouldCatchException<NotFoundException>("the property 'bad property' doesn't exist");
        }

        [TestMethod]
        public void GetProperty()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            new ResidentsController(dataService, new ConsoleLogger())
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
            var id = dataService.GetFirst().Images.First().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveMediaAsync(null, id.ToString(), EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void RemoveMediaBadResidentId()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Images.First().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveMediaAsync("a", id.ToString(), EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void RemoveMediaNullMediaId()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveMediaAsync(id.ToString(), null, EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no media with a null id");
        }

        [TestMethod]
        public void RemoveMediaBadMediaId()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveMediaAsync(id.ToString(), "a", EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no media with a id a");
        }

        [TestMethod]
        public void RemoveMedia()
        {
            var dataService = new TestResidentsService();
            var residentId = dataService.GetFirst().Id;
            var mediaId = dataService.GetFirst().Images.First().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveMediaAsync(residentId.ToString(), mediaId.ToString(), EMediaType.Image)
                .Wait();

            dataService
                .GetFirst()
                .Images
                .Should()
                .NotContain(x => x.Id == mediaId);
        }


        [TestMethod]
        public void RemoveColorNullResidentId()
        {
            var dataService = new TestResidentsService();
            var color = dataService.GetFirst().Colors.First();

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveColorAsync(null, color)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void RemoveColorBadResidentId()
        {
            var dataService = new TestResidentsService();
            var color = dataService.GetFirst().Colors.First();

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveColorAsync("a", color)
                .ShouldCatchException<NotFoundException>("there is no resident with a id a");
        }

        [TestMethod]
        public void RemoveColorNullColor()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveColorAsync(id.ToString(), null)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void RemoveColor()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var color = dataService.GetFirst().Colors.First();

            new ResidentsController(dataService, new ConsoleLogger())
                .RemoveColorAsync(id.ToString(), color)
                .Wait();

            dataService
                .GetFirst()
                .Colors
                .Should()
                .NotContain(x => x.IsSameOrEqualTo(color));
        }

        #endregion DELETE
    }
}