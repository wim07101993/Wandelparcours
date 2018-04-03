namespace DatabaseImporter.Models
{
    public class File<T>
    {
        public string Path { get; set; }
        
        public T Content { get; set; }
    }
}