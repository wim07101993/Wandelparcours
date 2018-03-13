using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ControllerTests.Abstract
{
    public partial class ARestControllerTests<T>
    {
        #region ConvertStringToSelector

        [TestMethod]
        public void ConvertNullStringToSelector()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ConvertUnknownStringToSelector()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ConvertStringToSelector()
        {
            throw new System.NotImplementedException();
        }

        #endregion ConvertStringToSelector

        #region ConvertStringsToSelectors

        [TestMethod]
        public void ConvertNullStringsToSelectors()
        {
            var controller = CreateNewController();

            ActionExtensions.ShouldCatchArgumentNullException(
                () => controller.ConvertStringsToSelectors(null),
                "propertyNames",
                "the propertyNames cannot be null");
        }

        [TestMethod]
        public void ConvertEmptyStringsToSelectors()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ConvertUnknownStringsToSelectors()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ConvertStringsToSelectorsWithSomeUnknownStrings()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ConvertStringsToSelectors()
        {
            throw new System.NotImplementedException();
        }

        #endregion ConvertStringsToSelectors
    }
}