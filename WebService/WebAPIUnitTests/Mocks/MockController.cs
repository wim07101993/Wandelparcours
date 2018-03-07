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

        public override IEnumerable<Expression<Func<MockEntity, object>>> PropertiesToSendOnGet { get; } = null;

        public override IEnumerable<Expression<Func<MockEntity, object>>> ConvertStringsToSelectors(
            IEnumerable<string> strings)
        {
            var selectors = new List<Expression<Func<MockEntity, object>>>();

            foreach (var propertyName in strings)
            {
                if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.S)))
                    selectors.Add(x => x.S);
                else if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.I)))
                    selectors.Add(x => x.I);
                else if (propertyName.EqualsWithCamelCasing(nameof(MockEntity.B)))
                    selectors.Add(x => x.B);
                else
                    throw new ArgumentException(nameof(strings),
                        $"Property {propertyName} cannot be found on {typeof(MockEntity).Name}");
            }

            return selectors;
        }
    }
}