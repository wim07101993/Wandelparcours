using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Helpers.Extensions;

namespace WebAPIUnitTests.Helpers.Extensions
{
    [TestClass]
    public class TypeExtensions
    {
        [TestMethod]
        public void GetDefault()
        {
            typeof(bool)
                .GetDefault()
                .Should()
                .Be(default(bool), $"the default value of a bool is {default(bool)}");

            typeof(byte)
                .GetDefault()
                .Should()
                .Be(default(byte), $"the default value of a byte is  {default(byte)}");

            typeof(char)
                .GetDefault()
                .Should()
                .Be(default(char), $"the default value of a char is  {default(char)}");

            typeof(decimal)
                .GetDefault()
                .Should()
                .Be(default(decimal), $"the default value of a decimal is  {default(decimal)}");

            typeof(double)
                .GetDefault()
                .Should()
                .Be(default(double), $"the default value of a double is  {default(double)}");

            typeof(float)
                .GetDefault()
                .Should()
                .Be(default(float), $"the default value of a float is  {default(float)}");

            typeof(int)
                .GetDefault()
                .Should()
                .Be(default(int), $"the default value of a int is  {default(int)}");

            typeof(long)
                .GetDefault()
                .Should()
                .Be(default(long), $"the default value of a long is  {default(long)}");

            typeof(object)
                .GetDefault()
                .Should()
                .Be(default(object), $"the default value of a object is  {default(object)}");

            typeof(DateTime)
                .GetDefault()
                .Should()
                .Be(default(DateTime), $"the default value of a DateTime is  {default(DateTime)}");
        }
    }
}
