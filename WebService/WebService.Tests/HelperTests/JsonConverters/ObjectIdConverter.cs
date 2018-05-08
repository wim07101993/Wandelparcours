using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using WebService.Helpers.Extensions;

namespace WebService.Tests.HelperTests.JsonConverters
{
    [TestClass]
    public class ObjectIdConverter
    {
        private class TestClass
        {
            [JsonConverter(typeof(Helpers.JsonConverters.ObjectIdConverter))]
            public ObjectId ObjectId { get; set; }
        }

        [TestMethod]
        public void Serialize()
        {
            var obj = new TestClass {ObjectId = ObjectId.GenerateNewId()};

            var str = obj.Serialize();
            str
                .Should()
                .Contain(obj.ObjectId.ToString(), "it is the original id");

            str.Deserialize<TestClass>()
                .ObjectId
                .Should()
                .Be(obj.ObjectId, "it is the original id");
        }
    }
}