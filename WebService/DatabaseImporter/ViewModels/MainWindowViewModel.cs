using DatabaseImporter.ViewModelInterfaces;
using Prism.Mvvm;

namespace DatabaseImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        public MainWindowViewModel(ISourceViewModel sourceViewModel)
        {
            SourceViewModel = sourceViewModel;
        }


        public ISourceViewModel SourceViewModel { get; }
    }
}
