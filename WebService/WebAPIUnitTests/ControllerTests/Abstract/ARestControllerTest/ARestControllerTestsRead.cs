using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ControllerTests.Abstract
{
    // ReSharper disable once UnusedTypeParameter
    public abstract partial class ARestControllerTests<T>
    {
        #region GetAll

        [TestMethod]
        public void GetAllWithoutPropertiesToInclude()
        {
            var controller = CreateNewController();
            var items = controller.GetAsync(null).Result;
            items
                .Should()
                .BeEquivalentTo(controller.GetAll());
        }

        [TestMethod]
        public void GetAllWithEmptyPropertiesToInclude()
        {
            var controller = CreateNewController();
            var items = controller.GetAsync(new string[] { }).Result;
            items
                .Should()
                .BeEquivalentTo(controller.GetAll().Select(x => x.Id));
        }

        [TestMethod]
        public void GetAllWithKnownPropertiesToInclude()
        {
            var controller = CreateNewController();
            var items = controller.GetAsync(new string[]{}).Result;
            items
                .Should()
                .BeEquivalentTo(controller.GetAll());
        }

        [TestMethod]
        public void GetAllWithUnknownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        #endregion GetAll


        #region GetByID

        [TestMethod]
        public void GetByNullIdAndNoPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByUnknownIdAndNoPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByKnownIdAndNoPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByWrongFormatIdAndNoPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByNullIdAndEmptyPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByUnknownIdAndEmptyPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByKnownIdAndEmptyPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByWrongFormatIdAndEmptyPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByNullIdAndKnownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByUnknownIdAndKnownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByKnownIdAndKnownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByWrongFormatIdAndKnownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByNullIdAndUnknownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByUnknownIdAndUnknownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByKnownIdAndUnknownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetByWrongFormatIdAndUnknownPropertiesToInclude()
        {
            throw new NotImplementedException();
        }

        #endregion GetByID


        #region GetProperty

        [TestMethod]
        public void GetNullPropertyByNullId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetNullPropertyByUnknownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetNullPropertyByKnownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetNullPropertyByWrongFormatId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetUnknownPropertyByNullId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetUnknownPropertyByUnknownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetUnknownPropertyByKnownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetUnknownPropertyByWrongFormatId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetKnownPropertyByNullId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetKnownPropertyByUnknownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetKnownPropertyByKnownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetKnownPropertyByWrongFormatId()
        {
            throw new NotImplementedException();
        }

        #endregion GetProperty
    }
}