using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseImporter.Helpers;
using DatabaseImporter.Helpers.Events;
using DatabaseImporter.Helpers.Extensions;
using DatabaseImporter.Models.MongoModels;
using DatabaseImporter.Services;
using DatabaseImporter.ViewModelInterfaces;
using Prism.Events;

namespace DatabaseImporter.ViewModels
{
    public class DataSelectionViewModel : AViewModelBase, IDataSelectionViewModel
    {
        public DataSelectionViewModel(IEventAggregator eventAggregator, IStateManager stateManager)
            : base(eventAggregator, stateManager)
        {
            SelectedDataType = EDataType.Resident.ToString();
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
            set => StateManager.SetState(EState.DataType, Enum.Parse(typeof(EDataType), value));
        }

        private EDataType SelectedEDataType
            => StateManager.GetState<EDataType>(EState.DataType);

        protected override void OnStateChanged(object sender, StateChangedEventArgs e, EState state)
        {
            switch (state)
            {
                case EState.DataType:
                    RaisePropertyChanged(nameof(SelectedEDataType));
                    break;
                case EState.FileContent:
                case EState.Exception:
                case EState.UnKnown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }  
        }
    }
}