using System.Windows;
using DatabaseImporter.Services;
using DatabaseImporter.Services.FileIO;
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
                .RegisterType<IFileService, FileService>(new ContainerControlledLifetimeManager())
                // serialization services
                .RegisterType<ICsvService, CsvService>(new ContainerControlledLifetimeManager())
                .RegisterType<IJsonService, JsonService>(new ContainerControlledLifetimeManager())
                .RegisterType<IXmlService, XmlService>(new ContainerControlledLifetimeManager())
                // viewmodels
                .RegisterType<ISourceViewModel, SourceViewModel>()
                .RegisterType<IDestinationViewModel, DestinationViewModel>()
                .RegisterType<IMainWindowViewModel, MainWindowViewModel>();

            base.ConfigureContainer();
        }
    }
}