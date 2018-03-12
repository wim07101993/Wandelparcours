using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.TestMocks.Mock
{
    public class MockDataService : AMockDataService<MockEntity>
    {
        public override List<MockEntity> MockData { get; } = TestMocks.MockData.MockEntities;

        public override MockEntity CreateNewItem(ObjectId id)
            => new MockEntity {Id = id};
    }
}