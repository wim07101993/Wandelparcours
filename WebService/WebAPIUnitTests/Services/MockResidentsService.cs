using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models;
using WebService.Services.Data.Mock;

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

            Assert.AreEqual(mockResidentsService.GetAsync().Result, mockResidentsService.MockData);
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
                Assert.AreEqual(residents[i].Id, mockResidentsService.MockData[i].Id);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
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
                Assert.AreEqual(residents[i].Id, mockResidentsService.MockData[i].Id);
                Assert.AreEqual(residents[i].FirstName, mockResidentsService.MockData[i].FirstName);
                Assert.AreEqual(residents[i].LastName, mockResidentsService.MockData[i].LastName);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetResidentsWithEmptySelector()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            Assert.AreEqual(
                mockResidentsService.GetAsync(new Expression<Func<Resident, object>>[] { }).Result,
                mockResidentsService.MockData);
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

            var id = mockResidentsService.CreateAsync(new Resident()).Result;
            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = mockResidentsService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            Assert.IsNotNull(newResident);
            Assert.IsNull(newResident.FirstName);
            Assert.IsNull(newResident.LastName);
            Assert.AreEqual(newResident.Birthday, default(DateTime));
            Assert.IsNull(newResident.Colors);
            Assert.IsNull(newResident.Images);
            Assert.IsNull(newResident.Doctor);
            Assert.AreEqual(newResident.LastRecordedPosition, default(Point));
            Assert.IsNull(newResident.Locations);
            Assert.IsNull(newResident.Music);
            Assert.IsNull(newResident.Picture);
            Assert.IsNull(newResident.Room);
            Assert.IsNull(newResident.Tags);
            Assert.IsNull(newResident.Videos);
        }

        [TestMethod]
        public void CreateNormalResident()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            var residtent = new Resident
            {
                FirstName = "Anna",
                LastName = "Heylen",
                Room = "AT107 A",
                Birthday = new DateTime(1923,01,27),
                Doctor = new Doctor
                {
                    Name = "Johan Jespers",
                    PhoneNumber = "089 35 27 89",
                },
            };

            var id = mockResidentsService.CreateAsync(residtent).Result;

            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = mockResidentsService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
            Assert.IsNotNull(newResident);
            Assert.AreEqual(newResident, residtent);

            Assert.IsNotNull(newResident.FirstName);
            Assert.IsNotNull(newResident.LastName);
            Assert.AreNotEqual(newResident.Birthday, default(DateTime));
            Assert.IsNull(newResident.Colors);
            Assert.IsNull(newResident.Images);
            Assert.IsNotNull(newResident.Doctor);
            Assert.AreEqual(newResident.LastRecordedPosition, default(Point));
            Assert.IsNull(newResident.Locations);
            Assert.IsNull(newResident.Music);
            Assert.IsNull(newResident.Picture);
            Assert.IsNotNull(newResident.Room);
            Assert.IsNull(newResident.Tags);
            Assert.IsNull(newResident.Videos);
        }

        #endregion Create


        #region RemoveResident

        [TestMethod]
        public void RemoveResidentWithNonExistingID()
        {
            var mockResidentsService = new WebService.Services.Data.Mock.MockResidentsService();

            Assert.IsFalse(mockResidentsService.RemoveAsync(new ObjectId()).Result);
        }

        #endregion RemoveResident
    }
}