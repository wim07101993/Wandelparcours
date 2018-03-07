using MongoDB.Bson;

namespace WebService.Models.Bases
{
    public interface IModelWithID
    {
        ObjectId Id { get; set; }
    }
}