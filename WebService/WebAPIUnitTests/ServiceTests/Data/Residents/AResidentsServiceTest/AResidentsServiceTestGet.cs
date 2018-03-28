using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebService.Helpers.Exceptions;
using WebService.Models;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public abstract partial class AResidentsServiceTest
    {
        [TestMethod]
        public void GetOneByUnknownTagAndNoPropertiestToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService().GetOneAsync(-1).Wait(),
                "the given tag address doesn't exist in the database");
        }

        [TestMethod]
        public void GetOneByUnknownTagAndEmptyPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService().GetOneAsync(-1, new Expression<Func<Resident, object>>[] { }).Wait(),
                "the given tag address doesn't exist in the database");
        }

        [TestMethod]
        public void GetOneByUnknownTagAndPropertiesToInclude()
        {
            ActionExtensions.ShouldCatchException<NotFoundException>(
                () => CreateNewDataService()
                    .GetOneAsync(-1, new Expression<Func<Resident, object>>[] {x => x.FirstName, x => x.LastName})
                    .Wait(),
                "the given tag address doesn't exist in the database");
        }


        [TestMethod]
        public void GetOneByKnownTagAndNoPropertiestToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            dataService
                .GetOneAsync(original.Id).Result
                .Should()
                .BeEquivalentTo(original, "it is the same item and all properties are passed");
        }

        [TestMethod]
        public void GetOneByKnownTagAndEmptyPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            var result = dataService
                .GetOneAsync(original.Tags.First(), new Expression<Func<Resident, object>>[] { })
                .Result;

            var empty = new Resident();

            result
                .Id
                .Should()
                .Be(original.Id, "the id address should always be passed");

            var props = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id));

            foreach (var prop in props)
                prop.GetValue(result)
                    .Should()
                    .BeEquivalentTo(prop.GetValue(empty), "when a property is not asked, it gets the default value");
        }

        [TestMethod]
        public void GetOneByKnownTagAndPropertiesToInclude()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            var result = dataService
                .GetOneAsync(original.Tags.First(), new Expression<Func<Resident, object>>[]
                {
                    x => x.FirstName,
                    x => x.LastName
                })
                .Result;

            var empty = new Resident();

            result
                .Id
                .Should()
                .Be(original.Id, "the id address should always be passed");

            var props = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id) &&
                            x.Name != nameof(Resident.FirstName) &&
                            x.Name != nameof(Resident.LastName));

            foreach (var prop in props)
                prop.GetValue(result)
                    .Should()
                    .BeEquivalentTo(prop.GetValue(empty), "when a property is not asked, it gets the default value");
        }
    }
}