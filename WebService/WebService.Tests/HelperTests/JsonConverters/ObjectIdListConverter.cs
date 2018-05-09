using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using WebService.Helpers.Extensions;

namespace WebService.Tests.HelperTests.JsonConverters
{
    [TestClass]
    public class ObjectIdListConverter
    {
        private class TestClass
        {
            [JsonConverter(typeof(Helpers.JsonConverters.ObjectIdListConverter))]
            public IEnumerable<ObjectId> ObjectIds { get; set; }
        }

        [TestMethod]
        public void Serialize()
        {
            var id1 = ObjectId.GenerateNewId();
            var id2 = ObjectId.GenerateNewId();
            var id3 = ObjectId.GenerateNewId();

            var obj = new TestClass {ObjectIds = new[] {id1, id2, id3}};

            var str = obj.Serialize();
            str
                .Should()
                .Contain(id1.ToString(), "it is one of the ids in the original list")
                .And.Contain(id2.ToString(), "it is one of the ids in the original list")
                .And.Contain(id3.ToString(), "it is one of the ids in the original list");

            str.Deserialize<TestClass>()
                .ObjectIds
                .Should()
                .Contain(id1, "it is one of the ids in the original list")
                .And.Contain(id2, "it is one of the ids in the original list")
                .And.Contain(id3, "it is one of the ids in the original list");
        }
    }
}