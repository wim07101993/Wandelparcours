using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ControllerTests.Abstract
{
    public abstract partial class ARestControllerTests<T>
    {
        [TestMethod]
        public void DeleteNull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteUnknownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteKnownId()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteWrongFormatId()
        {
            throw new NotImplementedException();
        }
    }
}