using DatabaseImporter.ViewModelInterfaces;

namespace DatabaseImporter.Views
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