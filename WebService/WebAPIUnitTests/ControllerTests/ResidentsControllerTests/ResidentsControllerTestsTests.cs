using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.Media;
using WebAPIUnitTests.TestServices.Residents;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.ResidentsControllerTests
{
    [TestClass]
    public class ResidentsControllerTestsTests// : IResidentsControllerTests
    {
        #region CREATE

        [TestMethod]
        public void AddColorNullIdNullData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync(null, null)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddColorNullIdWithData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync(null, new byte[2])
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddColorBadIdNullData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync("a", null)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddColorBadIdWithData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddColorAsync("a", new byte[2])
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddColorExistingIdNullData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), null)
                .ShouldCatchArgumentException<WebArgumentNullException>("data", "the data to add cannot be null");
        }

        [TestMethod]
        public void AddColorExistingIdWithData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;
            var count = dataService.GetAll().Count();

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddColorAsync(id.ToString(), new byte[2]).ShouldReturnStatus(HttpStatusCode.Created,
                    "when an item is created, that is the status-code");

            dataService
                .GetAll()
                .Count()
                .Should()
                .BeGreaterThan(count, "an item has been added");
        }


        [TestMethod]
        public void AddMediaNullIdNullData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaNullIdNullFile()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, new MultiPartFile(), EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaNullIdWithData()
        {
            // TODO create test
        }

        [TestMethod]
        public void AddMediaBadIdNullData()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaBadIdNullFile()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", new MultiPartFile(), EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaBadIdWithData()
        {
            // TODO create test
        }

        [TestMethod]
        public void AddMediaExistingIdNullData()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void AddMediaExistingIdNullFile()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), dataService, new ConsoleLogger())
                .AddMediaAsync(id.ToString(), new MultiPartFile(), EMediaType.Image, 50)
                .ShouldCatchException<NotFoundException>("there is no resident with id a");
        }

        [TestMethod]
        public void AddMediaExistingIdWithData()
        {
            // TODO create test
        }


        [TestMethod]
        public void AddMediaNullIdNullUrl()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, null, EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaNullIdWithUrl()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(null, "dummy url", EMediaType.Image)
                .ShouldCatchException<NotFoundException>("there is no resident with a null id");
        }

        [TestMethod]
        public void AddMediaBadIdNullUrl()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", null, EMediaType.Image)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaBadIdWithUrl()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync("a", "dummy url", EMediaType.Image)
                .ShouldCatchException<NotFoundException>("the id cannot be parsed");
        }

        [TestMethod]
        public void AddMediaExistingIdNullUrl()
        {
            var dataService = new TestResidentsService();
            var id = dataService.GetFirst().Id;

            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .AddMediaAsync(id.ToString(), null, EMediaType.Image)
                .ShouldCatchException<WebArgumentNullException>("the url to add cannot be null");
        }

        [TestMethod]
        public void AddMediaExistingIdWithUrl()
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

        #endregion CREATE

        #region READ

        [TestMethod]
        public void GetRandomMediaBadTagNullMediaType()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(-1, null)
                .ShouldCatchException<NotFoundException>("there is no resident that has tag -1");
        }

        [TestMethod]
        public void GetRandomMediaBadTagBadMediaType()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(-1, "badMediaType")
                .ShouldCatchException<NotFoundException>("there is no resident that has tag -1");
        }

        [TestMethod]
        public void GetRandomMediaBadTagExistingMediaType()
        {
            new ResidentsController(new Throw(), new TestResidentsService(), new ConsoleLogger())
                .GetRandomElementFromPropertyAsync(-1, EMediaType.Audio.ToString())
                .ShouldCatchException<NotFoundException>("there is no resident that has tag -1");
        }

        [TestMethod]
        public void GetRandomMediaExistingTagNullMediaType()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            var controller = new ResidentsController(new Throw(), dataService, new ConsoleLogger());

            controller
                .GetRandomElementFromPropertyAsync(tag, null)
                .ShouldCatchArgumentException<WebArgumentNullException>("the media type cannot be null");
        }

        [TestMethod]
        public void GetRandomMediaExistingTagBadMediaType()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            var controller = new ResidentsController(new Throw(), dataService, new ConsoleLogger());

            controller
                .GetRandomElementFromPropertyAsync(tag, "bad mediaType")
                .ShouldCatchException<NotFoundException>("the media type doesn't exist");
        }

        [TestMethod]
        public void GetRandomMediaExistingTagExistingMediaTyp()
        {
            var dataService = new TestResidentsService();
            var tag = dataService.GetFirst().Tags[0];

            var controller = new ResidentsController(new Throw(), dataService, new ConsoleLogger());

            var obj = controller.GetRandomElementFromPropertyAsync(tag, EMediaType.Image.ToString()).Result;
            obj.Should().NotBeNull("it holds an image");
        }

        #endregion READ

        #region UPDATE

        [TestMethod]
        public void UpdateNullItemNullProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateNullItemEmptyProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateNullItemSomeProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateBadItemNullProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateBadItemEmptyProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateBadItemSomeProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateExistingItemNullProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateExistingItemEmptyProperties()
        {
            // TODO create test
        }

        [TestMethod]
        public void UpdateExistingItemSomeProeprties()
        {
            // TODO create test
        }

        #endregion UPDATE

        #region DELETE

        [TestMethod]
        public void RemoveColorNullResidentNullColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorNullResidentBadColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorNullResidentExistingColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorBadResidentNullColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorBadResidentBadColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorBadResidentExistingColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorExistingResidentNullColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorExistingResidentBadColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveColorExistingResidentExistingColor()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaNullResidentNullMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaNullResidentBadMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaNullResidentExistingMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaBadResidentNullMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaBadResidentBadMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaBadResidentExistingMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaExistingResidentNullMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaExistingResidentBadMedia()
        {
            // TODO create test
        }

        [TestMethod]
        public void RemoveMediaExistingResidentExistingMedia()
        {
            // TODO create test
        }

        #endregion DELETE
    }
}