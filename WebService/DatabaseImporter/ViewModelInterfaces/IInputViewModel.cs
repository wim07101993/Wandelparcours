using System.Collections.Generic;
using System.Windows.Input;

namespace DatabaseImporter.ViewModelInterfaces
{
    public interface IInputViewModel
    {
        IEnumerable<string> Sources { get; }
        IEnumerable<string> ModelNames { get; }

        ICommand ChooseFileCommand { get;  }
    }
}
