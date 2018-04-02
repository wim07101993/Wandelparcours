using System.Windows;
using DatabaseImporter.Services;
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
                .RegisterType<IAuthenticationService, AuthenticationService>(new ContainerControlledLifetimeManager())
                .RegisterType<IMainWindowViewModel, MainWindowViewModel>();

            base.ConfigureContainer();
        }
    }
}