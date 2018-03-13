using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebService.Helpers.Exceptions;

namespace WebAPIUnitTests.TestHelpers.Extensions
{
    public static class ActionExtensions
    {
        public static void ShouldCatchArgumentNullException(this Action action, string paramName, string because)
        {
            try
            {
                action();
                Assert.Fail($"a null argument exception should have been thrown because {because}");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentNullException)?.ParamName == paramName)
                    .Should()
                    .BeTrue($"at least one exception should be an argument null exception since {because}");
            }
            catch (ArgumentNullException e)
            {
                e.ParamName
                    .Should()
                    .Be(paramName, because);
            }
        }

        public static void ShouldCatchNotFoundException(this Action action, string because)
        {
            try
            {
                action();
                Assert.Fail($"a not found exception should have been thrown because {because}");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => x is NotFoundException)
                    .Should()
                    .BeTrue($"at least one exception should be a not found exception since {because}");
            }
            catch (NotFoundException)
            {
                // IGNORED => this should happen
            }
        }

        public static void ShouldCatchArgumentException(this Action action, string paramName, string because)
        {
            try
            {
                action();
                Assert.Fail($"a argument exception should have been thrown because {because}");
            }
            catch (AggregateException e)
            {
                e.InnerExceptions
                    .Any(x => (x as ArgumentException)?.ParamName == paramName)
                    .Should()
                    .BeTrue($"at least one exception should be an argument exception since {because}");
            }
            catch (ArgumentException e)
            {
                e.ParamName
                    .Should()
                    .Be(paramName, because);
            }
        }
    }
}