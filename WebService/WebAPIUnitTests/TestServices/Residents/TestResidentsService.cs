using System.Collections.Generic;
using MongoDB.Bson;
using WebAPIUnitTests.TestModels;
using WebService.Helpers.Extensions;
using WebService.Models;
using WebService.Services.Data.Mock;
using WebService.Services.Exceptions;

namespace WebAPIUnitTests.TestServices.Residents
{
    public class TestResidentsService : MockResidentsService, ITestResidentsService
    {
        public TestResidentsService() : base(new Throw())
        {
        }

        public override List<Resident> MockData { get; } = TestData.TestResidents.Clone();

        public override Resident CreateNewItem(ObjectId id)
            => new Resident {Id = id};

        public Resident GetFirst()
            => !EnumerableExtensions.IsNullOrEmpty(MockData) ? MockData[0].Clone() : null;

        public IEnumerable<Resident> GetAll()
            => MockData?.Clone();
    }
}