using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using ModuleSettingsEditor.WPF.Models;
using ModuleSettingsEditor.WPF.Services;
using ModuleSettingsEditor.WPF.ViewModelInterfaces;
using ModuleSettingsEditor.WPF.ViewModels;
using ModuleSettingsEditor.WPF.Views;
using Prism.Events;
using Prism.Unity;

namespace ModuleSettingsEditor.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow?.Show();
        }

        protected override void ConfigureContainer()
        {
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));

            Container
                .RegisterType<IFileService<Settings>, FileService<Settings>>(new ContainerControlledLifetimeManager())
                .RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager())
                .RegisterType<ISelectDriveViewModel, SelectDriveViewModel>()
                .RegisterType<IMainWindowViewModel, MainWindowViewModel>();


            base.ConfigureContainer();
        }
    }
}