﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.MediaControllerTests
{
    public class MediaControllerTests : IMediaControllerTests
    {
        [TestMethod]
        public void GetNullId()
        {
            var controller = new MediaController(new Throw(), new Mock<IDataService<MediaData>>().Object,
                new ConsoleLogger());

            controller.CreateAsync(null)
                .ShouldCatchArgumentException<WebArgumentNullException>("item", "the item to create cannot be null");
        }

        [TestMethod]
        public void GetBadId()
        {
            // TODO create test
        }

        [TestMethod]
        public void GetExistingId()
        {
            // TODO create test
        }
    }
}