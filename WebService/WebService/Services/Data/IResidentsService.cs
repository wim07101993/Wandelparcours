using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data
{
    public interface IResidentsService : IDataService<Resident>
    {
        /// <summary>
        /// GetAsync is supposed to return the <see cref="Resident"/> that holds the given tag from the database. 
        /// <para/>
        /// It should only fill the properties passed in the <see cref="propertiesToInclude"/> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude"/> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="tag">is the tag of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the <see cref="Resident"/>s in the database.</returns>
        Task<Resident> GetAsync(int tag, IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        /// <summary>
        /// AddMediaAsync is supposed to add the <see cref="data"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        ///  with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/></param>
        /// <param name="data">is the data of the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        Task<bool> AddMediaAsync(ObjectId residentId, byte[] data, EMediaType mediaType);

        /// <summary>
        /// AddMediaAsync is supposed to add the <see cref="url"/> as media of the type <see cref="mediaType"/> to the <see cref="Resident"/>
        /// with as <see cref="Resident.Id"/> the passed <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> add the media to</param>
        /// <param name="url">is the url to the media to add</param>
        /// <param name="mediaType">is the type of media to add</param>
        /// <returns>
        /// - true if the media was added
        /// - false if the media was not added
        /// </returns>
        Task<bool> AddMediaAsync(ObjectId residentId, string url, EMediaType mediaType);

        /// <summary>
        /// RemoveMediaAsync is supposed to remove the media of type <see cref="mediaType"/> with as id <see cref="mediaId"/> of the
        /// <see cref="Resident"/> with as id <see cref="residentId"/>.
        /// </summary>
        /// <param name="residentId">is the id of the <see cref="Resident"/> to remove the media from</param>
        /// <param name="mediaId">is the id to the media to remove</param>
        /// <param name="mediaType">is the type fo media to remove</param>
        /// <returns>
        /// - true if the media was removed
        /// - false if the media was not removed
        /// </returns>
        Task<bool> RemoveMediaAsync(ObjectId residentId, ObjectId mediaId, EMediaType mediaType);
    }
}