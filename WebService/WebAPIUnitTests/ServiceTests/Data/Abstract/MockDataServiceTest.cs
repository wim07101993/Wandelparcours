using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices;
using WebAPIUnitTests.TestServices.Abstract;

namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    [TestClass]
    public class MockDataServiceTest : ADataServiceTest
    {
        public override ITestDataService<TestEntity> CreateNewDataService()
            => new TestDataService();
    }
}