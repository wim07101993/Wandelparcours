using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices.Abstract;
using WebService.Helpers.Extensions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestControllers
{
    public class TestRestController : ATestRestController<TestEntity>
    {
        public TestRestController()
            : base(new TestDataService(), new ConsoleLogger())
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

        public IDictionary<string, Expression<Func<TestEntity, object>>> Selectors { get; set; } =
            new Dictionary<string, Expression<Func<TestEntity, object>>>
            {
                {nameof(TestEntity.S), x => x.S},
                {nameof(TestEntity.I), x => x.I},
                {nameof(TestEntity.B), x => x.B},
            };
    }
}