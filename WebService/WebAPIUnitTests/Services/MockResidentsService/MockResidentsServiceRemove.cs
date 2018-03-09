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
    }
}
