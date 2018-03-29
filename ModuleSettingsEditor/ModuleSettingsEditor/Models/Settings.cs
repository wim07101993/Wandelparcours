using ModuleSettingsEditor.Helpers.Mvvm;

namespace ModuleSettingsEditor.Models
{
    public class Settings : BindableBase
    {
        private string _serverIPAddress;
        private string _socketIPAddress;

        public string ServerIPAddress
        {
            get => _serverIPAddress;
            set => SetProperty(ref _serverIPAddress, value);
        }

        public string SocketIPAddress
        {
            get => _socketIPAddress;
            set => SetProperty(ref _socketIPAddress, value);
        }
    }
}