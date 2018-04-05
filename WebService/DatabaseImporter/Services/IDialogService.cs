namespace DatabaseImporter.Services
{
    public interface IDialogService
    {
        string OpenFileDialog(string extensionFilter = null);
        string WriteFileDialog(string extensionFilter = null);
     }
}