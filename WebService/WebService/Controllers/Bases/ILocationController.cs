using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// An interface that extends from the <see cref="IRestController{T}"/> interface with as generic type parameter
    /// <see cref="ResidentLocation"/>.
    /// It is used to do the basic CRUD operations for the locations of the residents.
    /// </summary>
    public interface ILocationController : IRestController<ResidentLocation>
    {
    }
}