using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.Media;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.MediaControllerTests
{
    [TestClass]
    public class MediaControllerTests : IMediaControllerTests
    {
        [TestMethod]
        public void GetNullId()
        {
            //new MediaController(new Throw(), new Mock<IMediaService>().Object, new ConsoleLogger())
            //    .GetOneAsync(null, null)
            //    .ShouldCatchException<NotFoundException>("there is no item with id null");
        }

        [TestMethod]
        public void GetBadId()
        {
            //var controller = new MediaController(new Throw(), new Mock<IMediaService>().Object, new ConsoleLogger());

            //controller
            //    .GetOneAsync("a", null)
            //    .ShouldCatchException<NotFoundException>("there is no item with the given id null");
        }

        [TestMethod]
        public void Get()
        {
            //var dataService = new TestMediaService();

            //var controller = new MediaController(new Throw(), dataService, new ConsoleLogger());

            //var item = dataService.GetFirst();

            //controller
            //    .GetOneAsync(item.Id.ToString(), null)
            //    .Result
            //    .Should()
            //    .BeOfType<FileContentResult>()
            //    .And
            //    .Subject
            //    .As<FileContentResult>()
            //    .FileContents
            //    .Should()
            //    .BeEquivalentTo(item.Data);
        }
    }
}