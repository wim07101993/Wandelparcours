using MongoDB.Bson;

namespace WebService.Controllers.Bases
{
    public interface IController
    {
        ObjectId CurrentUserId { get; set; }
    }
}