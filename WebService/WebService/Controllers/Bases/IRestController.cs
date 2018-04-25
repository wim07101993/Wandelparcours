using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebService.Models.Bases;

namespace WebService.Controllers.Bases
{
    public interface IRestController<T> : IController
        where T : IModelWithID
    {
        #region CREATE

        Task<string> CreateAsync([FromBody] T item);

        Task<StatusCodeResult> AddItemToListAsync(string id, string property, string jsonValue);

        #endregion CREATE


        #region READ

        Task<IEnumerable<T>> GetAllAsync([FromQuery] string[] propertiesToInclude);

        Task<T> GetOneAsync(string id, [FromQuery] string[] propertiesToInclude);

        Task<object> GetPropertyAsync(string id, string propertyName);

        #endregion READ


        #region UPDATE

        Task UpdateAsync([FromBody] T item, [FromQuery] string[] propertiesToUpdate);

        Task UpdatePropertyAsync(string id, string propertyName, [FromBody] string jsonValue);

        #endregion UPDATE


        #region DELETE

        Task DeleteAsync(string id);

        #endregion DELETE
    }
}