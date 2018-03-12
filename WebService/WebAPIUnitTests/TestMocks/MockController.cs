using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebService.Controllers.Bases;
using WebService.Helpers.Extensions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestMocks
{
    public class MockController : ARestControllerBase<MockEntity>
    {
        public MockController(IDataService<MockEntity> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

<<<<<<< HEAD:WebService/WebAPIUnitTests/Mocks/MockController.cs
        public override IEnumerable<Expression<Func<MockEntity, object>>> PropertiesToSendOnGetAll { get; }
=======
        public override IEnumerable<Expression<Func<MockEntity, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<MockEntity, object>>[]
            {
                x => x.B,
                x => x.I,
                x => x.Id
            };
>>>>>>> REST_Service:WebService/WebAPIUnitTests/TestMocks/MockController.cs

        public override Expression<Func<MockEntity, object>> ConvertStringToSelector(
            string propertyName)
        {
            if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.S)))
                return x => x.S;
            if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.I)))
                return x => x.I;
            if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.B)))
                return x => x.B;

            throw new ArgumentException(nameof(propertyName),
                $"Property {propertyName} cannot be found on {typeof(MockEntity).Name}");
        }
    }
}