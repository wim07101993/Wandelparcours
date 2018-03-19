using System.Collections.Generic;
using System.Linq;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.Media
{
    public class TestMediaService : MockMediaService, ITestMediaService
    {
        public TestMediaService() : base(new Throw())
        {
        }

        public MediaData GetFirst()
            => MockData.FirstOrDefault().Clone();

        public IEnumerable<MediaData> GetAll()
            => MockData.Clone();

    }
}