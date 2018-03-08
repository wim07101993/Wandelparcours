using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
#pragma warning disable CS1998 // disable warning async methods that not use await operator
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Residents in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public partial class MockReceiverModulesService : AMockDataService<ReceiverModule>, IReceiverModuleService
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="ReceiverModule" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="ReceiverModule" /></returns>
        public override ReceiverModule CreateNewItem(ObjectId id)
            => new ReceiverModule {Id = id};

        /// <inheritdoc cref="IReceiverModuleService.GetAsync(string)" />
        /// <summary>
        /// GetAsync should return the receiver module with the given mac.
        /// </summary>
        /// <param name="mac">is the mac address of the receiver module to fetch</param>
        /// <returns>The receiver module with the given mac</returns>
        public async Task<ReceiverModule> GetAsync(string mac)
            => MockData.FirstOrDefault(x => x.Mac == mac);

        /// <inheritdoc cref="IReceiverModuleService.RemoveAsync(string)" />
        /// <summary>
        /// Remove removes the <see cref="ReceiverModule"/> with the given mac from the database.
        /// </summary>
        /// <param name="mac">is the mac of the <see cref="ReceiverModule"/> to remove in the database</param>
        /// <returns>
        /// - true if the <see cref="ReceiverModule"/> was removed from the database
        /// - false if the item was not removed
        /// </returns>
        public async Task<bool> RemoveAsync(string mac)
        {
            // get the index of the newItem with the given id
            var index = MockData.FindIndex(x => x.Mac == mac);

            // if the index is -1 there was no item found
            if (index == -1)
                return false;

            // remove the newItem
            MockData.RemoveAt(index);
            return true;
        }
    }
#pragma warning restore CS1998
}