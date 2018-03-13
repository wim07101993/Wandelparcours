using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestControllers;
using WebAPIUnitTests.TestModels;

namespace WebAPIUnitTests.ControllerTests.Abstract
{
    [TestClass]
    public class TestControllerTest : ARestControllerTests<TestEntity>
    {

        public override ATestRestController<TestEntity> CreateNewController()
            => new TestRestController();

        public override void ConvertStringToSelector()
        {
            throw new System.NotImplementedException();
        }

        public override void ConvertUnknownStringsToSelectors()
        {
            throw new System.NotImplementedException();
        }

        public override void ConvertStringsToSelectorsWithSomeUnknownStrings()
        {
            throw new System.NotImplementedException();
        }

        public override void ConvertStringsToSelectors()
        {
            throw new System.NotImplementedException();
        }

        public override void CreateNull()
        {
            throw new System.NotImplementedException();
        }

        public override void CreateEmpty()
        {
            throw new System.NotImplementedException();
        }

        public override void Create()
        {
            throw new System.NotImplementedException();
        }
    }
}