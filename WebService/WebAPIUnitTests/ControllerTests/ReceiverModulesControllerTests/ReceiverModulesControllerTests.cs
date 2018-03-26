using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Moq;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.ReceiverModules;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.ControllerTests.ReceiverModulesControllerTests
{
    [TestClass]
    public class ReceiverModulesControllerTests : IReceiverModulesControllerTests
    {
        #region CREATE

        [TestMethod]
        public void CreateNullItem()
        {
            var controller =
                new ReceiverModulesController(new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .CreateAsync(null)
                .ShouldCatchArgumentException<WebService.Helpers.Exceptions.ArgumentNullException>("item",
                    "the item to create cannot be null");
        }

        [TestMethod]
        public void CreateDuplicate()
        {
            var item = new ReceiverModule {Id = ObjectId.GenerateNewId()};
            var duplicate = new ReceiverModule {Id = item.Id};

            var list = new List<ReceiverModule> {item};

            var mock = new Mock<IReceiverModulesService>();
            mock.Setup(x => x.CreateAsync(duplicate)).Callback(new Action<ReceiverModule>(x => list.Add(x)));

            var controller = new ReceiverModulesController(mock.Object, new ConsoleLogger());

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

            list.First(x => x.Id == item.Id)
                .Position
                ?.TimeStamp
                .Should()
                .NotBe(default(DateTime));
        }

        [TestMethod]
        public void CreateItem()
        {
            var item = new ReceiverModule {Id = ObjectId.GenerateNewId(), Mac = "some mac"};

            var list = new List<ReceiverModule>();

            var mock = new Mock<IReceiverModulesService>();
            mock.Setup(x => x.CreateAsync(item)).Callback(new Action<ReceiverModule>(x => list.Add(x)));

            var controller = new ReceiverModulesController(mock.Object, new ConsoleLogger());

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
                .Contain(x => x.Mac == item.Mac);
        }

        #endregion CREATE


        #region READ

        [TestMethod]
        public void GetNullMac()
        {
            var controller =
                new ReceiverModulesController(
                    new Mock<IReceiverModulesService>().Object,
                    new ConsoleLogger());

            controller
                .GetOneAsync(null, null)
                .ShouldCatchException<NotFoundException>("there is no item with a null mac");
        }

        [TestMethod]
        public void GetBadMac()
        {
            var controller =
                new ReceiverModulesController(
                    new Mock<IReceiverModulesService>().Object,
                    new ConsoleLogger());

            controller
                .GetOneAsync("bad mac", null)
                .ShouldCatchException<NotFoundException>("there is no item with that mac");
        }

        [TestMethod]
        public void GetNullProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(dataService, new ConsoleLogger());

            var mac = dataService.GetFirst().Mac;

            var item = controller.GetOneAsync(mac, null).Result;
            var original = dataService.GetAsync(mac).Result;

            foreach (var property in typeof(ReceiverModule).GetProperties())
                property
                    .GetValue(item)
                    .Should()
                    .BeEquivalentTo(property.GetValue(original));
        }

        [TestMethod]
        public void GetEmptyProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(dataService, new ConsoleLogger());

            var mac = dataService.GetFirst().Mac;

            var item = controller.GetOneAsync(mac, new string[0]).Result;
            var original = dataService.GetAsync(mac).Result;

            item.Mac
                .Should()
                .Be(original.Mac);

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Mac));

            foreach (var property in properties)
                property
                    .GetValue(item)
                    .Should()
                    .BeEquivalentTo(property.PropertyType.GetDefault());
        }

        [TestMethod]
        public void GetBadProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(dataService, new ConsoleLogger());

            var mac = dataService.GetFirst().Mac;

            controller
                .GetOneAsync(mac, new[] {"bad property", nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<PropertyNotFoundException<ReceiverModule>>(
                    "there no property 'bad property' in a receiver module");
        }

        [TestMethod]
        public void Get()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(dataService, new ConsoleLogger());

            var mac = dataService.GetFirst().Mac;

            var item = controller.GetOneAsync(mac, new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)})
                .Result;
            var original = dataService.GetAsync(mac).Result;

            item.Mac
                .Should()
                .Be(original.Mac);

            item.IsActive
                .Should()
                .Be(original.IsActive);

            var properties = typeof(ReceiverModule)
                .GetProperties()
                .Where(x => x.Name != nameof(ReceiverModule.Mac) && x.Name != nameof(ReceiverModule.IsActive));

            foreach (var property in properties)
                property
                    .GetValue(item)
                    .Should()
                    .BeEquivalentTo(property.PropertyType.GetDefault());
        }

        #endregion READ


        #region DELETE

        public void DeleteNullMac()
        {
        }

        public void DeleteBadMac()
        {
            throw new NotImplementedException();
        }

        #endregion DELETE
    }
}