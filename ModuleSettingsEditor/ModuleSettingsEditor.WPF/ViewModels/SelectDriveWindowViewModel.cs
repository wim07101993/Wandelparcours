using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class SelectDriveWindowViewModel : BindableBase, ISelectDriveWindowViewModel
    {
        private IEnumerable<string> _drives;
        private string _selectedDrive;
        private bool _ok;


        public SelectDriveWindowViewModel()
        {
            Drives = DriveInfo.GetDrives().Select(x => x.Name);

            OkCommand = new DelegateCommand(() => Ok = true);
            CancelCommand = new DelegateCommand(() => { });
        }


        public IEnumerable<string> Drives
        {
            get => _drives;
            private set => SetProperty(ref _drives, value);
        }

        public string SelectedDrive
        {
            get => _selectedDrive;
            set => SetProperty(ref _selectedDrive, value);
        }

        public bool Ok
        {
            get => _ok;
            set => SetProperty(ref _ok, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
    }
}