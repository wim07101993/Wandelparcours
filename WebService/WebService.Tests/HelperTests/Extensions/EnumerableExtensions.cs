using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Helpers.Extensions;
using WebService.Tests.TestHelpers.Extensions;

namespace WebService.Tests.HelperTests.Extensions
{
    [TestClass]
    public class EnumerableExtensions
    {
        #region IsNullOrEmpty

        [TestMethod]
        public void IsNullOrEmptyOneItem()
            => Helpers.Extensions.EnumerableExtensions
                .IsNullOrEmpty(new[] {1})
                .Should()
                .BeFalse("we gave a list with one item (that is not empty or null)");

        [TestMethod]
        public void IsNullOrEmptyMoreItems()
            => Helpers.Extensions.EnumerableExtensions
                .IsNullOrEmpty(new[] {"a", "hello world", "test string"})
                .Should()
                .BeFalse("we gave a list with three item (that is not empty or null)");

        [TestMethod]
        public void IsNullOrEmptyEmpty()
            => Helpers.Extensions.EnumerableExtensions
                .IsNullOrEmpty(new int[] { })
                .Should()
                .BeTrue("we gave a list with no items (that is empty)");

        [TestMethod]
        public void IsNullOrEmptyNull()
            => Helpers.Extensions.EnumerableExtensions
                .IsNullOrEmpty(null as IEnumerable<int>)
                .Should()
                .BeTrue("we gave null");

        #endregion IsNullOrEmpty


        #region RandomItem

        [TestMethod]
        public void RandomItemOfEmptyList()
            => ((Action) (() => new List<int>().RandomItem()))
                .ShouldCatchException<IndexOutOfRangeException>("there are no items in the list");

        [TestMethod]
        public void RandomItem()
        {
            var list = new[] {546, 879, 453, 564, 231, 1564, 89431, 286, 7897, 42, 45, 89, 464, 4, 94, 894, 34, 5};
            var random = (int) list.RandomItem();

            list
                .Should()
                .Contain(random, "the item is a random item from that list");
        }

        #endregion RandomItem


        #region Remove

        [TestMethod]
        public void RemoveFalsePredicate()
        {
            var list = new List<int>
                {546, 879, 453, 564, 231, 1564, 89431, 286, 7897, 42, 45, 89, 464, 4, 94, 894, 34, 5};

            list
                .Remove(x => false)
                .Should()
                .BeFalse("the predicate is never true");

            list
                .Count
                .Should()
                .Be(18, "no item has been removed");
        }

        [TestMethod]
        public void RemoveTruePredicate()
        {
            var list = new List<int>
                {546, 879, 453, 564, 231, 1564, 89431, 286, 7897, 42, 45, 89, 464, 4, 94, 894, 34, 5};

            list
                .Remove(x => true)
                .Should()
                .BeTrue("the predicate is always true");

            list
                .Count
                .Should()
                .Be(17, "one item should be removed");
        }

        [TestMethod]
        public void RmoveExistingItem()
        {
            var list = new List<int>
                {546, 879, 453, 564, 231, 1564, 89431, 286, 7897, 42, 45, 89, 464, 4, 94, 894, 34, 5};

            list
                .Remove(x => x == 42)
                .Should()
                .BeTrue("the number 42 exists in the list");

            list
                .Count
                .Should()
                .Be(17, "one item should be removed");

            list
                .Should()
                .NotContain(42, "that is the item that has been removed");
        }

        #endregion Remove
    }
}