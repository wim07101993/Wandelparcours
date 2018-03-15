using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebAPIUnitTests.TestControllers.bases;
using WebAPIUnitTests.TestModels;
using WebAPIUnitTests.TestServices.Abstract;
using WebService.Services.Exceptions;
using WebService.Services.Logging;

namespace WebAPIUnitTests.TestControllers
{
    public class TestRestController : ATestRestController<TestEntity>
    {
        public TestRestController()
            : base(new Throw(), new TestDataService(), new ConsoleLogger())
        {
        }

        public override IEnumerable<Expression<Func<TestEntity, object>>> PropertiesToSendOnGetAll { get; } =
            new Expression<Func<TestEntity, object>>[]
            {
                x => x.B,
                x => x.I,
                x => x.Id
            };

        public override IDictionary<string, Expression<Func<TestEntity, object>>> PropertySelectors { get; } =
            new Dictionary<string, Expression<Func<TestEntity, object>>>
            {
                {nameof(TestEntity.S), x => x.S},
                {nameof(TestEntity.I), x => x.I},
                {nameof(TestEntity.B), x => x.B},
                {nameof(TestEntity.Id), x => x.Id}
            };

        public IDictionary<string, Expression<Func<TestEntity, object>>> Selectors { get; set; } =
            new Dictionary<string, Expression<Func<TestEntity, object>>>
            {
                {nameof(TestEntity.S), x => x.S},
                {nameof(TestEntity.I), x => x.I},
                {nameof(TestEntity.B), x => x.B},
            };
    }
}