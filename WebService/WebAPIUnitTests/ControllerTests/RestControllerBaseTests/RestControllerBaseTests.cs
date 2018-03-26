using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using WebAPIUnitTests.TestControllers;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices.Abstract;
using WebService.Helpers.Exceptions;

namespace WebAPIUnitTests.ControllerTests.RestControllerBaseTests
{
    [TestClass]
    public class RestControllerBaseTests : IRestControllerTests
    {
        #region CREATE

        [TestMethod]
        public void CreateNull()
        {
            var controller = new TestRestController();

            controller
                .CreateAsync(null)
                .ShouldCatchArgumentException<WebService.Helpers.Exceptions.ArgumentNullException>("item",
                    "the item to create cannot be null");
        }

        [TestMethod]
        public void CreateDuplicate()
        {
            var item = new TestEntity {Id = ObjectId.GenerateNewId()};
            var duplicate = new TestEntity {Id = item.Id};

            var list = new List<TestEntity> {item};

            var mock = new Mock<ITestDataService<TestEntity>>();
            mock.Setup(x => x.CreateAsync(duplicate)).Callback(new Action<TestEntity>(x => list.Add(x)));

            var controller = new TestRestController(mock.Object);

            controller
                .CreateAsync(duplicate)
                .Result
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created, "the item should have been created but the id should have changed");

            list
                .Should()
                .HaveCount(2)
                .And
                .ContainSingle(x => x.Id == item.Id);
        }

        [TestMethod]
        public void Create()
        {
            var item = new TestEntity {Id = ObjectId.GenerateNewId(), S = "some strange string"};

            var list = new List<TestEntity>();

            var mock = new Mock<ITestDataService<TestEntity>>();
            mock.Setup(x => x.CreateAsync(item)).Callback(new Action<TestEntity>(x => list.Add(x)));

            var controller = new TestRestController(mock.Object);

            controller
                .CreateAsync(item)
                .Result
                .StatusCode
                .Should()
                .Be((int) HttpStatusCode.Created, "the item should have been created but the id should have changed");

            list
                .Should()
                .HaveCount(1)
                .And
                .Contain(x => x.S == item.S);
        }

        #endregion CREATE


        #region READ

        [TestMethod]
        public void GetAllNullProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetAllEmptyProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetAllSomeProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOneNullId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOneBadId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOneNullProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOneEmptyProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOneBadProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetOne()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetNullProperty()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetBadProperty()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetPropertyNullID()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetPropertyBadId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void GetProperty()
        {
            throw new System.NotImplementedException();
        }

        #endregion READ


        #region UPDATE

        [TestMethod]
        public void UpdateNullItem()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateBadItem()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateNullProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateEmptyProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdatedBadProperties()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Update()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdatePropertyNullId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdatePropertyBadId()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateNullProperty()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateBadProperty()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdatePropertyBadValue()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void UpdateProperty()
        {
            throw new System.NotImplementedException();
        }

        #endregion UPDATE


        #region DELETE

        [TestMethod]
        public void DeleteNullId()
        {
            // TODO create test
        }

        [TestMethod]
        public void DeleteBadId()
        {
            // TODO create test
        }

        [TestMethod]
        public void Delete()
        {
            // TODO create test
        }

        #endregion DELETE
    }
}