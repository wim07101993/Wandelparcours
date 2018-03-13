using WebAPIUnitTests.TestServices.Abstract;
using WebService.Models;
using WebService.Services.Data;

namespace WebAPIUnitTests.TestServices.Residents
{
    public interface ITestResidentsService : ITestDataService<Resident>, IResidentsService
    {
    }
}