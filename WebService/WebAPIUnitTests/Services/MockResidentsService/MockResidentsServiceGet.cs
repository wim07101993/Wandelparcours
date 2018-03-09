using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services
{
    public partial class MockResidentsService
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
    }
}
