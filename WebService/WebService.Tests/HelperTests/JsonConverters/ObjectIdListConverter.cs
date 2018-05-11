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
            var id1 = new ObjectId("5af5426acbe1a920c0f6b507");
            var id2 = new ObjectId("5af5426acbe1a920c0f6b508");
            var id3 = new ObjectId("5af5426acbe1a920c0f6b509");

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