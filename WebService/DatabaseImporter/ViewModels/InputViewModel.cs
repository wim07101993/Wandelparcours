using System.Collections.Generic;
using System.Windows.Input;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Commands;

namespace DatabaseImporter.ViewModels
{
    public class InputViewModel : IInputViewModel
    {
        public InputViewModel()
        {
            ChooseFileCommand = new DelegateCommand(async () => await OpenFile());
        }


        public IEnumerable<string> Sources { get; } = new[] {"Csv", "Json", "Mongo REST server"};
        public IEnumerable<string> ModelNames { get; } = new[] {nameof(Resident), nameof(User), nameof(ReceiverModule)};

        public ICommand ChooseFileCommand { get; }


        public void OpenFile()
        {

        }
    }
}