using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebService.Controllers.Bases;
using WebService.Helpers.Extensions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.Mocks
{
    public class MockController : ARestControllerBase<MockEntity>
    {
        public MockController(IDataService<MockEntity> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        public override IEnumerable<Expression<Func<MockEntity, object>>> PropertiesToSendOnGetAll { get; } = null;

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