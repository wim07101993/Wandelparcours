using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <summary>
    /// An interface that extends from the <see cref="IRestController{T}"/> interface with as generic type parameter
    /// <see cref="MediaData"/>.
    /// It is used to do the basic CRUD operations for the media of the residents.
    /// </summary>
    public interface IMediaController : IRestController<MediaData>
    {
        /// <summary>
        /// Fetches the asked media from the database.
        /// </summary>
        /// <param name="id">the id of the media</param>
        /// <param name="extension">the extension of the media</param>
        /// <param name="token">the authenticationtoken to identify the user. This can also be passed in the headers</param>
        /// <returns>The media with the passed id, wrapped in a <see cref="FileContentResult"/></returns>
        Task<FileContentResult> GetOneAsync(string id, string extension, [FromQuery] string token);
        
        
        /// <summary>
        /// Fetches the asked media from the database.
        /// </summary>
        /// <param name="id">the id of the media</param>
        /// <param name="token">the authenticationtoken to identify the user. This can also be passed in the headers</param>
        /// <returns>The media with the passed id, wrapped in a <see cref="FileContentResult"/></returns>
        Task<FileContentResult> GetFileAsync(string id, [FromQuery] string token);
    }
}