using WebAPIUnitTests.TestServices.Abstract;
using WebService.Models;
using WebService.Services.Data;

namespace WebAPIUnitTests.TestServices.ReceiverModules
{
    public interface ITestReceiverModulesService : ITestDataService<ReceiverModule>, IReceiverModulesService
    {
    }
}