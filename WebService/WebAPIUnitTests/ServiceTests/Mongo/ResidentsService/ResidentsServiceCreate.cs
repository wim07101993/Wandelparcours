using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Models;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mongo
{
    public partial class ResidentsService
    {
        [TestMethod]
        public void AddMediaBytes()
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
        public void AddMediaNullBytes()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = dataService
                        .AddMediaAsync(dataService.MockData[0].Id, null as byte[], EMediaType.Audio)
                        .Result;
                },
                "data",
                "data cannot be null");
        }

        [TestMethod]
        public void AddBytesMediaNonExistingResident()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new WebService.Services.Data.Mock.MockResidentsService()
                        .AddMediaAsync(new ObjectId(), new byte[] { }, EMediaType.Audio)
                        .Result;
                },
                "the resident cannot be found");
        }

        [TestMethod]
        public void AddMediaUrl()
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
        public void AddMediaNullUrl()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    var _ = dataService
                        .AddMediaAsync(dataService.MockData[0].Id, null as string, EMediaType.Audio)
                        .Result;
                },
                "url",
                "url cannot be null");
        }

        [TestMethod]
        public void AddBytesUrlNonExistingResident()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = new WebService.Services.Data.Mock.MockResidentsService()
                        .AddMediaAsync(new ObjectId(), "", EMediaType.Audio)
                        .Result;
                },
                "the resident cannot be found");
        }
    }
}