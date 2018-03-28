using MongoDB.Bson;

namespace WebService.Controllers.Bases
{
    public interface IController
    {
        ObjectId UserId { get; set; }
    }
}
