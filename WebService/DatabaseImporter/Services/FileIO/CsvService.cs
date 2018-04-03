using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.FileIO
{
    public class CsvService : ASerializationService, ICsvService
    {
        private const string ConstFilter = "Json Bestanden (*.json;*.js)|*.json;*.js|Alle bestanden (*.*)|*.*";


        public CsvService(IFileService fileService) : base(fileService)
        {
        }


        protected override string ExtensionFilter => ConstFilter;


        public override string Serialize<T>(IEnumerable<T> values)
            => values.SerializeToCsv();

        public override IEnumerable<T> Deserialize<T>(string json)
            => json.DeserializeCsv<T>();
    }
}