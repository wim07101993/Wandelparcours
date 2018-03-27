using System;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Helpers.Exceptions;
using WebService.Models;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.ReceiverModules
{
    public abstract partial class AReceiverModulesServiceTest
    {
        [TestMethod]
        public void GetOneWithNullMacAndNoPropertiestToInclude()
        {
            ActionExtensions.ShouldCatchArgumentException<WebArgumentNullException>(
                () => CreateNewDataService().GetAsync(null).Wait(),
                "mac",
                "the mac address cannot be null");
        }

        [TestMethod]
        public void GetOneWithNullMacAndEmptyPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchArgumentException<WebArgumentNullException>(
                () => CreateNewDataService().GetAsync(null, new Expression<Func<ReceiverModule, object>>[] { })
                    .Wait(),
                "mac",
                "the mac address cannot be null");
        }

        [TestMethod]
        public void GetOneWithNullMacAndPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchArgumentException<WebArgumentNullException>(
                () => CreateNewDataService()
                    .GetAsync(null, new Expression<Func<ReceiverModule, object>>[] {x => x.Position})
                    .Wait(),
                "mac",
                "the mac address cannot be null");
        }

        [TestMethod]
        public void GetOneWithUnknownMacAndNoPropertiestToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService().GetAsync("").Wait(),
                "the given mac address doesn't exist in the database");
        }

        [TestMethod]
        public void GetOneWithUnknownMacAndEmptyPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService().GetAsync("", new Expression<Func<ReceiverModule, object>>[] { })
                    .Wait(),
                "the given mac address doesn't exist in the database");
        }

        [TestMethod]
        public void GetOneWithUnknownMacAndPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService()
                    .GetAsync("", new Expression<Func<ReceiverModule, object>>[] {x => x.Position})
                    .Wait(),
                "the given mac address doesn't exist in the database");
        }

        [TestMethod]
        public void GetOneWithKnownMacAndNoPropertiestToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            dataService
                .GetAsync(original.Mac).Result
                .Should()
                .BeEquivalentTo(original, "it is the same item and all properties are passed");
        }

        [TestMethod]
        public void GetOneWithKnownMacAndEmptyPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            var result = dataService
                .GetAsync(original.Mac, new Expression<Func<ReceiverModule, object>>[] { })
                .Result;

            var empty = new ReceiverModule();

            result
                .Mac
                .Should()
                .Be(original.Mac, "the mac address should always be passed");

            result
                .IsActive
                .Should()
                .Be(empty.IsActive, "when a property is not asked, it gets the default value");

            result
                .Position
                .Should()
                .BeEquivalentTo(empty.Position, "when a property is not asked, it gets the default value");
        }

        [TestMethod]
        public void GetOneWithKnownMacAndPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            var result = dataService
                .GetAsync(original.Mac, new Expression<Func<ReceiverModule, object>>[] {x => x.Position})
                .Result;

            var empty = new ReceiverModule();

            result
                .Mac
                .Should()
                .Be(original.Mac, "the mac address should always be passed");

            result
                .IsActive
                .Should()
                .Be(empty.IsActive, "when a property is not asked, it gets the default value");

            result
                .Position
                .Should()
                .BeEquivalentTo(original.Position, "it is asked by and should be the same as the original");
        }
    }
}