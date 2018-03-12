using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebService.Controllers.Bases;
using WebService.Helpers.Extensions;
using WebService.Services.Data;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestMocks.Mock
{
    public class TestController : ARestControllerBase<TestEntity>
    {
        public TestController(IDataService<TestEntity> dataService, ILogger logger)
            : base(dataService, logger)
        {
        }

        public override IEnumerable<Expression<Func<TestEntity, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<TestEntity, object>>[]
            {
                x => x.B,
                x => x.I,
                x => x.Id
            };

        public override Expression<Func<TestEntity, object>> ConvertStringToSelector(
            string propertyName)
        {
            if (propertyName.EqualsWithCamelCasing(nameof(TestEntity.S)))
                return x => x.S;
            if (propertyName.EqualsWithCamelCasing(nameof(TestEntity.I)))
                return x => x.I;
            if (propertyName.EqualsWithCamelCasing(nameof(TestEntity.B)))
                return x => x.B;

            throw new ArgumentException(nameof(propertyName),
                $"Property {propertyName} cannot be found on {typeof(TestEntity).Name}");
        }
    }
}