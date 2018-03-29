// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using ModuleSettingsEditor.ViewModelInterfaces;
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

        private async void OnOpenButtonClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                FileTypeFilter = {".json", ".js"}
            };

            var file = await picker.PickSingleFileAsync();

            if (file == null)
                return;

            DataContext
                .GetType()
                .GetProperties()
                .Where(x => typeof(ICommand).IsAssignableFrom(x.PropertyType) &&
                            x.Name == nameof(IMainWindowViewModel.OpenCommand))
                .Select(x => x.GetValue(DataContext))
                .Cast<ICommand>()
                .FirstOrDefault()
                ?.Execute(file);
        }
    }
}