using System.Collections.Generic;
using System.Windows.Input;
using DatabaseImporter.ViewModelInterfaces;

namespace DatabaseImporter.ViewModels
{
    public class InputViewModel: IInputViewModel
    {
        public IEnumerable<string> Sources { get; }
        public IEnumerable<string> ModelNames { get; }

        public ICommand ChooseFileCommand { get; }
    }
}
