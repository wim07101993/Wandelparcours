using System.ComponentModel;
using ModuleSettingsEditor.Helpers.Mvvm;

namespace ModuleSettingsEditor.Models
{
    public class Settings : BindableBase
    {
        private string _serverIPAddress;
        private string _socketIPAddress;

        [Browsable(true)]
        [DisplayName("Server IP-adres")]
        public string ServerIPAddress
        {
            get => _serverIPAddress;
            set => SetProperty(ref _serverIPAddress, value);
        }

        [Browsable(true)]
        [DisplayName("Socket IP-adres")]
        public string SocketIPAddress
        {
            get => _socketIPAddress;
            set => SetProperty(ref _socketIPAddress, value);
        }
    }
}