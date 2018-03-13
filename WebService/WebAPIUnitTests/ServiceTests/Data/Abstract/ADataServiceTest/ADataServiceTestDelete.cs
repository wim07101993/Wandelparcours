using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    public abstract partial class ADataServiceTest
    {
        #region Remove

        [TestMethod]
        public void RemoveUnknownItem()
        {
            ActionExtensions.ShouldCatchNotFoundException(
                () => { CreateNewDataService().RemoveAsync(ObjectId.GenerateNewId()).Wait(); },
                "the given id doesn't exist");
        }

        [TestMethod]
        public void RemoveKnownItem()
        {
            var dataService = CreateNewDataService();

            var id = dataService.GetFirst().Id;

            dataService.RemoveAsync(id).Wait();

            dataService
                .GetAll()
                .Should()
                .NotContain(x => x.Id == id, "it has just been deleted");
        }

        #endregion Remove
    }
}