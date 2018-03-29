// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ModuleSettingsEditor.ViewModels;

namespace ModuleSettingsEditor.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}