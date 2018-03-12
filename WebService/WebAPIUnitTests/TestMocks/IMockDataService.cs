using System.Collections.Generic;
using WebAPIUnitTests.TestMocks.Mock;
using WebService.Services.Data;

namespace WebAPIUnitTests.TestMocks
{
    public interface IMockDataService : IDataService<MockEntity>
    {
        MockEntity GetFirst();
        IEnumerable<MockEntity> GetAll();
    }
}