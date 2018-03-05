using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models;

namespace WebAPIUnitTests.Services
{
    [TestClass]
    public class MockDataService
    {
        public WebService.Services.Data.Mock.MockResidentsService DataService { get; set; } =
            new WebService.Services.Data.Mock.MockResidentsService();


        #region Get

        [TestMethod]
        public void GetResidentsWithAllProperties()
            => Assert.AreEqual(DataService.GetAsync().Result, DataService.MockData);

        [TestMethod]
        public void GetResidentsWithOnlyID()
        {
            var residents = DataService
                .GetAsync(new Expression<Func<Resident, object>>[] {x => x.Id})
                .Result
                .ToList();

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.Id))
                .ToList();

            for (var i = 0; i < residents.Count; i++)
            {
                Assert.AreEqual(residents[i].Id, DataService.MockData[i].Id);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetResidentsWithSomeFields()
        {
            var residents = DataService
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
                Assert.AreEqual(residents[i].Id, DataService.MockData[i].Id);
                Assert.AreEqual(residents[i].FirstName, DataService.MockData[i].FirstName);
                Assert.AreEqual(residents[i].LastName, DataService.MockData[i].LastName);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetResidentsWithEmptySelector()
            => Assert.AreEqual(
                DataService.GetAsync(new Expression<Func<Resident, object>>[] { }).Result,
                DataService.MockData);

        #endregion Get


        #region Create

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateNullResident()
        {
            var _ = DataService.CreateAsync(null).Result;
        }

        [TestMethod]
        public void CreateEmptyResident()
        {
            var id = DataService.CreateAsync(new Resident()).Result;
            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = DataService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
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
            var residtent = new Resident
            {
                FirstName = "Anna",
                LastName = "Heylen",
                Room = "AT107 A",
                Doctor = new Doctor
                {
                    Name = "Johan Jespers",
                    PhoneNumber = "089 35 27 89",
                },
            };

            var id = DataService.CreateAsync(residtent).Result;

            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = DataService.MockData.FirstOrDefault(x => x.Id == new ObjectId(id));
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
            Assert.IsFalse(DataService.RemoveAsync(new ObjectId()).Result);
        }

        #endregion RemoveResident
    }
}