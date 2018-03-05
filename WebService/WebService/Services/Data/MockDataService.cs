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
            //new Resident
            //{
            //    Id = new ObjectId("5a9566c58b9ed54db08d0ce7"),
            //    FirstName = "Lea",
            //    LastName = "Thuwis",
            //    Room = "AT109 A",
            //    Birthday = new DateTime(1937, 4, 8),
            //    Doctor = new Doctor
            //    {
            //        Name = "Massimo Destino",
            //        PhoneNumber = "089 84 29 87"
            //    }
            //},
            //new Resident
            //{
            //    Id = new ObjectId("5a95677d8b9ed54db08d0ce8"),
            //    FirstName = "Martha",
            //    LastName = "Schroyen",
            //    Room = "AT109 A",
            //    Birthday = new DateTime(1929, 5, 26),
            //    Doctor = new Doctor
            //    {
            //        Name = "Luc Euben",
            //        PhoneNumber = "089 38 51 57"
            //    }
            //},
            //new Resident
            //{
            //    Id = new ObjectId("5a9568328b9ed54db08d0ce9"),
            //    FirstName = "Roland",
            //    LastName = "Mertens",
            //    Room = "AQ230 A",
            //    Birthday = new DateTime(1948, 9, 19),
            //    Doctor = new Doctor
            //    {
            //        Name = "Peter Potargent",
            //        PhoneNumber = "089 35 26 87"
            //    }
            //},
            //new Resident
            //{
            //    Id = new ObjectId("5a9568838b9ed54db08d0cea"),
            //    FirstName = "Maria",
            //    LastName = "Creces",
            //    Room = "SA347 A",
            //    Birthday = new DateTime(1934, 1, 26),
            //    Doctor = new Doctor
            //    {
            //        Name = "Willy Denier - Medebo",
            //        PhoneNumber = "089 35 47 22"
            //    }
            //},
            //new Resident
            //{
            //    Id = new ObjectId("5a967fc4c45be323bc42b5d8"),
            //    FirstName = "Ludovica",
            //    LastName = "Van Houten",
            //    Room = "AQ468 A",
            //    Birthday = new DateTime(1933, 1, 25),
            //    Doctor = new Doctor
            //    {
            //        Name = "Marcel Mellebeek",
            //        PhoneNumber = "089 65 74 85"
            //    }
            //},
        };


        /// <inheritdoc cref="IDataService.GetResidents" />
        /// <summary>
        /// Get returns all the residents from the mock list. 
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
                    // create new newResident to return with the ide filled in
                    var residentToReturn = new Resident {Id = mockResident.Id};

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

                    // return the newResident
                    return residentToReturn;
                });
        }

        /// <inheritdoc cref="IDataService.CreateResident" />
        /// <summary>
        /// Create saves the passed <see cref="Resident"/> to the list.
        /// <para/>
        /// If the newResident is created, the method returns the id of the new <see cref="Resident"/>, else null.
        /// </summary>
        /// <param name="resident">is the <see cref="Resident"/> to save in the list</param>
        /// <returns>
        /// - the new id if the <see cref="Resident"/> was created in the list
        /// - null if the newResident was not created
        /// </returns>
     
        public string CreateResident(Resident resident)
        {
            // create a new ide for the resident
            resident.Id = new ObjectId();
            // add the new resident to the list
            MockData.Add(resident);

            // check if the resident was created

            return MockData.Any(x => x.Id == resident.Id)
                // if it is, return the id
                ? resident.Id.ToString()
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
        /// - false if the newResident was not removed
        /// </returns>
        public bool RemoveResident(ObjectId id)
        {
            // get the index of the newResident with the given id
            var index = MockData.FindIndex(x => x.Id == id);

            // if the index is -1 there was no item found
            if (index == -1)
                return false;

            // remove the newResident
            MockData.RemoveAt(index);
            return true;
        }

        /// <inheritdoc cref="IDataService.UpdateResident" />
        /// <summary>
        /// UpdateResident updates the <see cref="Resident" /> with the id of the given <see cref="Resident" />.
        /// <para />
        /// The updated properties are defined in the <see cref="propertiesToUpdate" /> parameter.
        /// If the <see cref="!:propertiesToUpdate" /> parameter is null or empty (which it is by default), all properties are updated.
        /// </summary>
        /// <param name="newResident">is the <see cref="Resident" /> to update</param>
        /// <param name="propertiesToUpdate">are the properties that need to be updated</param>
        /// <returns>The updated newResident</returns>
        public Resident UpdateResident(Resident newResident,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToUpdate = null)
        {
            // create list of the enumerable to prevent multiple enumerations of enumerable
            var propertiesToUpdateList = propertiesToUpdate?.ToList();

            var index = MockData.FindIndex(x => x.Id == newResident.Id);
            if (index < 0)
                return null;


            // check if thereare properties to update.
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToUpdateList))
            {
                // update the resident;
                MockData[index] = newResident;
                // return the updated resident
                return MockData.FirstOrDefault(x => x.Id == newResident.Id);
            }

            // ReSharper disable once PossibleNullReferenceException
            // iterate over all the properties that need to be updated
            foreach (var selector in propertiesToUpdateList)
            {
                // get the property
                var prop = selector.Body is MemberExpression expression
                    // via member expression
                    ? expression.Member as PropertyInfo
                    // if that failse, unary expression
                    : ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member as PropertyInfo;

                // check if the property exists
                if (prop != null)
                    // if it does, add the selector and value to the updateDefinition
                    prop.SetValue(MockData[index], prop.GetValue(newResident));
            }

            // return the updated resident
            return MockData.FirstOrDefault(x => x.Id == newResident.Id);
        }

        public IEnumerable<ReceiverModule> GetReceiverModules()
        {
            throw new NotImplementedException();
        }

        public string CreateReceiverModule(ReceiverModule receiver)
        {
            throw new NotImplementedException();
        }

        public bool RemoveReceiverModule(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public ReceiverModule UpdateReceiverModule(ReceiverModule newReceiverModule)
        {
            throw new NotImplementedException();
        }
    }
}