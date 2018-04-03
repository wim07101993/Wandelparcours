using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.FileIO
{
    public class JsonService : ASerializationService, IJsonService
    {
        private const string ConstFilter = "Json Bestanden (*.json;*.js)|*.json;*.js|Alle bestanden (*.*)|*.*";


        public JsonService(IFileService fileService) : base(fileService)
        {
        }


        protected override string ExtensionFilter => ConstFilter;


        public override string Serialize<T>(IEnumerable<T> values)
            => values.SerializeToJson();

        public override IEnumerable<T> Deserialize<T>(string json)
            => json.DeserializeJson<IEnumerable<T>>();
    }
}