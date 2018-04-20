using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPIUnitTests.TestHelpers.Extensions
{
    public static class ActionExtensions
    {
        public static void ShouldCatchException<T>(this Action action, string because = "") where T : Exception
        {
            try
            {
                action();
                Assert.Fail($"a {typeof(T).Name} should have been thrown because {because}");
            }
            catch (Exception e)
            {
                if (e is AggregateException aggregateException)
                    aggregateException.InnerExceptions
                        .Should()
                        .Contain(x => x is T, $"at least one exception should be a {typeof(T).Name} since {because}");
                else
                    e.Should()
                        .BeAssignableTo<T>(because);
            }
        }

        public static void ShouldCatchArgumentException<T>(this Action action, string paramName, string because = "")
            where T : ArgumentException
        {
            try
            {
                action();
                Assert.Fail($"a {typeof(T).Name} should have been thrown because {because}");
            }
            catch (Exception e)
            {
                if (e is AggregateException aggregateException)
                    aggregateException.InnerExceptions
                        .Should()
                        .Contain(x => x is T && ((T) x).ParamName == paramName,
                            $"at least one exception should be a {typeof(T).Name} since {because}");
                else
                    e.Should()
                        .BeAssignableTo<T>(because)
                        .And
                        .Subject
                        .As<T>()
                        .ParamName
                        .Should()
                        .Be(paramName, because);
            }
        }
    }
}