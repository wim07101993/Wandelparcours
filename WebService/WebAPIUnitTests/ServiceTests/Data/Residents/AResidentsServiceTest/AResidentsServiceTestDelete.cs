using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ServiceTests.Data.Residents
{
    public abstract partial class AResidentsServiceTest
    {
        [TestMethod]
        public void RemoveKnownMediaWithUnknownId()
        {
            // TODO
        }

        [TestMethod]
        public void RemoveKnownMediaWithKnownId()
        {
            var datatService = CreateNewDataService();

            var original = datatService.GetFirst();
            // TODO
        }

        [TestMethod]
        public void RemoveUnknownMediaWithKnownId()
        {
            var dataService = CreateNewDataService();

            var original = dataService.GetFirst();

            // TODO
        }
    }
}