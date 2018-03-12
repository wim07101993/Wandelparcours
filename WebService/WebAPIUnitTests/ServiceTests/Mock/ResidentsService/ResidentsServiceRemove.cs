using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
    public partial class ResidentsService
    {
        [TestMethod]
        public void RemoveMedia()
        {
            var dataService = new MockResidentsService();
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
            var dataService = new MockResidentsService();

            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = dataService.RemoveMediaAsync(
                            ObjectId.GenerateNewId(),
                            ObjectId.GenerateNewId(),
                            EMediaType.Audio)
                        .Result;
                },
                "there is no resident with the given id");
        }

        [TestMethod]
        public void RemoveMediaNonExistingMedia()
        {
            var dataService = new MockResidentsService();

            dataService.MockData[0].Music = new List<MediaWithId>();

            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = dataService.RemoveMediaAsync(
                        dataService.MockData[0].Id,
                        ObjectId.GenerateNewId(),
                        EMediaType.Audio).Result;
                },
                "there is no media with the given id");
        }

        [TestMethod]
        public void RemoveMediaNonExistingMediaCollection()
        {
            var dataService = new MockResidentsService();

            dataService.MockData[0].Music = new List<MediaWithId>();

            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    var _ = dataService.RemoveMediaAsync(
                        dataService.MockData[0].Id,
                        ObjectId.GenerateNewId(),
                        EMediaType.Audio).Result;
                },
                "the given resident has no music");
        }
    }
}