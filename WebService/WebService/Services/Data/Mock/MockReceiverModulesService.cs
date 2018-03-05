using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Residents in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class MockReceiverModulesService : AMockDataService<ReceiverModule>
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// MockData is the list of items to test the application.
        /// </summary>
        public override List<ReceiverModule> MockData { get; } = new List<ReceiverModule>
        {
            new ReceiverModule
            {
                Id = new ObjectId("5a996594c081860934bcadef"),
                IsActive = true,
                Mac = "dd:dd:dd:dd:dd:dd",
                Position = new Point
                {
                    X = 0.200073229417303,
                    Y = 0.395857307249712,
                }
            },
            new ReceiverModule
            {
                Id = new ObjectId("5a996a5dab36bd0804a0f986"),
                IsActive = true,
                Mac = "dd:dd:dd:dd:dd:12",
                Position = new Point
                {
                    X = 0.4,
                    Y = 0.8,
                }
            },
            new ReceiverModule
            {
                Id = new ObjectId("5a996f25ddc3c03954d2586f"),
                IsActive = false,
                Mac = "ad:aa:aa:aa:aa:aa",
                Position = new Point
                {
                    X = 0.622053872053872,
                    Y = 0.392156862745098,
                }
            },
        };


        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="ReceiverModule" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="ReceiverModule" /></returns>
        public override ReceiverModule CreateNewItem(ObjectId id)
            => new ReceiverModule {Id = id};
    }
}