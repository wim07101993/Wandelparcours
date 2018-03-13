using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace WebAPIUnitTests.ControllerTests.Abstract
{
    // ReSharper disable once UnusedTypeParameter
    public abstract partial class ARestControllerTests<T>
    {
        [TestMethod]
        public abstract void CreateNull();

        [TestMethod]
        public abstract void CreateEmpty();

        [TestMethod]
        public abstract void Create();
    }
}