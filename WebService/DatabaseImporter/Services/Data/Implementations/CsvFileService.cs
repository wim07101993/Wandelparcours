using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.Data.Implementations
{
    public class CsvFileService : AFileService, ICsvFileService
    {
        public override string ExtensionFilter { get; } = "csv bestanden (*.csv)|*.csv|alle bestanden (*.*)|*.*";


        public override string Serialize<T>(IEnumerable<T> values)
            => values.SerializeToCsv();

        public override IEnumerable<T> Deserialize<T>(string str)
            => str.DeserializeCsv<T>();
    }
}