using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.Models
{
    public class Settings : BindableBase
    {
        #region FIELDS

        private string _restServerIP;
        private int _restServerPort;
        private string _socketIP;
        private int _socketPort;
        private string _wifiSSID;
        private string _wifiWPA;
        private string _kioskType;

        #endregion FIELDS



        public string RestServerIP
        {
            get => _restServerIP;
            set => SetProperty(ref _restServerIP, value);
        }

        public int RestServerPort
        {
            get => _restServerPort;
            set => SetProperty(ref _restServerPort, value);
        }


        public string SocketIP
        {
            get => _socketIP;
            set => SetProperty(ref _socketIP, value);
        }

        public int SocketPort
        {
            get => _socketPort;
            set => SetProperty(ref _socketPort, value);
        }


        public string WifiSSID
        {
            get => _wifiSSID;
            set => SetProperty(ref _wifiSSID, value);
        }

        public string WifiWPA
        {
            get => _wifiWPA;
            set => SetProperty(ref _wifiWPA, value);
        }


        public string KioskType
        {
            get => _kioskType;
            set => SetProperty(ref _kioskType, value);
        }
    }
}