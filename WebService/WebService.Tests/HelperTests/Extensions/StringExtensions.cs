using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers.Extensions;

namespace WebService.Tests.HelperTests.Extensions
{
    [TestClass]
    public class StringExtensions
    {
        [TestMethod]
        public void ToLowerCamelCase()
        {
            "HelloWorld"
                .ToLowerCamelCase()
                .Should()
                .Be("helloWorld", "the lower camel case vairant starts with a lower case letter");

            "hello World"
                .ToLowerCamelCase()
                .Should()
                .Be("helloWorld", "spaces are ignored in camel case and only the first char after a space should be made upper case");

            "HELLOWORLD"
                .ToLowerCamelCase()
                .Should()
                .Be("hELLOWORLD", "only the first letter is made lower case");

            "hello world"
                .ToLowerCamelCase()
                .Should()
                .Be("helloWorld", "first letter after a space should become caps");
        }

        [TestMethod]
        public void EqualsWithCamelCasing()
        {
            "HelloWorld"
                .EqualsWithCamelCasing("HelloWorld")
                .Should()
                .BeTrue("it is the same string content");

            "helloWorld"
                .EqualsWithCamelCasing("HelloWorld")
                .Should()
                .BeTrue("only the first letter is in caps and in camelcasing that caracter is ignored");

            "Hello World"
                .EqualsWithCamelCasing("HelloWorld")
                .Should()
                .BeTrue("spaces are removed in camelcasing");

            "hello world"
                .EqualsWithCamelCasing("helloWorld")
                .Should()
                .BeTrue(
                    "spaces are removed in camelcasing and the words after spaces should start with a capital letter");

            "helloworld"
                .EqualsWithCamelCasing("helloWorld")
                .Should()
                .BeFalse("these strings are different: the 'w' is in one with caps and the other without");
        }

        [TestMethod]
        public void Hash()
        {
            var id = ObjectId.GenerateNewId();
            var password = "password";

            var hash = password.Hash(id, false, false);
            password.EqualsToHash(id, hash, false, false)
                .Should().BeTrue("it is the same password");

            hash = password.Hash(id, true, false);
            password.EqualsToHash(id, hash, true, false)
                .Should().BeTrue("it is the same password");

            hash = password.Hash(id, false, true);
            password.EqualsToHash(id, hash, false, true)
                .Should().BeTrue("it is the same password");

            hash = password.Hash(id, true, true);
            password.EqualsToHash(id, hash, true, true)
                .Should().BeTrue("it is the same password");
        }
    }
}