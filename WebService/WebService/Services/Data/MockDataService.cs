using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models;

namespace WebService.Services.Data
{
    /// <inheritdoc cref="IDataService"/>
    /// <summary>
    /// MockDataService is a class that implements the <see cref="IDataService"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Residents in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class MockDataService

    {
        /// <summary>
        /// MockData is the list of residents to test the application.
        /// </summary>
        public List<Resident> MockData { get; set; } = new List<Resident>
        {
            new Resident
            {
                ID = new ObjectId("5a9566c58b9ed54db08d0ce7"),
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
                ID = new ObjectId("5a95677d8b9ed54db08d0ce8"),
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
                ID = new ObjectId("5a9568328b9ed54db08d0ce9"),
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
                ID = new ObjectId("5a9568838b9ed54db08d0cea"),
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
                ID = new ObjectId("5a967fc4c45be323bc42b5d8"),
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


        /// <inheritdoc cref="IDataService.GetResidents" />
        /// <summary>
        /// GetResidents returns all the residents from the mock list. 
        /// <para />
        /// It only fills the properties passed in the <see cref="propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="propertiesToInclude" /> parameter is null (which it is by default), all the properties are included.
        /// Other properties are given their default value. 
        /// </summary>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="IEnumerable{T}"/> filled with all the residents in the mock list.</returns>
        public IEnumerable<Resident> GetResidents(
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var propertiesToIncludeList = propertiesToInclude?.ToList();
            return EnumerableExtensions.IsNullOrEmpty(propertiesToIncludeList)
                // if the properties to include are null, return the complete list
                ? MockData
                // else return a list with only the asked properties filled
                : MockData.Select(mockResident =>
                {
                    // create new resident to return with the ide filled in
                    var residentToReturn = new Resident {ID = mockResident.ID};

                    // ReSharper disable once PossibleNullReferenceException
                    // go over each property selector that should be included
                    foreach (var selector in propertiesToIncludeList)
                    {
                        // get property
                        var prop = selector.Body is MemberExpression expression
                            // via member expression
                            ? expression.Member as PropertyInfo
                            // via unary expression
                            : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                        // set the value of the property with the value of the mockResident
                        prop?.SetValue(residentToReturn, prop.GetValue(mockResident));
                    }

                    // return the resident
                    return residentToReturn;
                });
        }

        /// <inheritdoc cref="IDataService.CreateResident" />
        /// <summary>
        /// CreateResident saves the passed <see cref="Resident"/> to the list.
        /// <para/>
        /// If the resident is created, the method returns the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the list</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the list
        /// - null if the resident was not created
        /// </returns>
        public string CreateResident(Resident resident)
        {
            // create a new ide for the resident
            resident.ID = new ObjectId();
            // add the new resident to the list
            MockData.Add(resident);

            // check if the resident was created
            return MockData.Any(x => x.ID == resident.ID)
                // if it is, return the id
                ? resident.ID.ToString()
                // else return null
                : null;
        }

        /// <inheritdoc cref="IDataService.RemoveResident" />
        /// <summary>
        /// RemoveResident removes the <see cref="Resident"/> with the given id from the list.
        /// </summary>
        /// <param name="id">is the id of the <see cref="Resident"/> to remove in the list</param>
        /// <returns>
        /// - true if the <see cref="Resident"/> was removed from the list
        /// - false if the resident was not removed
        /// </returns>
        public bool RemoveResident(ObjectId id)
        {
            // get the index of the resident with the given id
            var index = MockData.FindIndex(x => x.ID == id);

            // if the index is -1 there was no item found
            if (index == -1)
                return false;

            // remove the resident
            MockData.RemoveAt(index);
            return true;
        }
    }
}