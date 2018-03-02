using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.Controllers
{
    [TestClass]
    public class ResidentsController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var mockData = new List<Resident>
            {
                new Resident
                {
                    Id = new ObjectId("5a9566c58b9ed54db08d0ce7"),
                    FirstName = "Lea",
                    LastName = "Thuwis",
                    Room = "AT109 A",
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
                    Doctor = new Doctor
                    {
                        Name = "Marcel Mellebeek",
                        PhoneNumber = "089 65 74 85"
                    }
                },
            };
            var dataService = new Mock<IDataService>();
            dataService
                .Setup(x => x.GetResidents(WebService.Controllers.ResidentsController.SmallDataProperties))
                .Returns(() => mockData);

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Get().Should().BeOfType<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<IEnumerable<Resident>>()
                .Subject.Count().Should().Be(5);
        }

        [TestMethod]
        public void GetWithDataServiceNotWorking()
        {
            var dataService = new Mock<IDataService>();
            dataService
                .Setup(x => x.GetResidents(WebService.Controllers.ResidentsController.SmallDataProperties))
                .Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Get().Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion get


        #region create

        [TestMethod]
        public void CreateNewResident()
        {
            var id = new ObjectId().ToString();
            var resident = new Resident();

            var dataService = new Mock<IDataService<Resident>>();
            dataService.Setup(x => x.CreateAsync(resident)).Returns(() => Task.FromResult(id));


            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .CreateAsync(resident).Result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [TestMethod]
        public void CreateResidentDoesNotExecute()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService>();
            dataService.Setup(x => x.CreateResident(resident)).Returns(() => null);

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Create(resident).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void CreateResidentThrowsException()
        {
            var resident = new Resident();

            var dataService = new Mock<IDataService>();
            dataService.Setup(x => x.CreateResident(resident)).Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Create(resident).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion create


        #region delete

        [TestMethod]
        public void DeleteExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService>();
            dataService.Setup(x => x.RemoveResident(id)).Returns(() => true);

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Delete(id.ToString()).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteNonExistingResident()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService>();
            dataService.Setup(x => x.RemoveResident(id)).Returns(() => false);

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Delete(id.ToString()).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeleteServiceException()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService>();
            dataService.Setup(x => x.RemoveResident(id)).Returns(() => throw new Exception());

            new WebService.Controllers.ResidentsController(dataService.Object, new ConsoleLogger())
                .Delete(id.ToString()).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        #endregion delete


        #region update

        [TestMethod]
        public void UpdateWithNormalConditions()
        {
            var dataService = new MockDataService();
            var resident = new Resident {Id = dataService.MockData[0].Id, FirstName = "Test", LastName = null};

            var updater = new ResidentUpdater
            {
                Resident = resident,
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .Update(updater).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public void UpdateNonExistingResident()
        {
            var updater = new ResidentUpdater
            {
                Resident = new Resident {Id = new ObjectId(), FirstName = "Test", LastName = null},
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            var dataService = new MockDataService();
            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .Update(updater).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [TestMethod]
        public void UpdateWithoutResident()
        {
            var updater = new ResidentUpdater
            {
                PropertiesToUpdate = new[] {nameof(Resident.FirstName), nameof(Resident.LastName)}
            };

            var dataService = new MockDataService();
            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .Update(updater).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void UpdateWithoutPropertiesToUpdate()
        {
            var dataService = new MockDataService();
            var updater = new ResidentUpdater
            {
                Resident = new Resident {Id = dataService.MockData[0].Id, FirstName = "Test", LastName = null}
            };

            new WebService.Controllers.ResidentsController(dataService, new ConsoleLogger())
                .Update(updater).Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        #endregion update
    }
}