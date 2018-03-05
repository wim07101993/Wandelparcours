using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPIUnitTests.Helpers
{
    [TestClass]
    public class EnumerableExtensions
    {
        [TestMethod]
        public void IsNullOrEmptyOneItem()
        {
            var list = new List<int> {1};
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should().BeFalse();
        }

        [TestMethod]
        public void IsNullOrEmptyMoreItems()
        {
            var list = new List<string> {"a", "hello world", "test string"};
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should().BeFalse();
        }

        [TestMethod]
        public void IsNullOrEmptyEmpty()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var list = new List<int>();
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should().BeTrue();
        }

        [TestMethod]
        public void IsNullOrEmptyNull()
        {
            List<int> list = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should().BeTrue();
        }
    }
}