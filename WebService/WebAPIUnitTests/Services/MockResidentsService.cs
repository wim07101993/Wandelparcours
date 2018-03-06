using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockResidentsService
    {
        #region Get

        [TestMethod]
        public void GetResidentsWithAllProperties()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            mockResidentsService.GetAsync().Result
                .Should()
                .BeEquivalentTo(mockResidentsService.MockData, "get should return all the data stored in the db");
        }

        [TestMethod]
        public void GetResidentsWithOnlyID()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            var residents = mockResidentsService
                .GetAsync(new Expression<Func<Resident, object>>[] {x => x.Id})
                .Result
                .ToList();

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id))
                .ToList();

            for (var i = 0; i < residents.Count; i++)
            {
                mockResidentsService.MockData[i]
                    .Id
                    .Should()
                    .Be(residents[i].Id,
                        "it should be the same object and the object id the only field that is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(residents[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(),
                            "only the id property is asked in the selector");
            }
        }

        [TestMethod]
        public void GetResidentsWithSomeFields()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            var residents = mockResidentsService
                .GetAsync(new Expression<Func<Resident, object>>[]
                {
                    x => x.Id,
                    x => x.FirstName,
                    x => x.LastName
                })
                .Result
                .ToList();

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id) &&
                            x.Name != nameof(Resident.FirstName) &&
                            x.Name != nameof(Resident.LastName))
                .ToList();

            for (var i = 0; i < residents.Count; i++)
            {
                mockResidentsService.MockData[i]
                    .Id
                    .Should()
                    .Be(residents[i].Id, "it should be the same object and the object id is never \"not-passed\"");

                mockResidentsService.MockData[i]
                    .FirstName
                    .Should()
                    .Be(residents[i].FirstName, "it is asked in the selector");

                mockResidentsService.MockData[i]
                    .LastName
                    .Should()
                    .Be(residents[i].LastName, "it is asked in the selector");

                foreach (var property in properties)
                    property
                        .GetValue(residents[i])
                        .Should()
                        .Be(property.PropertyType.GetDefault(), "it is not asked in the selector");
            }
        }

        [TestMethod]
        public void GetResidentsWithEmptySelector()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            mockResidentsService
                .GetAsync(new Expression<Func<Resident, object>>[] { }).Result
                .Should()
                .BeEquivalentTo(mockResidentsService.MockData, "get should return all the data stored in the db");
        }

        #endregion Get


        #region Create

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateNullResident()
        {
            try
            {
                var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

                var _ = mockResidentsService.CreateAsync(null).Result;
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }

        [TestMethod]
        public void CreateEmptyResident()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            var resident = new Resident();

            var id = mockResidentsService.CreateAsync(resident).Result;
            id
                .Should()
                .NotBeNullOrEmpty("it is assigned in the create method of the service");

            var newResident = mockResidentsService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            newResident
                .Should()
                .NotBeNull("it is created in the create method of the service");

            // ReSharper disable PossibleNullReferenceException
            newResident
                .Id
                .Should()
                .NotBe(default(ObjectId), "a new object id is created in the service");
            // ReSharper restore PossibleNullReferenceException

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id));

            foreach (var property in properties)
                property
                    .GetValue(newResident)
                    .Should()
                    .Be(property.GetValue(resident), $"it should be equal to the {property.Name} of the resident");
        }

        [TestMethod]
        public void CreateNormalResident()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            var resident = new Resident
            {
                FirstName = "Anna",
                LastName = "Heylen",
                Room = "AT107 A",
                Birthday = new DateTime(1923, 01, 27),
                Doctor = new Doctor
                {
                    Name = "Johan Jespers",
                    PhoneNumber = "089 35 27 89",
                },
            };

            var id = mockResidentsService.CreateAsync(resident).Result;
            id
                .Should()
                .NotBeNullOrEmpty("it is assigned in the create method of the service");

            var newResident = mockResidentsService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            newResident
                .Should()
                .NotBeNull("it is created in the create method of the service");

            // ReSharper disable PossibleNullReferenceException
            newResident
                .Id
                .Should()
                .NotBe(default(ObjectId), "a new object id is created in the service");
            // ReSharper restore PossibleNullReferenceException

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id));

            foreach (var property in properties)
                property
                    .GetValue(newResident)
                    .Should()
                    .Be(property.GetValue(resident), $"it should be equal to the {property.Name} of the resident");
        }

        #endregion Create


        #region RemoveResident

        [TestMethod]
        public void RemoveResidentWithNonExistingID()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            mockResidentsService
                .RemoveAsync(new ObjectId()).Result
                .Should()
                .BeFalse("the item doesn't exist");
        }

        [TestMethod]
        public void RemoveResidentWithExistingID()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            mockResidentsService
                .RemoveAsync(mockResidentsService.MockData[0].Id).Result
                .Should()
                .BeTrue("the item exist");
        }

        #endregion RemoveResident
    }
}