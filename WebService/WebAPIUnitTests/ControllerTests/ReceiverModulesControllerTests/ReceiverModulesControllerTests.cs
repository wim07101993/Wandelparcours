using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPIUnitTests.TestHelpers.Extensions;
using WebAPIUnitTests.TestServices.ReceiverModules;
using WebService.Controllers;
using WebService.Helpers.Exceptions;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Exceptions;
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
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .CreateAsync(null)
                .ShouldCatchArgumentException<WebArgumentNullException>("item", "the item to create cannot be null");
        }

        [TestMethod]
        public void CreateDuplicate()
        {
            var dataService = new TestReceiverModulesService();

            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

            var id = dataService.GetFirst().Id;

            var duplicate = new ReceiverModule
            {
                Mac = null,
                Id = id
            };

            controller
                .CreateAsync(duplicate)
                .Wait();

            dataService
                .GetAsync(id).Result
                .Mac
                .Should()
                .NotBe(duplicate.Mac, "this is not update and create should give a new id to the item");
        }

        [TestMethod]
        public void CreateItem()
        {
            var dataService = new TestReceiverModulesService();

            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

            var item = new ReceiverModule {Mac = "fakeMac"};

            controller
                .CreateAsync(item)
                .Wait();

            dataService
                .GetAsync(item.Mac).Result
                .Position
                .TimeStamp
                .Should()
                .NotBe(default(DateTime), "the time stamp should automatically be created");
        }

        #endregion CREATE


        #region READ

        [TestMethod]
        public void GetNullMacNullProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync(null, null)
                .ShouldCatchException<NotFoundException>("there is no item with a null mac");
        }

        [TestMethod]
        public void GetNullMacEmptyProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync(null, new string[0])
                .ShouldCatchException<NotFoundException>("there is no item with a null mac");
        }

        [TestMethod]
        public void GetNullMacBadproperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync(null, new[] {"bad property", nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<NotFoundException>("there is no item with a null mac");
        }

        [TestMethod]
        public void GetNullMacSomeProeprties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync(null, new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<NotFoundException>("there is no item with a null mac");
        }

        [TestMethod]
        public void GetBadMacNullProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync("a", null)
                .ShouldCatchException<NotFoundException>("there is no item with that mac");
        }

        [TestMethod]
        public void GetBadMacEmptyProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync("a", new string[0])
                .ShouldCatchException<NotFoundException>("there is no item with that mac");
        }

        [TestMethod]
        public void GetBadMacBadProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync("a", new[] {"bad property", nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<NotFoundException>("there no property 'bad property' in a receiver module");
        }

        [TestMethod]
        public void GetBadMacSomeProperties()
        {
            var controller =
                new ReceiverModulesController(new Throw(), new TestReceiverModulesService(), new ConsoleLogger());

            controller
                .GetOneAsync("a", new[] {nameof(ReceiverModule.Mac), nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<NotFoundException>("there is no item with that mac");
        }

        [TestMethod]
        public void GetExistingMacNullProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

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
        public void GetExistingMacEmptyProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

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
        public void GetExistingMacBadProperties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

            var mac = dataService.GetFirst().Mac;

            controller
                .GetOneAsync(mac, new[] {"bad property", nameof(ReceiverModule.IsActive)})
                .ShouldCatchException<PropertyNotFoundException>("there no property 'bad property' in a receiver module");
        }

        [TestMethod]
        public void GetExistingMacSomeProerties()
        {
            var dataService = new TestReceiverModulesService();
            var controller = new ReceiverModulesController(new Throw(), dataService, new ConsoleLogger());

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
    }
}