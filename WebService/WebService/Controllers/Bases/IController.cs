using MongoDB.Bson;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// An interface that holds the id of the user that made the request.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// The id of the user that made the request.
        /// </summary>
        ObjectId CurrentUserId { get; set; }
    }
}