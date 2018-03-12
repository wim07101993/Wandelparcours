using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.TestMocks.Mock
{
    public class MockDataService : AMockDataService<MockEntity>, IMockDataService
    {
        public override List<MockEntity> MockData { get; } = TestMocks.MockData.MockEntities.Clone();

        public override MockEntity CreateNewItem(ObjectId id)
            => new MockEntity {Id = id};

        public MockEntity GetFirst()
            => !EnumerableExtensions.IsNullOrEmpty(MockData) ? MockData[0] : null;

        public IEnumerable<MockEntity> GetAll()
            => MockData?.Clone();
    }
}