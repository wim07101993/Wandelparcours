using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Models;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services
{
    public partial class MockResidentsService
    {
        [TestMethod]
        public void AddByteMedia()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();
            var bytes = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

            dataService
                .AddMediaAsync(dataService.MockData[0].Id, bytes, EMediaType.Audio).Result
                .Should()
                .BeTrue("the media should be added and when that happens, true should be returned");

            dataService.MockData[0]
                .Music
                .Any(x => x.Data == bytes)
                .Should()
                .BeTrue("the media should be added to the resident");
        }

        [TestMethod]
        public void AddBytesMediaNonExistingResident()
        {
            try
            {
                var _ = new WebService.Services.Data.Mock.MockResidentsService()
                    .AddMediaAsync(new ObjectId(), new byte[] { }, EMediaType.Audio)
                    .Result;

                Assert.Fail("cannot create element null");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == "data")
                    .Should()
                    .BeTrue(
                        "because at least one exception should be an argument null exception and have the data parameter, that is the parameter that is null");
            }

            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            dataService
                .AddMediaAsync(new ObjectId(), new byte[] { }, EMediaType.Audio).Result
                .Should()
                .BeFalse("the resident to add the media to doesn't exist");
        }

        [TestMethod]
        public void AddUrlMedia()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();
            const string url = "someUrl";

            dataService
                .AddMediaAsync(dataService.MockData[0].Id, url, EMediaType.Audio).Result
                .Should()
                .BeTrue("the media should be added and when that happens, true should be returned");

            dataService.MockData[0]
                .Music
                .Any(x => x.Url == url)
                .Should()
                .BeTrue("the media should be added to the resident");
        }

        [TestMethod]
        public void AddBytesUrlNonExistingResident()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            dataService
                .AddMediaAsync(new ObjectId(), "", EMediaType.Audio).Result
                .Should()
                .BeFalse("the resident to add the media to doesn't exist");
        }
    }
}