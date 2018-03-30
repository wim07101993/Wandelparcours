using MongoDB.Bson;

namespace DatabaseImporter.Models.MongoModels.Bases
{
    public interface IModelWithObjectID
    {
        ObjectId Id { get; set; }
    }
}