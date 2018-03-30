using DatabaseImporter.ViewModelInterfaces;
using Prism.Mvvm;

namespace DatabaseImporter.ViewModels
{
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        public MainWindowViewModel(IInputViewModel inputViewModel, IOutputViewModel outputViewModel)
        {
            InputViewModel = inputViewModel;
            OutputViewModel = outputViewModel;
        }


        public IInputViewModel InputViewModel { get; }
        public IOutputViewModel OutputViewModel { get; }
    }
}
