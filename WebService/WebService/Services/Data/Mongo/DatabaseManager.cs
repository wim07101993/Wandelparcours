using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WebService.Helpers.Extensions;
using WebService.Models;

namespace WebService.Services.Data.Mongo
{
    public class DatabaseManager : IDatabaseManager
    {
        #region FIELDS

        private volatile bool _isWorking;
        private volatile bool _isSchedulerRunning;

        private readonly IMongoCollection<Resident> _residentsCollection;
        private readonly IMongoCollection<ReceiverModule> _receiverModulesCollection;
        private readonly IMongoCollection<MediaData> _mediaCollection;
        private readonly IMongoCollection<User> _usersCollection;

        #endregion FIELDS


        #region CONSTRUCTOR

        public DatabaseManager(IConfiguration config)
        {
            var database = new MongoClient(config["Database:ConnectionString"])
                .GetDatabase(config["Database:DatabaseName"]);

            _residentsCollection = database.GetCollection<Resident>(config["Database:ResidentsCollectionName"]);
            _receiverModulesCollection =
                database.GetCollection<ReceiverModule>(config["Database:ReceiverModulesCollectionName"]);
            _mediaCollection = database.GetCollection<MediaData>(config["Database:MediaCollectionName"]);
            _usersCollection = database.GetCollection<User>(config["Database:UsersCollectionName"]);
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public bool IsWorking
        {
            get => _isWorking;
            private set => _isWorking = value;
        }

        public bool IsSchedulerRunning
        {
            get => _isSchedulerRunning;
            private set => _isSchedulerRunning = value;
        }

        public TimeSpan SchedulingInterval { get; set; }

        #endregion PROPERTIES


        #region METHODS

        #region RemoveUnresolvableRelations

        public async Task RemoveUnresolvableRelations()
        {
            await RemoveMediaWithNonExistingResident();
            await RemoveUnKnownMedia();
        }

        private async Task RemoveMediaWithNonExistingResident()
        {
            var residentIds = _residentsCollection
                .Find(FilterDefinition<Resident>.Empty)
                .Project(x => x.Id)
                .ToList();

            
            
            var ownerExistsFilter = Builders<MediaData>.Filter.Nin(x => x.OwnerId, residentIds);
            await _mediaCollection.DeleteManyAsync(ownerExistsFilter);
        }

        private async Task RemoveUnKnownMedia()
        {
            var fields = new Expression<Func<Resident, object>>[]
            {
                x => x.Music,
                x => x.Images,
                x => x.Videos
            };

            var residents = await _residentsCollection
                .Find(FilterDefinition<Resident>.Empty)
                .Select(fields)
                .ToListAsync();

            var musicField = new ExpressionFieldDefinition<Resident>(fields[0]);
            var imageField = new ExpressionFieldDefinition<Resident>(fields[1]);
            var videoField = new ExpressionFieldDefinition<Resident>(fields[2]);

            foreach (var resident in residents)
            {
                foreach (var mediaUrl in resident.Music)
                    await RemoveMediaUrlIfThereIsNodata(mediaUrl, musicField, resident.Id);

                foreach (var mediaUrl in resident.Images)
                    await RemoveMediaUrlIfThereIsNodata(mediaUrl, imageField, resident.Id);

                foreach (var mediaUrl in resident.Videos)
                    await RemoveMediaUrlIfThereIsNodata(mediaUrl, videoField, resident.Id);
            }
        }

        private async Task RemoveMediaUrlIfThereIsNodata(MediaUrl mediaUrl, FieldDefinition<Resident> field,
            ObjectId residentId)
        {
            if (mediaUrl.Url != null)
                return;

            var exists = await _mediaCollection
                .Find(x => x.Id == mediaUrl.Id)
                .AnyAsync();

            if (!exists)
            {
                var mediaFilter = Builders<MediaUrl>
                    .Filter
                    .Eq(x => x.Id, mediaUrl.Id);
                var updater = Builders<Resident>
                    .Update
                    .PullFilter(field, mediaFilter);

                await _residentsCollection
                    .FindOneAndUpdateAsync(x => x.Id == residentId, updater);
            }
        }

        #endregion RemoveUnresolvableRelations


        public async Task RemoveRedundantData()
        {
        }

        public async Task FillMissingFields()
        {
        }


        public async void Cleanup()
        {
            IsWorking = true;

            await RemoveUnresolvableRelations();
            await RemoveRedundantData();
            await FillMissingFields();

            IsWorking = false;
        }

        public async Task ScheduleCleanup()
        {
            IsSchedulerRunning = true;
            while (IsSchedulerRunning)
            {
                Cleanup();
                await Task.Delay(SchedulingInterval);
            }
        }

        public async Task ScheduleCleanup(TimeSpan interval)
        {
            SchedulingInterval = interval;
            await ScheduleCleanup();
        }

        public void CancelCleanupSchedule()
            => IsSchedulerRunning = false;


        public void ConfigureDB()
        {
            if (!_usersCollection.Find(x => x.UserType == EUserType.Module).Any())
                CreateDefaultModuleUser();
            if (!_usersCollection.Find(x => x.UserType == EUserType.SysAdmin).Any())
                CreateDefaultAdmin();
        }

        private void CreateDefaultModuleUser()
        {
            var id = ObjectId.GenerateNewId();
            _usersCollection.InsertOne(
                new User
                {
                    Id = id,
                    UserName = "Modul3",
                    Password = "KioskTo3rmali3n".Hash(id),
                    UserType = EUserType.Module
                });
        }

        private void CreateDefaultAdmin()
        {
            var id = ObjectId.GenerateNewId();
            _usersCollection.InsertOne(
                new User
                {
                    Id = id,
                    UserName = "Administrator",
                    Password = "AdminToermalien".Hash(id),
                    UserType = EUserType.SysAdmin
                });
        }

        #endregion METHODS
    }
}