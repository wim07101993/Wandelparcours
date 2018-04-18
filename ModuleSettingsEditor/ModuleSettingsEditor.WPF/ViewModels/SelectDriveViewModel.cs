using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ModuleSettingsEditor.WPF.Helpers.Events;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class SelectDriveViewModel : BindableBase, ISelectDriveViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        private IEnumerable<string> _drives;
        private string _selectedDrive;
        private bool _ok;
        private bool _isNoSelectedDriveVisible;
        private string _noDriveSelectedMessage;


        public SelectDriveViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            const string piDriveVolumeName = "boot";
            Drives = DriveInfo
                .GetDrives()
                .Where(x =>
                {
                    try
                    {
                        return x.VolumeLabel == piDriveVolumeName;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .Select(x => x.Name);

            OkCommand = new DelegateCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedDrive))
                {
                    IsNoSelectedDriveVisible = true;
                }
                else
                {
                    Ok = true;
                    SendCloseDialogEvent();
                }
            });
            CancelCommand = new DelegateCommand(SendCloseDialogEvent);

            NoSelectedDriveOkCommand = new DelegateCommand(() => IsNoSelectedDriveVisible = false);
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

        public bool IsNoSelectedDriveVisible
        {
            get => _isNoSelectedDriveVisible;
            set => SetProperty(ref _isNoSelectedDriveVisible, value);
        }

        public string NoDriveSelectedMessage
        {
            get => _noDriveSelectedMessage;
            set => SetProperty(ref _noDriveSelectedMessage, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand NoSelectedDriveOkCommand { get; }


        private void SendCloseDialogEvent()
        {
            _eventAggregator.GetEvent<CloseDialogEvent>().Publish();
        }
    }
}