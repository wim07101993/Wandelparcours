using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Models;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public abstract partial class AResidentsServiceTest
    {
        [TestMethod]
        public void AddByteMediaWithUnknownId()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                    CreateNewDataService()
                        .AddMediaAsync(ObjectId.GenerateNewId(), new byte[] {13, 123, 234}, EMediaType.Color)
                        .Wait();
                },
                "there is no resident with the given id");
        }

        [TestMethod]
        public void AddNullByteMediaWithUnknownId()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                   CreateNewDataService()
                        .AddMediaAsync(ObjectId.GenerateNewId(), null as byte[], EMediaType.Color)
                        .Wait();
                },
                "data",
                "data cannot be null");
        }

        [TestMethod]
        public void AddByteMediaWithKnownId()
        {
            var dataService = CreateNewDataService();

            dataService
                .AddMediaAsync(dataService.GetFirst().Id, new byte[] {13, 123, 234}, EMediaType.Color)
                .Wait();

            dataService
                .GetFirst()
                .Colors
                .Should()
                .Contain(x => x.Data != null && x.Data[0] == 13 && x.Data[1] == 123 && x.Data[2] == 234, "that color has just been added");
        }

        [TestMethod]
        public void AddNullByteMediaWithKnownId()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                    dataService
                        .AddMediaAsync(dataService.GetFirst().Id, null as byte[], EMediaType.Color)
                        .Wait();
                },
                "data",
                "the data cannot be null");
        }

        [TestMethod]
        public void AddUrlMediaWithUnknownId()
        {
            ActionExtensions.ShouldCatchNotFoundException(() =>
                {
                   CreateNewDataService()
                        .AddMediaAsync(ObjectId.GenerateNewId(), "abcd", EMediaType.Color)
                        .Wait();
                },
                "there is no resident with the given id");
        }

        [TestMethod]
        public void AddNullUrlMediaWithUnknownId()
        {
            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                   CreateNewDataService()
                        .AddMediaAsync(ObjectId.GenerateNewId(), null as string, EMediaType.Color)
                       .Wait();
                },
                "url",
                "the url cannot be null");
        }

        [TestMethod]
        public void AddUrlMediaWithKnownId()
        {
            var dataService = CreateNewDataService();

            dataService
                .AddMediaAsync(dataService.GetFirst().Id, "abcd", EMediaType.Color)
                .Wait();

            dataService
                .GetFirst()
                .Colors
                .Should()
                .Contain(x => x.Url == "abcd", "that color has just been added");
        }

        [TestMethod]
        public void AddNullUrlMediaWithKnownId()
        {
            var dataService = CreateNewDataService();

            ActionExtensions.ShouldCatchArgumentNullException(() =>
                {
                   dataService
                        .AddMediaAsync(dataService.GetFirst().Id, null as string, EMediaType.Color)
                        .Wait();
                },
                "url",
                "the url cannot be null");
        }
    }
}