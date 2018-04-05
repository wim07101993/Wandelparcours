using System.Collections.Generic;
using DatabaseImporter.Helpers.Extensions;

namespace DatabaseImporter.Services.Data.Implementations
{
   public class XmlFileService : AFileService, IXmlFileService
   {
       public override string ExtensionFilter { get; } = "xml bestanden (*.xml)|*.xml|alle bestanden (*.*)|*.*";


       public override string Serialize<T>(IEnumerable<T> values)
           => values.SerializeToXml();

       public override IEnumerable<T> Deserialize<T>(string str)
           => str.DeserializeXml<IEnumerable<T>>();
   }
}
