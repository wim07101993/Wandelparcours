using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.Mocks
{
    public class MockDataService : AMockDataService<MockEntity>
    {
        public override List<MockEntity> MockData { get; } = new List<MockEntity>
        {
            new MockEntity {B = false, I = 64, Id = ObjectId.GenerateNewId(), S = "Hello"},
            new MockEntity {B = true, I = 44, Id = ObjectId.GenerateNewId(), S = "Bumbabelu"},
            new MockEntity {B = false, I = 42, Id = ObjectId.GenerateNewId(), S = "Bam"},
        };

        public override MockEntity CreateNewItem(ObjectId id)
            => new MockEntity {Id = id};
    }
}