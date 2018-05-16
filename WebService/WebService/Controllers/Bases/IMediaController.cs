using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    public interface IMediaController
    {
        /// <summary>
        /// Fetches the asked media from the database.
        /// </summary>
        /// <param name="id">the id of the media</param>
        /// <param name="extension">the extension of the media</param>
        /// <param name="token">the authenticationtoken to identify the user. This can also be passed in the headers</param>
        /// <returns>The media with the passed id, wrapped in a <see cref="FileContentResult"/></returns>
        Task<FileStreamResult> GetOneAsync(string id, string extension, [FromQuery] string token);
        
        
        /// <summary>
        /// Fetches the asked media from the database.
        /// </summary>
        /// <param name="id">the id of the media</param>
        /// <param name="token">the authenticationtoken to identify the user. This can also be passed in the headers</param>
        /// <returns>The media with the passed id, wrapped in a <see cref="FileContentResult"/></returns>
        Task<FileStreamResult> GetFileAsync(string id, [FromQuery] string token);
    }
}