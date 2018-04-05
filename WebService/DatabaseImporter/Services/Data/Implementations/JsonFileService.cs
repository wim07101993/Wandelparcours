using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.Data.Implementations
{
    public class JsonFileService : AFileService, IJsonFileService
    {
        public override string ExtensionFilter { get; } = "json bestanden (*.json;*.js)|*.json;*.js|alle bestanden (*.*)|*.*";


        public override string Serialize<T>(IEnumerable<T> values)
            => values.SerializeToJson();

        public override IEnumerable<T> Deserialize<T>(string str)
            => str.DeserializeJson<IEnumerable<T>>();
    }
}