using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
        private readonly IMongoCollection<User> _usersCollection;

        private readonly IConfiguration _configuration;
        private readonly IMediaService _mediaService;
        private readonly GridFSBucket _mediaBucket;

        #endregion FIELDS


        #region CONSTRUCTOR

        public DatabaseManager(IConfiguration config, IMediaService mediaService)
        {
            _configuration = config;
            _mediaService = mediaService;
            var database = new MongoClient(_configuration["Database:ConnectionString"])
                .GetDatabase(_configuration["Database:DatabaseName"]);

            _residentsCollection = database.GetCollection<Resident>(_configuration["Database:ResidentsCollectionName"]);
            _usersCollection = database.GetCollection<User>(_configuration["Database:UsersCollectionName"]);

            _mediaBucket = new GridFSBucket(
                database, new GridFSBucketOptions
                {
                    BucketName = config["Database:MediaBucket"],
                    ChunkSizeBytes = MediaService.ChunkSize,
                });
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

        public async Task RemoveUnresolvableRelationsAsync()
        {
            await RemoveMediaWithNonExistingResident();
            await RemoveUnKnownMedia();
            await RemoveNonExistingResidentsFromUsers();
        }

        private async Task RemoveMediaWithNonExistingResident()
        {
            var mediaFields = new Expression<Func<Resident, object>>[]
            {
                x => x.Music,
                x => x.Images,
                x => x.Videos
            };

            var residents = await _residentsCollection
                .Find(FilterDefinition<Resident>.Empty)
                .Select(mediaFields)
                .ToListAsync();

            var residentMediaIds = new List<ObjectId>();
            foreach (var resident in residents)
            {
                residentMediaIds.AddRange(resident.Images.Select(x => x.Id));
                residentMediaIds.AddRange(resident.Videos.Select(x => x.Id));
                residentMediaIds.AddRange(resident.Music.Select(x => x.Id));
            }

            var mediaIdsToRemove = new List<ObjectId>();
            using (var result = await _mediaBucket.FindAsync(FilterDefinition<GridFSFileInfo>.Empty))
            {
                var ids = result
                    .ToEnumerable()
                    .Where(x => !residentMediaIds.Any())
                    .Select(x => x.Id);
                mediaIdsToRemove.AddRange(ids);
            }

            foreach (var id in mediaIdsToRemove)
                await _mediaBucket.DeleteAsync(id);
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

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await _mediaService.GetOneAsync(mediaUrl.Id, memoryStream);
                }
            }
            catch (Exception)
            {
                var mediaFilter = Builders<MediaUrl>.Filter
                    .Eq(x => x.Id, mediaUrl.Id);
                var updater = Builders<Resident>.Update
                    .PullFilter(field, mediaFilter);

                await _residentsCollection
                    .UpdateOneAsync(x => x.Id == residentId, updater);
            }
        }

        private async Task RemoveNonExistingResidentsFromUsers()
        {
            var residentsField = new Expression<Func<User, object>>[] {x => x.Id, x => x.Residents};

            var users = await _usersCollection
                .Find(FilterDefinition<User>.Empty)
                .Select(residentsField)
                .ToListAsync();

            foreach (var user in users)
            {
                if (EnumerableExtensions.IsNullOrEmpty(user.Residents))
                    continue;

                foreach (var residentId in user.Residents)
                {
                    var exists = await _residentsCollection
                        .Find(x => x.Id == residentId)
                        .AnyAsync();

                    if (!exists)
                    {
                        var updater = Builders<User>.Update
                            .Pull(x => x.Residents, residentId);

                        await _usersCollection.UpdateOneAsync(x => x.Id == user.Id, updater);
                    }
                }
            }
        }

        #endregion RemoveUnresolvableRelations


        public async Task RemoveRedundantDataAsync()
        {
        }

        public async Task FillMissingFieldsAsync()
        {
        }


        public async void Cleanup()
        {
            IsWorking = true;

            if (Directory.Exists(VideoConverter.VideoConverter.FilesDirectory))
                Directory.Delete(VideoConverter.VideoConverter.FilesDirectory, true);
            await RemoveUnresolvableRelationsAsync();
            await RemoveRedundantDataAsync();
            await FillMissingFieldsAsync();

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
                    UserName = _configuration["Users:Module:UserName"],
                    Password = _configuration["Users:Module:Password"].Hash(id),
                    UserType = EUserType.Module
                });
        }

        private void CreateDefaultAdmin()
        {
            var userName = _configuration["Users:Administrator:UserName"];
            var password = _configuration["Users:Administrator:Password"];

            var id = ObjectId.GenerateNewId();
            _usersCollection.InsertOne(
                new User
                {
                    Id = id,
                    UserName = userName,
                    Password = password.Hash(id),
                    UserType = EUserType.SysAdmin
                });
        }

        #endregion METHODS
    }
}