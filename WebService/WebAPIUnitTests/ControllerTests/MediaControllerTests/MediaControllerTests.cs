using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.Media;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.MediaControllerTests
{
    [TestClass]
    public class MediaControllerTests : IMediaControllerTests
    {
        [TestMethod]
        public void GetNullId()
        {
            var controller = new MediaController(new Throw(), new TestMediaService(), new ConsoleLogger());

            controller
                .GetAsync(null)
                .ShouldCatchException<NotFoundException>("there is no item with id null");
        }

        [TestMethod]
        public void GetBadId()
        {
            var controller = new MediaController(new Throw(), new TestMediaService(), new ConsoleLogger());

            controller
                .GetAsync(ObjectId.GenerateNewId().ToString())
                .ShouldCatchException<NotFoundException>("there is no item with the given id null");
        }

        [TestMethod]
        public void GetExistingId()
        {
            var dataService = new TestMediaService();

            var controller = new MediaController(new Throw(), dataService, new ConsoleLogger());

            var item = dataService.GetFirst();

            controller
                .GetAsync(item.Id.ToString())
                .Result
                .Should()
                .BeOfType<FileContentResult>()
                .And
                .Subject
                .As<FileContentResult>()
                .FileContents
                .Should()
                .BeEquivalentTo(item.Data);
        }
    }
}