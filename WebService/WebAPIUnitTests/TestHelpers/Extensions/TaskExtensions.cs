using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIUnitTests.TestHelpers.Extensions
{
    public static class TaskExtensions
    {
        public static void ShouldCatchException<T>(this Task This, string because = "") where T : Exception
            => ActionExtensions.ShouldCatchException<T>(This.Wait, because);

        public static void ShouldCatchArgumentException<T>(this Task This, string paramName, string because = "")
            where T : ArgumentException
            => ActionExtensions.ShouldCatchArgumentException<T>(This.Wait, paramName, because);

        public static void ShouldReturnStatus(this Task<StatusCodeResult> This, HttpStatusCode statusCode,
            string because = "")
            => This.Result.StatusCode.Should().Be((int) statusCode, because);
    }
}