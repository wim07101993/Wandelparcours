using System;
using DatabaseImporter.Helpers;
using Microsoft.Practices.Unity;

namespace DatabaseImporter.Services.Data.Implementations
{
    public class DataServiceSelector : IDataServiceSelector
    {
        public IDataService GetService(ESource source)
        {
            switch (source)
            {
                case ESource.Json:
                    return App.Bootstrapper.Container.Resolve<JsonFileService>();
                case ESource.Csv:
                    return App.Bootstrapper.Container.Resolve<CsvFileService>();
                case ESource.MongoDB:
                    return App.Bootstrapper.Container.Resolve<MongoDatabaseService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public IDataService GetService(EDestination destination)
        {
            switch (destination)
            {
                case EDestination.Json:
                    return App.Bootstrapper.Container.Resolve<JsonFileService>();
                case EDestination.Csv:
                    return App.Bootstrapper.Container.Resolve<CsvFileService>();
                case EDestination.MongoDB:
                    return App.Bootstrapper.Container.Resolve<MongoDatabaseService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(destination), destination, null);
            }
        }
    }
}