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
using WebAPIUnitTests.Mocks;
using WebService.Helpers.Extensions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.Controllers
{
    [TestClass]
    public class RestServiceController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var dataService = new MockDataService();

            new MockController(dataService, new ConsoleLogger())
                .GetAsync(null).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<IEnumerable<MockEntity>>("a collection of entities is asked").Subject
                .Count()
                .Should()
                .Be(dataService.MockData.Count, "all the items in the db should be returned");
        }

        [TestMethod]
        public void GetByIdNormal()
        {
            var dataService = new MockDataService();
            var id = dataService.MockData[0].Id.ToString();
            var selector = new[] {"s"};

            var entity = new MockController(dataService, new ConsoleLogger())
                .GetAsync(id, selector).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<MockEntity>("a collection of entities is asked").Subject;

            entity
                .S
                .Should()
                .Be(dataService.MockData[0].S, "that is the S value of the entity with the given id");

            var properties = typeof(MockEntity)
                .GetProperties()
                .Where(x => x.Name != nameof(MockEntity.S) && x.Name != nameof(MockEntity.Id));

            foreach (var property in properties)
                property
                    .GetValue(entity)
                    .Should()
                    .BeEquivalentTo(property.PropertyType.GetDefault(), "this property was not asked to return");
        }

        [TestMethod]
        public void GetByIdBadIdFormat()
        {
            var dataService = new MockDataService();
            var selector = new[] {"s"};

            new MockController(dataService, new ConsoleLogger())
                .GetAsync("bad id", selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound, "a bad id cannot be found");
        }

        [TestMethod]
        public void GetByIdNonExistingID()
        {
            var dataService = new MockDataService();
            var selector = new[] {"s"};

            new MockController(dataService, new ConsoleLogger())
                .GetAsync(ObjectId.GenerateNewId().ToString(), selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound, "a non existing id cannot be found");
        }

        [TestMethod]
        public void GetByIdBadProperties()
        {
            var dataService = new MockDataService();
            var id = dataService.MockData[0].Id.ToString();
            var selector = new[] {"a"};

            new MockController(dataService, new ConsoleLogger())
                .GetAsync(id, selector).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.BadRequest, "bad properties cannot be found");
        }

        [TestMethod]
        public void GetByIdNoProperties()
        {
            var dataService = new MockDataService();
            var id = dataService.MockData[0].Id.ToString();
            var entity = new MockController(dataService, new ConsoleLogger())
                .GetAsync(id, null).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<MockEntity>("a collection of entities is asked").Subject;

            var properties = typeof(MockEntity).GetProperties();

            foreach (var property in properties)
                property
                    .GetValue(entity)
                    .Should()
                    .BeEquivalentTo(
                        property.GetValue(dataService.MockData[0]),
                        $"that is the {property.Name} of the entity");
        }

        #endregion get


        #region create

        [TestMethod]
        public void CreateNewTestEntity()
        {
            var entity = new MockEntity();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => Task.FromResult(true));

            new MockController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created,
                    "the creation should be executed and the corresponding response is 201 in http");
        }

        [TestMethod]
        public void CreateTestEntityDoesNotExecute()
        {
            var entity = new MockEntity();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => Task.FromResult(false));

            new MockController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void CreateTestEntityThrowsException()
        {
            var entity = new MockEntity();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => throw new Exception());

            new MockController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity)
                .Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the service throws an exception, a 500 erro should be returned");
        }

        #endregion create


        #region delete

        [TestMethod]
        public void DeleteExistingTestEntity()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(true));

            new MockController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if the item is removed, a 200 code should be returned");
        }

        [TestMethod]
        public void DeleteNonExistingTestEntity()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(false));

            new MockController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound,
                    "if the receiver is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void DeleteNonParsableId()
        {
            new MockController(
                    new Mock<IDataService<MockEntity>>().Object,
                    new ConsoleLogger())
                .DeleteAsync("abc").Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.NotFound,
                    "if the receiver is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void DeleteServiceException()
        {
            var id = new ObjectId();

            var dataService = new Mock<IDataService<MockEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => throw new Exception());

            new MockController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString()).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.InternalServerError,
                    "if the service throws an exception, a 500 error should be returned");
        }

        #endregion delete


        #region update

        [TestMethod]
        public void UpdateWithNormalConditions()
        {
            var dataService = new MockDataService();

            new MockController(dataService, new ConsoleLogger())
                .UpdateAsync(
                    new MockEntity {Id = dataService.MockData[0].Id, S = "Test", B = true},
                    new[] {nameof(MockEntity.S), nameof(MockEntity.B)})
                .Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK, "if an item is updated, a 200 status should be returned");
        }

        [TestMethod]
        public void UpdateNonExistingTestEntity()
        {
            var dataService = new MockDataService();
            new MockController(dataService, new ConsoleLogger())
                .UpdateAsync(
                    new MockEntity {Id = new ObjectId(), S = "Test", B = false},
                    new[] {nameof(MockEntity.S), nameof(MockEntity.B)})
                .Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created, "if an item is not found, a 404 not found error should be returned");
        }

        [TestMethod]
        public void UpdateWithoutPropertiesToUpdate()
        {
            var dataService = new MockDataService();
            new MockController(dataService, new ConsoleLogger())
                .UpdateAsync(new MockEntity {Id = dataService.MockData[0].Id, S = "Test", B = false}, null).Result
                .Should()
                .BeOfType<StatusCodeResult>("all controller methods should return a status code as confirmation")
                .Subject
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.OK,
                    "if the service says it everything is ok, we should return a 200 ok status");
        }

        #endregion update
    }
}