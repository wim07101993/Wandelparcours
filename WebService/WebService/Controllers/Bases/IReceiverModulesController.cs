using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// IReceiverModulesController defines the methods for the <see cref="ReceiverModule"/> REST controller.
    /// </summary>
    public interface IReceiverModulesController : IRestController<ReceiverModule>
    {
    }
}