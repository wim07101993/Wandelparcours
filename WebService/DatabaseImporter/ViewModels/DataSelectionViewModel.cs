using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Services;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class DataSelectionViewModel : BindableBase, IDataSelectionViewModel
    {
        public DataSelectionViewModel(IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            SelectedDataType = EDataType.Resident.ToString();

            StateManager.StateChanged += OnStateChanged;
        }


        public IEnumerable<ISelectPropertyViewModel> SelectableProperties
        {
            get
            {
                PropertyInfo[] properties;
                switch (SelectedEDataType)
                {
                    case EDataType.User:
                        properties = typeof(User).GetProperties();
                        break;
                    case EDataType.Resident:
                        properties = typeof(Resident).GetProperties();
                        break;
                    case EDataType.ReceiverModule:
                        properties = typeof(ReceiverModule).GetProperties();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return properties
                    .Where(x => x.CanRead && x.CanWrite)
                    .Select(x => new SelectPropertyViewModel {Property = x});
            }
        }

        public IEnumerable<string> DataTypes { get; } = Enum.GetNames(typeof(EDataType));

        public string SelectedDataType
        {
            get => SelectedEDataType.ToString();
            set
            {
                if (Enum.TryParse(value, out EDataType dataType))
                    StateManager.SetState(EStateManagerKey.DataType.ToString(), dataType);
                else
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private EDataType SelectedEDataType
            => StateManager.GetState<EDataType>(EStateManagerKey.DataType.ToString());

        protected void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            StateManager.StateChanged -= OnStateChanged;

            if (e.State == EStateManagerKey.DataType.ToString())
                RaisePropertyChanged(nameof(SelectedEDataType));

            StateManager.StateChanged += OnStateChanged;
        }
    }
}