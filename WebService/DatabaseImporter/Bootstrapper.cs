using System.Windows;
using DatabaseImporter.ViewModelInterfaces;
using DatabaseImporter.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
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
            //    .RegisterType<IFileService<Settings>, FileService<Settings>>(new ContainerControlledLifetimeManager())
            //    .RegisterType<ISelectDriveWindowViewModel, SelectDriveWindowViewModel>()
                .RegisterType<IMainWindowViewModel, MainWindowViewModel>();


            base.ConfigureContainer();
        }
    }
}