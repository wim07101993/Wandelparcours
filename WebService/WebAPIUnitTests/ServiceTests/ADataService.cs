using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestMocks;

namespace WebAPIUnitTests.ServiceTests
{
    [TestClass]
    public abstract partial class ADataService
    {
        public abstract IMockDataService CreateNewDataService();
    }
}
