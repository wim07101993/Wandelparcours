using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Interface that describes a class that provides basic CRUD operations for <see cref="T:WebService.Models.Resident" /> in a database.
    /// </summary>
    public interface IResidentsService : IDataService<Resident>
    {
        /// <summary>
        /// Adds media to a resident by adding data.
        /// </summary>
        /// <param name="residentId">Id of the resident to add the media to.</param>
        /// <param name="title">Title of the media to add</param>
        /// <param name="data">Data of the media to add</param>
        /// <param name="mediaType">Type of media to add</param>
        /// <param name="extension">Extension of the data to add</param>
        Task AddMediaAsync(ObjectId residentId, string title, Stream data, EMediaType mediaType,
            string extension = null);

        /// <summary>
        /// Adds media to a resident by adding an url.
        /// </summary>
        /// <param name="residentId">Id of the resident to add the media to</param>
        /// <param name="url">Url to the media to add</param>
        /// <param name="mediaType">Typ of media to add</param>
        Task AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType);


        /// <summary>
        /// Gets all <see cref="Resident"/>s with the ids specified in the <see cref="objectIds"/> parameter.
        /// </summary>
        /// <param name="objectIds">Ids of the <see cref="Resident"/>s to get</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The <see cref="Resident"/>s with the specified ids</returns>
        Task<IEnumerable<Resident>> GetMany(IEnumerable<ObjectId> objectIds,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets a single <see cref="Resident"/> from the database by tag.
        /// </summary>
        /// <param name="tag">Tag of the <see cref="Resident"/> to get</param>
        /// <param name="propertiesToInclude">
        /// The properties to include in the query (if it is null, all properties are passed).
        /// </param>
        /// <returns>The <see cref="Resident"/> with the given tag</returns>
        Task<Resident> GetOneAsync(int tag, IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        /// <summary>
        /// Gets a property from a specific <see cref="Resident"/> in the database.
        /// </summary>
        /// <param name="tag">Id of the <see cref="Resident"/> to get the property from</param>
        /// <param name="propertyToSelect">The property to get from the <see cref="Resident"/></param>
        /// <typeparam name="TValue">The type of the property to get</typeparam>
        /// <returns>The property from the <see cref="Resident"/> with id <see cref="tag"/></returns>
        Task<TValue> GetPropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToSelect);

        /// <summary>
        /// Gets the highest tag number of all <see cref="Resident"/>s.
        /// </summary>
        /// <returns>The highest tag number of all <see cref="Resident"/>s.</returns>
        Task<int> GetHighestTagNumberAsync();


        /// <summary>
        /// Updates a property of a <see cref="Resident"/>.
        /// </summary>
        /// <param name="tag">Tag of the <see cref="Resident"/> to update the property of.</param>
        /// <param name="propertyToUpdate">The property of to update.</param>
        /// <param name="value">The value to update the property to.</param>
        /// <typeparam name="TValue">The type of the property to update</typeparam>
        Task UpdatePropertyAsync<TValue>(int tag, Expression<Func<Resident, TValue>> propertyToUpdate, TValue value);


        /// <summary>
        /// Removes the media with the given id from the <see cref="Resident"/>s media.
        /// </summary>
        /// <param name="residentId">Id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="mediaId">Id of the media to remove</param>
        /// <param name="mediaType">Type of media to remove</param>
        /// <returns></returns>
        Task RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType);

        /// <summary>
        /// Removes a color from a <see cref="Resident"/>s colors.
        /// </summary>
        /// <param name="residentId">Id of the <see cref="Resident"/> to remove the color from</param>
        /// <param name="color">Color to remove</param>
        Task RemoveColor(ObjectId residentId, Color color);
    }
}