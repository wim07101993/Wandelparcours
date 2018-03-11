using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Services.Data.Mock;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.Services.Mock
{
    public partial class ResidentsService
    {
        #region Get

        [TestMethod]
        public void GetByTag()
        {
            var dataService = new MockResidentsService();
            var resident = dataService.MockData[0];

            dataService.GetAsync(resident.Tags.ToList()[0]).Result
                .Should()
                .BeEquivalentTo(resident, "that resident has the asked id");
        }

        [TestMethod]
        public void GetByNonExistingTag()
        {
            var dataService = new MockResidentsService();

            dataService.GetAsync(-1).Result
                .Should()
                .BeNull("There is not resident with that id");
        }

        #endregion Get
    }
}
