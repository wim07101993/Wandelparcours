using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
    public partial class MockMediaService
    {
        public override List<MediaData> MockData { get; } = new List<MediaData>
        {
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            },
            new MediaData
            {
                Id = ObjectId.GenerateNewId(),
                Data = Randomizer.Randomizer.Instance.Next(new byte[10])
            }
        };

    }
}
