using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Models;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockResidentsService
    {
        #region Get

        [TestMethod]
        public void GetByTag()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();
            var resident = dataService.MockData[0];

            dataService.GetAsync(resident.Tags.ToList()[0]).Result
                .Should()
                .BeEquivalentTo(resident, "that resident has the asked id");
        }

        [TestMethod]
        public void GetByNonExistingTag()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            dataService.GetAsync(-1).Result
                .Should()
                .BeNull("Ther is not resident with that id");
        }

        #endregion Get

        #region Add media

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
            const string url = "someurl";

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

        #endregion Add media

        #region Remove media

        [TestMethod]
        public void RemoveMedia()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();
            const string url = "someURl";

            var _ = dataService.AddMediaAsync(dataService.MockData[0].Id, url, EMediaType.Audio).Result;

            dataService.RemoveMediaAsync(
                dataService.MockData[0].Id,
                dataService.MockData[0].Music[0].Id,
                EMediaType.Audio).Result
                .Should()
                .BeTrue("the item to remove exists and should be removed. When that happens, true should be returned");
        }

        [TestMethod]
        public void RemoveMediaNonExistingResident()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            dataService.RemoveMediaAsync(
                    ObjectId.GenerateNewId(),
                    ObjectId.GenerateNewId(),
                    EMediaType.Audio).Result
                .Should()
                .BeFalse("the resident to remove the item from doesn't exist");
        }

        [TestMethod]
        public void RemoveMediaNonExistingMedia()
        {
            var dataService = new WebService.Services.Data.Mock.MockResidentsService();

            dataService.RemoveMediaAsync(
                    dataService.MockData[0].Id,
                    ObjectId.GenerateNewId(),
                    EMediaType.Audio).Result
                .Should()
                .BeFalse("the media to remove the item from doesn't exist");
        }

        #endregion Remove media
    }
}