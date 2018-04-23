using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using WebService.Helpers.Extensions;

namespace WebService.Tests.HelperTests.Extensions
{
    [TestClass]
    public class ObejctExtensions
    {
        public class TestClass
        {
            public string S { get; set; }
            public int I { get; set; }
            public int[] Ints { get; set; }
            public TestClass Class { get; set; }
            public bool B { get; set; }
        }

        
        [TestMethod]
        public void SerializeObject()
        {
            var obj = new TestClass
            {
                B = false,
                Class = new TestClass(),
                I = 5,
                Ints = new[] {5, 6, 7, 9, 2, 7, 2, 3},
                S = "Hello test"
            };

            obj.Serialize()
                .Should()
                .Be(JsonConvert.SerializeObject(obj));
        }

        [TestMethod]
        public void DeserializeObject()
        {
            var obj = new TestClass
            {
                B = false,
                Class = new TestClass(),
                I = 5,
                Ints = new[] { 5, 6, 7, 9, 2, 7, 2, 3 },
                S = "Hello test"
            };

            var serialized = JsonConvert.SerializeObject(obj);
            
            serialized.Deserialize<TestClass>()
                .Should()
                .BeEquivalentTo(obj);
        }

        [TestMethod]
        public void CloneObject()
        {
            var obj = new TestClass
            {
                B = false,
                Class = new TestClass(),
                I = 5,
                Ints = new[] { 5, 6, 7, 9, 2, 7, 2, 3 },
                S = "Hello test"
            };

            var cloned = obj.Clone();

            cloned
                .Should()
                .BeEquivalentTo(obj);
            cloned
                .Should()
                .NotBeSameAs(obj);
        }
    }
}