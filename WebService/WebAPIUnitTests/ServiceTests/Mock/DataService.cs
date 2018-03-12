using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestMocks;
using WebAPIUnitTests.TestMocks.Mock;

namespace WebAPIUnitTests.ServiceTests.Mock
{
    [TestClass]
    public class DataService : ADataService
    {
        public override IMockDataService CreateNewDataService()
            => new MockDataService();
    }
}