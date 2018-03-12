using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestServices;
using WebAPIUnitTests.TestServices.Abstract;

namespace WebAPIUnitTests.ServiceTests.Data.Abstract
{
    [TestClass]
    public abstract partial class ADataServiceTest : IDataServiceTest
    {
        public abstract ITestDataService<TestEntity> CreateNewDataService();
    }
}
