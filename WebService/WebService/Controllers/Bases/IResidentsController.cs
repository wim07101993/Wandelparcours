using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Helpers.Exceptions;
using WebService.Models;

namespace WebService.Controllers.Bases
{
    /// <inheritdoc cref="IRestController{T}" />
    /// <summary>
    /// IResidentsController defines the methods the <see cref="ResidentsController"/> should have.
    /// </summary>
    public interface IResidentsController : IRestController<Resident>
    {
        /// <summary>
        /// Get is supposed to return the <see cref="Resident"/> with the given id in the database. 
        /// To limit data traffic it is possible to select only a number of properties
        /// </summary>
        /// <returns>The <see cref="Resident"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When the properties could not be converted to selectors</exception>
        Task<Resident> GetAsync(int tag, string[] properties);

        #region MUSIC

        /// <summary>
        /// AddMusicAsymc is supposed to add music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicData">is the music to add to the <see cref="Resident"/>'s music</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddMusicAsync(string residentId, [FromBody] byte[] musicData);

        /// <summary>
        /// AddMusicAsymc is supposed to add music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the music to add to the <see cref="Resident"/>'s music</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddMusicAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// RemoveMusicAsync is supposed to remove music from the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicId">is the id of the music to remove from the <see cref="Resident"/>'s music</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="musicId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        Task RemoveMusicAsync(string residentId, string musicId);

        #endregion MUSIC


        #region VIDEO

        /// <summary>
        /// AddVideoAsymc is supposed to add video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoData">is the video to add to the <see cref="Resident"/>'s video</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddVideoAsync(string residentId, [FromBody] byte[] videoData);

        /// <summary>
        /// AddVideoAsymc is supposed to add video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the video to add to the <see cref="Resident"/>'s video</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddVideoAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// RemoveVideoAsync is supposed to remove video from the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoId">is the id of the video to remove from the <see cref="Resident"/>'s video</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="videoId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        Task RemoveVideoAsync(string residentId, string videoId);

        #endregion VIDEO


        #region IMAGE

        /// <summary>
        /// AddImageAsymc is supposed to add image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageData">is the image to add to the <see cref="Resident"/>'s image</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddImageAsync(string residentId, [FromBody] byte[] imageData);

        /// <summary>
        /// AddImageAsymc is supposed to add image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the image to add to the <see cref="Resident"/>'s image</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddImageAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// RemoveImageAsync is supposed to remove image from the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageId">is the id of the image to remove from the <see cref="Resident"/>'s image</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="musicId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        Task RemoveImageAsync(string residentId, string imageId);

        #endregion IMAGE


        #region COLOR

        /// <summary>
        /// AddColorAsymc is supposed to add color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorData">is the color to add to the <see cref="Resident"/>'s color</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddColorAsync(string residentId, [FromBody] byte[] colorData);

        /// <summary>
        /// AddColorAsymc is supposed to add color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the color to add to the <see cref="Resident"/>'s color</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        Task AddColorAsync(string residentId, [FromBody] string url);

        /// <summary>
        /// RemoveColorAsync is supposed to remove color from the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorId">is the id of the color to remove from the <see cref="Resident"/>'s color</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="musicId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaWithId"/> not found
        /// </exception>
        Task RemoveColorAsync(string residentId, string colorId);

        #endregion COLOR
    }
}