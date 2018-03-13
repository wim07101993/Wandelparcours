using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ControllerTests.Abstract
{
    // ReSharper disable once UnusedTypeParameter
    public abstract partial class ARestControllerTests<T>
    {
        [TestMethod]
        public void DeleteNull()
        {
            ActionExtensions.ShouldCatchWebArgumentException(
                () => CreateNewController().DeleteAsync(null).Wait(),
                "id",
                "the id cannot be null");
        }

        [TestMethod]
        public void DeleteUnknownId()
        {
            ActionExtensions.ShouldCatchWebArgumentException(
                () => CreateNewController().DeleteAsync(null).Wait(),
                ObjectId.GenerateNewId().ToString(),
                "the id cannot be null");
        }

        [TestMethod]
        public void DeleteKnownId()
        {
            var controller = CreateNewController();
            var item = controller.GetAsync(null).Result.First();
            controller.DeleteAsync(item.Id.ToString()).Wait();

            ActionExtensions.ShouldCatchNotFoundException(
                () => controller.GetAsync(item.Id.ToString(), null).Wait(),
                "we just removed the item");
        }

        [TestMethod]
        public void DeleteWrongFormatId()
        {
            ActionExtensions.ShouldCatchWebArgumentException(
                () => CreateNewController().DeleteAsync(null).Wait(),
                "wrong format",
                "the id must be convertible to an object id");
        }
    }
}