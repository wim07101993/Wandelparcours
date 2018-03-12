using System.Collections.Generic;
using WebService.Models.Bases;
using WebService.Services.Data;

namespace WebAPIUnitTests.TestServices.Abstract
{
    public interface ITestDataService<T> : IDataService<T> where T : IModelWithID
    {
        T GetFirst();
        IEnumerable<T> GetAll();
    }
}
