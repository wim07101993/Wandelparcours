using System.Windows;

namespace ModuleSettingsEditor.WPF
{
    public partial class App
    {
        public static Bootstrapper Bootstrapper { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Bootstrapper = new Bootstrapper();
            Bootstrapper.Run();
        }
    }
}