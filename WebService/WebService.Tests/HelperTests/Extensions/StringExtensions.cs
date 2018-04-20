using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Tests.TestHelpers.Extensions;
using ArgumentException = WebService.Helpers.Exceptions.ArgumentException;

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
                .Be(
                    "helloWorld",
                    "spaces are ignored in camel case and only the first char after a space should be made upper case");

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
        public void ToUpperCamelCase()
        {
            "helloWorld"
                .ToUpperCamelCase()
                .Should()
                .Be("HelloWorld", "only the first letter is made upper case");

            "hello World"
                .ToUpperCamelCase()
                .Should()
                .Be(
                    "HelloWorld",
                    "spaces are ignored in camel case and only the first char after a space should be made upper case");

            "hello world"
                .ToUpperCamelCase()
                .Should()
                .Be("HelloWorld", "first letter and first after a space should become caps");
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
        public void EqualsToHash()
        {
            var id = ObjectId.GenerateNewId();
            var password = "password";

            var hash = password.Hash(id, false, false);
            password.EqualsToHash(id, hash, false, false)
                .Should()
                .BeTrue("it is the same password");

            hash = password.Hash(id, true, false);
            password.EqualsToHash(id, hash, true, false)
                .Should()
                .BeTrue("it is the same password");

            hash = password.Hash(id, false, true);
            password.EqualsToHash(id, hash, false, true)
                .Should()
                .BeTrue("it is the same password");

            hash = password.Hash(id, true, true);
            password.EqualsToHash(id, hash, true, true)
                .Should()
                .BeTrue("it is the same password");
        }

        [TestMethod]
        public void GetEMediaTypeFromExtension()
        {
            var extensions = new[] {"jpg", "bmp", "png", "jpeg", "gif", "webp", "some unknown extension"};

            foreach (var ext in extensions)
                ext
                    .GetEMediaTypeFromExtension()
                    .Should()
                    .Be(EMediaType.Image, "it is an image file extension");

            extensions = new[] {"midi", "mp3", "mpeg", "wav", "m4a", "aac"};

            foreach (var ext in extensions)
                ext
                    .GetEMediaTypeFromExtension()
                    .Should()
                    .Be(EMediaType.Audio, "it is an audio file extension");

            extensions = new[] {"mp4", "avi", "webm", "ogg", "flv", "wmv", "mkv",};

            foreach (var ext in extensions)
                ext
                    .GetEMediaTypeFromExtension()
                    .Should()
                    .Be(EMediaType.Video, "it is an video file extension");
        }

        [TestMethod]
        public void ToObjectId()
        {
            var id = ObjectId.GenerateNewId();
            id
                .ToString()
                .ToObjectId()
                .Should()
                .Be(id);

            ((Action) (() => "not an objectId".ToObjectId()))
                .ShouldCatchArgumentException<ArgumentException>(null, "it is not an object id");
        }
    }
}