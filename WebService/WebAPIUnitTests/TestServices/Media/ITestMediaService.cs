using WebAPIUnitTests.TestServices.Abstract;
using WebService.Models;
using WebService.Services.Data;

namespace WebAPIUnitTests.TestServices.Media
{
    public interface ITestMediaService : ITestDataService<MediaData>, IMediaService
    {
    }
}