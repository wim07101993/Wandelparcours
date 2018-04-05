using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseImporter.Models.MongoModels;

namespace DatabaseImporter.Services.DataIO
{
    public interface IDatabaseService
    {
        #region PROPERTIES

        string ConnectionString { get; set; }

        string DatabaseName { get; set; }

        string ResidentsTableName { get; set; }
        string UsersTableName { get; set; }
        string ReceiverModuleTableName { get; set; }
        string MediaTableName { get; set; }

        #endregion PROPERTIES


        #region METHODS

        #region get

        Task<IEnumerable<Resident>> GetResidents(
            IEnumerable<Expression<Func<Resident, object>>> propertiesToInclude = null);

        Task<IEnumerable<User>> GetUsers(
            IEnumerable<Expression<Func<User, object>>> propertiesToInclude = null);

        Task<IEnumerable<ReceiverModule>> GetReceiverModules(
            IEnumerable<Expression<Func<ReceiverModule, object>>> propertiesToInclude = null);

        Task<IEnumerable<MediaData>> GetMedia(
            IEnumerable<Expression<Func<MediaData, object>>> propertiesToInclude = null);

        #endregion get

        #region add

        Task Add(IEnumerable<Resident> residents);
        Task Add(IEnumerable<User> users);
        Task Add(IEnumerable<ReceiverModule> receiverModules);
        Task Add(IEnumerable<MediaData> media);

        #endregion add

        #endregion METHODS
    }
}