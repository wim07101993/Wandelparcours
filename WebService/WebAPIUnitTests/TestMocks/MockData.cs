using System.Collections.Generic;
using MongoDB.Bson;
using WebAPIUnitTests.TestMocks.Mock;

namespace WebAPIUnitTests.TestMocks
{
    public static class MockData
    {
        public static List<MockEntity> MockEntities { get; } = new List<MockEntity>
        {
            new MockEntity {B = true, I = 64, Id = ObjectId.GenerateNewId(), S = "Hello"},
            new MockEntity {B = true, I = 44, Id = ObjectId.GenerateNewId(), S = "Bumbabelu"},
            new MockEntity {B = false, I = 42, Id = ObjectId.GenerateNewId(), S = "Bam"},
        };
    }
}