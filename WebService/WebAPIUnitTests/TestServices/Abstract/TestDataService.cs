using System.Collections.Generic;
using MongoDB.Bson;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.TestServices.Abstract
{
    public class TestDataService : AMockDataService<TestEntity>, ITestDataService<TestEntity>
    {
        public override List<TestEntity> MockData { get; } = TestData.TestEntities.Clone();

        public override TestEntity CreateNewItem(ObjectId id)
            => new TestEntity {Id = id};

        public TestEntity GetFirst()
            => !EnumerableExtensions.IsNullOrEmpty(MockData) ? MockData[0] : null;

        public IEnumerable<TestEntity> GetAll()
            => MockData?.Clone();
    }
}