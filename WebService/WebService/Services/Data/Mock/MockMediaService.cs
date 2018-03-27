using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

#pragma warning disable 1998

namespace WebService.Services.Data.Mock
{
    public class MockMediaService : AMockDataService<MediaData>, IMediaService
    {
        public override List<MediaData> MockData { get; } = Mock.MockData.MockMedia;

        public override MediaData CreateNewItem(ObjectId id)
            => new MediaData {Id = id};


        public async Task<byte[]> GetOneAsync(ObjectId id, string extension)
            => MockData
                .FirstOrDefault(x => x.Id == id && x.Extension == extension)
                ?.Data;
    }
}