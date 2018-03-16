using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices;
using WebAPIUnitTests.TestServices.Abstract;

namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    [TestClass]
    public class MongoDataServiceTest : ADataServiceTest
    {
        public override ITestDataService<TestEntity> CreateNewDataService()
            => new TestMongoDataService();
    }
}