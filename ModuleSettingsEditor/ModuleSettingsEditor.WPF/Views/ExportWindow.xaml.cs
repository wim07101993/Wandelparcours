using ModuleSettingsEditor.WPF.ViewModelInterfaces;

namespace ModuleSettingsEditor.WPF.Views
{
    public partial class ExportWindow
    {
        public ExportWindow(IExportWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}