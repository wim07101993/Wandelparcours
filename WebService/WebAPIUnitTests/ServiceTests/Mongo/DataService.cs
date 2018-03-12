using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestMocks;
using WebAPIUnitTests.TestMocks.Mongo;

namespace WebAPIUnitTests.ServiceTests.Mongo
{
    [TestClass]
    public class DataService : ADataService
    {
        public override IMockDataService CreateNewDataService()
            => new MongoDataService();
    }
}