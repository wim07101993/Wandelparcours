using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;
using WebService.Services.Exceptions;

#pragma warning disable 1998

namespace WebService.Services.Data.Mock
{
    public partial class MockMediaService : AMockDataService<MediaData>, IMediaService
    {
        public MockMediaService(IThrow iThrow) : base(iThrow)
        {
        }

        public override MediaData CreateNewItem(ObjectId id)
            => new MediaData {Id = id};


        public async Task<byte[]> GetAsync(ObjectId id, string extension)
            => MockData
                .FirstOrDefault(x => x.Id == id && x.Extension == extension)
                ?.Data;
    }
}