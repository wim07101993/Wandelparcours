using System.ComponentModel;

namespace ModuleSettingsEditor.Models
{
    public interface ISettings : INotifyPropertyChanged
    {
        string RestServerIP { get; set; }
        int RestServerPort { get; set; }

        string SocketIP { get; set; }
        int SocketPort { get; set; }

        string WifiSSID { get; set; }
        string WifiWPA { get; set; }
    }
}
