using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
    public partial class MockMediaService : AMockDataService<MediaData>, IMediaService
    {
        public override MediaData CreateNewItem(ObjectId id)
            => new MediaData {Id = id};
    }
}