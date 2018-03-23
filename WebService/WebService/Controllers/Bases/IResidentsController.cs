using System;
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
        #region CREATE

        /// <summary>
        /// AddMusicAsymc is supposed to add music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicData">is the music to add to the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromForm] MultiPartFile musicData);

        /// <summary>
        /// AddMusicAsymc is supposed to add music to the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the music to add to the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddMusicAsync(string residentId, [FromBody] string url);


        /// <summary>
        /// AddVideoAsymc is supposed to add video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoData">is the video to add to the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromForm] MultiPartFile videoData);

        /// <summary>
        /// AddVideoAsymc is supposed to add video to the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the video to add to the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddVideoAsync(string residentId, [FromBody] string url);


        /// <summary>
        /// AddImageAsymc is supposed to add image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageData">is the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddImageAsync(string residentId, [FromForm] MultiPartFile imageData);

        /// <summary>
        /// AddImageAsymc is supposed to add image to the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="url">is the url to the image to add to the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddImageAsync(string residentId, [FromBody] string url);


        /// <summary>
        /// AddColorAsymc is supposed to add color to the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorData">is the color to add to the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">When the <see cref="residentId"/> cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="Exception">When the item could not be added</exception>
        Task<StatusCodeResult> AddColorAsync(string residentId, [FromBody] byte[] colorData);

        #endregion CREATE


        #region READ

        /// <summary>
        /// GetByTag is supposed to return the <see cref="Resident"/> that holds a given tag in the database. 
        /// To limit data traffic it is possible to select only a number of properties.
        /// <para/>
        /// By default all properties are returned.
        /// </summary>
        /// <param name="tag">is the tag of the <see cref="Resident"/> to get</param>
        /// <param name="propertiesToInclude">are the properties of which the values should be returned</param>
        /// <returns>The <see cref="Resident"/> in the database that has the given id</returns>
        /// <exception cref="NotFoundException">When the id cannot be parsed or <see cref="Resident"/> not found</exception>
        /// <exception cref="WebArgumentException">When one ore more properties could not be converted to selectors</exception>
        Task<Resident> GetByTagAsync(int tag, string[] propertiesToInclude);

        Task<object> GetRandomElementFromPropertyAsync(int tag, string mediaType);

        Task<object> GetPropertyAsync(int tag, string propertyName);

        #endregion READ


        #region DELETE

        /// <summary>
        /// RemoveMusicAsync is supposed to remove music from the music list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="musicId">is the id of the music to remove from the <see cref="Resident"/>'s music list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="musicId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaData"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        Task RemoveMusicAsync(string residentId, string musicId);

        /// <summary>
        /// RemoveVideoAsync is supposed to remove video from the video list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="videoId">is the id of the video to remove from the <see cref="Resident"/>'s video list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="videoId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaData"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        Task RemoveVideoAsync(string residentId, string videoId);

        /// <summary>
        /// RemoveImageAsync is supposed to remove image from the image list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="imageId">is the id of the image to remove from the <see cref="Resident"/>'s image list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="imageId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaData"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        Task RemoveImageAsync(string residentId, string imageId);

        /// <summary>
        /// RemoveColorAsync is supposed to remove color from the color list of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="colorId">is the id of the color to remove from the <see cref="Resident"/>'s color list</param>
        /// <exception cref="NotFoundException">
        /// When the <see cref="residentId"/>/<see cref="colorId"/> cannot be parsed or <see cref="Resident"/>/<see cref="MediaData"/> not found
        /// </exception>
        /// <exception cref="Exception">When the item could not be removed</exception>
        Task RemoveColorAsync(string residentId, string colorId);

        #endregion DELETE
    }
}