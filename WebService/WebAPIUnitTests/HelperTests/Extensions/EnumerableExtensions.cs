using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPIUnitTests.HelperTests.Extensions
{
    [TestClass]
    public class EnumerableExtensions
    {
        [TestMethod]
        public void IsNullOrEmptyOneItem()
        {
            var list = new List<int> {1};
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should()
                .BeFalse("we gave a list with one item (that is not empty or null)");
        }

        [TestMethod]
        public void IsNullOrEmptyMoreItems()
        {
            var list = new List<string> {"a", "hello world", "test string"};
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should()
                .BeFalse("we gave a list with three item (that is not empty or null)");
        }

        [TestMethod]
        public void IsNullOrEmptyEmpty()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var list = new List<int>();
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should()
                .BeTrue("we gave a list with no items (that is empty)");
        }

        [TestMethod]
        public void IsNullOrEmptyNull()
        {
            List<int> list = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(list)
                .Should()
                .BeTrue("we gave null");
        }
    }
}