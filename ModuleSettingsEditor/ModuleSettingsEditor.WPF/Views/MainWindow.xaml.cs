using ModuleSettingsEditor.WPF.ViewModelInterfaces;

namespace ModuleSettingsEditor.WPF.Views
{
    public partial class MainWindow
    {
        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}