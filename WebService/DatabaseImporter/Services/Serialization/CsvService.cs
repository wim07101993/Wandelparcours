using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Services.DataIO;

namespace DatabaseImporter.Services.Serialization
{
    public class CsvService : ASerializationService, ICsvService
    {
        private const string ConstFilter = "Csv Bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*";


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