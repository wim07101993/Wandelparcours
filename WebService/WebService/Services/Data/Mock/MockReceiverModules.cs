using System.Collections.Generic;
using MongoDB.Bson;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
    public partial class MockReceiverModulesesService
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
    }
}