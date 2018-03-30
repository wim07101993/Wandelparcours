using System.Collections.Generic;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using Prism.Mvvm;

namespace ModuleSettingsEditor.WPF.ViewModels
{
    public class ExportWindowViewModel : BindableBase, IExportWindowViewModel
    {
        public IEnumerable<string> Partitions { get; }
        public string SelectedPartition { get; set; }
    }
}