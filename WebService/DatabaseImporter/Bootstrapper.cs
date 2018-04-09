using System.Windows;
using DatabaseImporter.Services;
using DatabaseImporter.Services.Data;
using DatabaseImporter.Services.Data.Implementations;
using DatabaseImporter.Services.Mocks;
using DatabaseImporter.ViewModelInterfaces;
using DatabaseImporter.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Unity;

namespace DatabaseImporter
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow?.Show();
        }

        protected override void ConfigureContainer()
        {
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));

            Container
                .RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager())
                .RegisterType<IStateManager, StateManager>(new ContainerControlledLifetimeManager())
                .RegisterType<IAuthenticationService, AuthenticationService>(new ContainerControlledLifetimeManager())
                .RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager())
                // data services
                .RegisterType<IDataServiceSelector, DataServiceSelector>()
                .RegisterType<ICsvFileService, CsvFileService>(new ContainerControlledLifetimeManager())
                .RegisterType<IJsonFileService, JsonFileService>(new ContainerControlledLifetimeManager())
                .RegisterType<IMongoService, MongoDatabaseService>(new ContainerControlledLifetimeManager())
                // viewmodels
                .RegisterType<ISourceViewModel, SourceViewModel>()
                .RegisterType<IDestinationViewModel, DestinationViewModel>()
                .RegisterType<IDataSelectionViewModel, DataSelectionViewModel>()
                .RegisterType<IMainWindowViewModel, MainWindowViewModel>();

            base.ConfigureContainer();
        }
    }
}