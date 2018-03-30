namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IMainWindowViewModel
    {
        IInputViewModel InputViewModel { get; }
        IOutputViewModel OutputViewModel { get; }
    }
}