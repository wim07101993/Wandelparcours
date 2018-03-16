using System.Collections.Generic;
using MongoDB.Bson;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.ReceiverModules
{
    public class TestReceiverModulesService : MockReceiverModulesesService, ITestReceiverModulesService
    {
        public TestReceiverModulesService() : base(new Throw())
        {
            
        }

        public override List<ReceiverModule> MockData { get; } = TestData.TestReceiverModules.Clone();

        public override ReceiverModule CreateNewItem(ObjectId id)
            => new ReceiverModule { Id = id };

        public ReceiverModule GetFirst()
            => !EnumerableExtensions.IsNullOrEmpty(MockData) ? MockData[0].Clone() : null;

        public IEnumerable<ReceiverModule> GetAll()
            => MockData?.Clone();
    }
}
