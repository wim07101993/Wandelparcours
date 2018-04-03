using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.FileIO
{
    public class XmlService : ASerializationService, IXmlService
    {
        private const string ConstFilter = "Xml Bestanden (*.xml)|*.xml|Alle bestanden (*.*)|*.*";


        public XmlService(IFileService fileService) : base(fileService)
        {
        }


        protected override string ExtensionFilter => ConstFilter;


        public override string Serialize<T>(IEnumerable<T> values)
            => values.SerializeToXml();

        public override IEnumerable<T> Deserialize<T>(string json)
            => json.DeserializeXml<IEnumerable<T>>();
    }
}