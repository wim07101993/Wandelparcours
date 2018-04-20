using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebService.Tests.HelperTests.Extensions
{
    [TestClass]
    public class EnumerableExtensions
    {
        [TestMethod]
        public void IsNullOrEmptyOneItem()
        {
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(new[] {1})
                .Should()
                .BeFalse("we gave a list with one item (that is not empty or null)");
        }

        [TestMethod]
        public void IsNullOrEmptyMoreItems()
        {
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(new[] {"a", "hello world", "test string"})
                .Should()
                .BeFalse("we gave a list with three item (that is not empty or null)");
        }

        [TestMethod]
        public void IsNullOrEmptyEmpty()
        {
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(new int[] { })
                .Should()
                .BeTrue("we gave a list with no items (that is empty)");
        }

        [TestMethod]
        public void IsNullOrEmptyNull()
        {
            WebService.Helpers.Extensions.EnumerableExtensions.IsNullOrEmpty(null as IEnumerable<int>)
                .Should()
                .BeTrue("we gave null");
        }
    }
}