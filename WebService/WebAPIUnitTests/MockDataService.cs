using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using WebService.Helpers;
using WebService.Models;

namespace WebAPIUnitTests
{
    [TestClass]
    public class MockDataService
    {
        public WebService.Services.Data.MockDataService DataService { get; set; } =
            new WebService.Services.Data.MockDataService();


        #region GetResidents

        [TestMethod]
        public void GetResidentsWithAllProperties()
            => Assert.AreEqual(DataService.GetResidents(), DataService.MockData);

        [TestMethod]
        public void GetResidentsWithOnlyID()
        {
            var residents = DataService
                .GetResidents(new Expression<Func<Resident, object>>[] {x => x.ID})
                .ToList();

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.ID))
                .ToList();

            for (var i = 0; i < residents.Count; i++)
            {
                Assert.AreEqual(residents[i].ID, DataService.MockData[i].ID);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetResidentsWithSomeFields()
        {
            var residents = DataService
                .GetResidents(new Expression<Func<Resident, object>>[]
                {
                    x => x.ID,
                    x => x.FirstName,
                    x => x.LastName
                })
                .ToList();

            var properties = typeof(Resident)
                .GetProperties()
                .Where(x => x.Name != nameof(Resident.ID) &&
                            x.Name != nameof(Resident.FirstName) &&
                            x.Name != nameof(Resident.LastName))
                .ToList();

            for (var i = 0; i < residents.Count; i++)
            {
                Assert.AreEqual(residents[i].ID, DataService.MockData[i].ID);
                Assert.AreEqual(residents[i].FirstName, DataService.MockData[i].FirstName);
                Assert.AreEqual(residents[i].LastName, DataService.MockData[i].LastName);
                foreach (var property in properties)
                    Assert.AreEqual(property.GetValue(residents[i]), property.PropertyType.GetDefault());
            }
        }

        [TestMethod]
        public void GetResidentsWithEmptySelector()
            => Assert.AreEqual(
                DataService.GetResidents(new Expression<Func<Resident, object>>[] { }),
                DataService.MockData);

        #endregion GetResidents


        #region CreateResident

        [TestMethod]
        public void CreateNullResident()
        {
            try
            {
                DataService.CreateResident(null);
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
                // IGNORED
            }
        }

        [TestMethod]
        public void CreateEmptyResident()
        {
            var id = DataService.CreateResident(new Resident());
            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = DataService.MockData.FirstOrDefault(x => x.ID == new ObjectId(id));
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
                Birthday = new DateTime(1923, 01, 27),
                FirstName = "Anna",
                LastName = "Heylen",
                Room = "AT107 A",
                Doctor = new Doctor
                {
                    Name = "Johan Jespers",
                    PhoneNumber = "089 35 27 89",
                },
            };

            var id = DataService.CreateResident(residtent);

            Assert.IsFalse(string.IsNullOrEmpty(id));

            var newResident = DataService.MockData.FirstOrDefault(x => x.ID == new ObjectId(id));
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

        #endregion CreateResident
    }
}