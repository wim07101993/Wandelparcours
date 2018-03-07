using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mock
{
#pragma warning disable 1998 // diable warning async method without await
    /// <inheritdoc cref="IDataService{T}"/>
    /// <summary>
    /// MockResidentsService is a class that implements the <see cref="IDataService{T}"/> interface.
    /// <para/>
    /// It handles the saving and retreiving data to and from a list of Residents in memory. It does not store anything in a database.
    /// <para/>
    /// The connectionstring, db name and collections that are used are stored in the IConfiguration dependency under the Database object.
    /// </summary>
    public class MockResidentsService : AMockDataService<Resident>, IResidentsService
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
                },
                Tags = new []{1,2,3,4}
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
                },
                Tags = new []{5,6}
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
                },
                Tags = new []{7,9,8}
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
                },
                Tags = new []{10}
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

        /// <inheritdoc cref="IResidentsService.GetAsync(int,IEnumerable{Expression{Func{Resident,object}}})" />
        /// <summary>
        /// GetAsync returns the <see cref="Resident" /> with the given id from the database. 
        /// <para />
        /// It should only fill the properties passed in the <see cref="!:propertiesToInclude" /> parameter. The id is always passed and 
        /// if the <see cref="!:propertiesToInclude" /> parameter is null (which it is by default), all the properties are included. 
        /// </summary>
        /// <param name="tag">is the id of the item that needs to be fetched</param>
        /// <param name="propertiesToInclude">are the properties that should be included in the objects</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> filled with all the ts in the database.</returns>
        public async Task<Resident> GetAsync(int tag,
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null)
        {
            var propertiesToIncludeList = propertiesToInclude?.ToList();
            if (EnumerableExtensions.IsNullOrEmpty(propertiesToIncludeList))
                return MockData.FirstOrDefault(x => x.Tags != null && x.Tags.Contains(tag));

            foreach (var mockItem in MockData)
            {
                if (mockItem.Tags == null || !mockItem.Tags.Contains(tag))
                    continue;

                // create new newItem to return with the ide filled in
                var itemToReturn = CreateNewItem(mockItem.Id);

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

                    // set the value of the property with the value of the mockItem
                    prop?.SetValue(itemToReturn, prop.GetValue(mockItem));
                }

                // return the newItem
                return itemToReturn;
            }

            // if no item is found, return the default value
            return default(Resident);
        }
    }
#pragma warning restore 1998
}