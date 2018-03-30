using System.Windows;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;

namespace ModuleSettingsEditor.WPF.Views
{
    public partial class SelectDriveWindow
    {
        public SelectDriveWindow(ISelectDriveWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void onCloseButtonClick(object sender, RoutedEventArgs e) 
            => Close();
    }
}