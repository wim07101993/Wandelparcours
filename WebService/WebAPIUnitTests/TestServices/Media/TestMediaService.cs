using System.Collections.Generic;
using System.Linq;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;

namespace WebAPIUnitTests.TestServices.Media
{
    public class TestMediaService : MockMediaService, ITestMediaService
    {
        public MediaData GetFirst()
            => MockData.FirstOrDefault().Clone();

        public IEnumerable<MediaData> GetAll()
            => MockData.Clone();
    }
}