using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseImporter.Models;

namespace DatabaseImporter.Services.FileIO
{
    public abstract class ASerializationService : ISerializationService
    {
        private readonly IFileService _fileService;

        protected ASerializationService(IFileService fileService)
        {
            _fileService = fileService;
        }


        protected abstract string ExtensionFilter { get; }


        public async Task<File<IEnumerable<T>>> ReadObjectFromFileAsync<T>()
        {
            var file = await _fileService.ReadFileAsync(ExtensionFilter);
            return new File<IEnumerable<T>> {Content = Deserialize<T>(file.Content), Path = file.Path};
        }

        public async Task WriteObjectToFileAsync<T>(IEnumerable<T> values)
        {
            var content = Serialize(values);
            await _fileService.WriteFileAsync(content, ExtensionFilter);
        }

        public abstract string Serialize<T>(IEnumerable<T> values);

        public abstract IEnumerable<T> Deserialize<T>(string json);
    }
}