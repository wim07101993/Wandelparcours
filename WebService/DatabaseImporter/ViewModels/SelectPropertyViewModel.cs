using System.Reflection;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Mvvm;

namespace DatabaseImporter.ViewModels
{
    public class SelectPropertyViewModel : BindableBase, ISelectPropertyViewModel
    {
        private bool _isSelected = true;
        private string _description;
        private PropertyInfo _property;


        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Name
            => Property.GetDisplayName();

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public PropertyInfo Property
        {
            get => _property;
            set
            {
                if(!SetProperty(ref _property, value))
                    return;
                RaisePropertyChanged(nameof(Name));
            }
        }
    }
}