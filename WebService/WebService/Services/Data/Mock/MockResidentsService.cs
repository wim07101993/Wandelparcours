using System;
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
    public class MockResidentsService : AMockDataService<Resident>
    {
        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// MockData is the list of residents to test the application.
        /// </summary>
        public override List<Resident> MockData { get; } = new List<Resident>
        {
            new Resident
            {
                Id = new ObjectId("5a9566c58b9ed54db08d0ce7"),
                FirstName = "Lea",
                LastName = "Thuwis",
                Room = "AT109 A",
                Birthday = new DateTime(1937, 4, 8),
                Doctor = new Doctor
                {
                    Name = "Massimo Destino",
                    PhoneNumber = "089 84 29 87"
                }
            },
            new Resident
            {
                Id = new ObjectId("5a95677d8b9ed54db08d0ce8"),
                FirstName = "Martha",
                LastName = "Schroyen",
                Room = "AT109 A",
                Birthday = new DateTime(1929, 5, 26),
                Doctor = new Doctor
                {
                    Name = "Luc Euben",
                    PhoneNumber = "089 38 51 57"
                }
            },
            new Resident
            {
                Id = new ObjectId("5a9568328b9ed54db08d0ce9"),
                FirstName = "Roland",
                LastName = "Mertens",
                Room = "AQ230 A",
                Birthday = new DateTime(1948, 9, 19),
                Doctor = new Doctor
                {
                    Name = "Peter Potargent",
                    PhoneNumber = "089 35 26 87"
                }
            },
            new Resident
            {
                Id = new ObjectId("5a9568838b9ed54db08d0cea"),
                FirstName = "Maria",
                LastName = "Creces",
                Room = "SA347 A",
                Birthday = new DateTime(1934, 1, 26),
                Doctor = new Doctor
                {
                    Name = "Willy Denier - Medebo",
                    PhoneNumber = "089 35 47 22"
                }
            },
            new Resident
            {
                Id = new ObjectId("5a967fc4c45be323bc42b5d8"),
                FirstName = "Ludovica",
                LastName = "Van Houten",
                Room = "AQ468 A",
                Birthday = new DateTime(1933, 1, 25),
                Doctor = new Doctor
                {
                    Name = "Marcel Mellebeek",
                    PhoneNumber = "089 65 74 85"
                }
            },
        };


        /// <inheritdoc cref="AMockDataService{T}" />
        /// <summary>
        /// CreateNewItems should return a new item of the given type <see cref="Resident" /> with as Id, <see cref="id" />.
        /// </summary>
        /// <param name="id">is the id for the new object</param>
        /// <returns>A new object of type <see cref="Resident" /></returns>
        public override Resident CreateNewItem(ObjectId id)
            => new Resident {Id = id};
    }
}