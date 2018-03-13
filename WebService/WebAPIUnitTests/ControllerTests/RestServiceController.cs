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
using WebAPIUnitTests.TestMocks.Mock;
using WebAPIUnitTests.TestServices.Abstract;
using WebService.Helpers.Extensions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests
{
    [TestClass]
    public class RestServiceController
    {
        #region get

        [TestMethod]
        public void GetWithDataServiceWorking()
        {
            var dataService = new TestDataService();

            new TestController(dataService, new ConsoleLogger())
                .GetAsync(null).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<IEnumerable<TestEntity>>("a collection of entities is asked").Subject
                .Count()
                .Should()
                .Be(dataService.MockData.Count, "all the items in the db should be returned");
        }

        [TestMethod]
        public void GetByIdNormal()
        {
            var dataService = new TestDataService();
            var id = dataService.MockData[0].Id.ToString();
            var selector = new[] {"s"};

            var entity = new TestController(dataService, new ConsoleLogger())
                .GetAsync(id, selector).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<TestEntity>("a collection of entities is asked").Subject;

            entity
                .S
                .Should()
                .Be(dataService.MockData[0].S, "that is the S value of the entity with the given id");

            var properties = typeof(TestEntity)
                .GetProperties()
                .Where(x => x.Name != nameof(TestEntity.S) && x.Name != nameof(TestEntity.Id));

            foreach (var property in properties)
                property
                    .GetValue(entity)
                    .Should()
                    .BeEquivalentTo(property.PropertyType.GetDefault(), "this property was not asked to return");
        }

        [TestMethod]
        public void GetByIdBadIdFormat()
        {
            var dataService = new TestDataService();
            var selector = new[] {"s"};

            new TestController(dataService, new ConsoleLogger())
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
            var dataService = new TestDataService();
            var selector = new[] {"s"};

            new TestController(dataService, new ConsoleLogger())
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
            var dataService = new TestDataService();
            var id = dataService.MockData[0].Id.ToString();
            var selector = new[] {"a"};

            new TestController(dataService, new ConsoleLogger())
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
            var dataService = new TestDataService();
            var id = dataService.MockData[0].Id.ToString();
            var entity = new TestController(dataService, new ConsoleLogger())
                .GetAsync(id, null).Result
                .Should()
                .BeOfType<OkObjectResult>("the controller should return a 200 ok to the client").Subject
                .Value
                .Should()
                .BeAssignableTo<TestEntity>("a collection of entities is asked").Subject;

            var properties = typeof(TestEntity).GetProperties();

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
            var entity = new TestEntity();

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => Task.FromResult(true));

            new TestController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity)
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
            var entity = new TestEntity();

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => Task.FromResult(false));

            new TestController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity)
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
            var entity = new TestEntity();

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService
                .Setup(x => x.CreateAsync(entity))
                .Returns(() => throw new Exception());

            new TestController(dataService.Object, new ConsoleLogger())
                .CreateAsync(entity)
                
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

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(true));

            new TestController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString())
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

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => Task.FromResult(false));

            new TestController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString())
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
            new TestController(
                    new Mock<IDataService<TestEntity>>().Object,
                    new ConsoleLogger())
                .DeleteAsync("abc")
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

            var dataService = new Mock<IDataService<TestEntity>>();
            dataService.Setup(x => x.RemoveAsync(id)).Returns(() => throw new Exception());

            new TestController(dataService.Object, new ConsoleLogger())
                .DeleteAsync(id.ToString())
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
            var dataService = new TestDataService();

            new TestController(dataService, new ConsoleLogger())
                .UpdateAsync(
                    new TestEntity {Id = dataService.MockData[0].Id, S = "Test", B = true},
                    new[] {nameof(TestEntity.S), nameof(TestEntity.B)})
                
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
            var dataService = new TestDataService();
            new TestController(dataService, new ConsoleLogger())
                .UpdateAsync(
                    new TestEntity {Id = new ObjectId(), S = "Test", B = false},
                    new[] {nameof(TestEntity.S), nameof(TestEntity.B)})
                
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
            var dataService = new TestDataService();
            new TestController(dataService, new ConsoleLogger())
                .UpdateAsync(new TestEntity {Id = dataService.MockData[0].Id, S = "Test", B = false}, null)
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