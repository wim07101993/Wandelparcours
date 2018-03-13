using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestModels;
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