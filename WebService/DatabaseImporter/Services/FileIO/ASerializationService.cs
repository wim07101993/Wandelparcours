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


        public async Task<File<IEnumerable<T>>> ReadObjectFromFileWithDialogAsync<T>()
        {
            var file = await _fileService.ReadFileWithDialogAsync(ExtensionFilter);
            return ConvertStringFile<T>(file);
        }

        public async Task<File<IEnumerable<T>>> ReadObjectFromFileAsync<T>(string filePath)
        {
            var file = await _fileService.ReadFileAsync(filePath);
            return ConvertStringFile<T>(file);
        }

        private File<IEnumerable<T>> ConvertStringFile<T>(File<string> file)
        {
            IEnumerable<T> content = null;
            try
            {
                content = Deserialize<T>(file.Content);
            }
            catch
            {
                // IGNORED
            }

            return new File<IEnumerable<T>> { Content = content, Path = file.Path };
        }


        public async Task WriteObjectToFileWithDialogAsync<T>(IEnumerable<T> values)
        {
            var content = Serialize(values);
            await _fileService.WriteFileWithDialogsAsync(content, ExtensionFilter);
        }

        public async Task WriteObjectToFileAsync<T>(string filePath, IEnumerable<T> values)
        {
            var content = Serialize(values);
            await _fileService.WriteFileAsync(filePath, content);
        }


        public abstract string Serialize<T>(IEnumerable<T> values);

        public abstract IEnumerable<T> Deserialize<T>(string json);
    }
}