using System.ComponentModel;
using System.Windows;
using DatabaseImporter.Services;
using Microsoft.Practices.Unity;

namespace DatabaseImporter.Views.Controls
{
    public partial class Login
    {
        public static readonly DependencyProperty IsLoggedInProperty =
            DependencyProperty.Register(
                nameof(IsLoggedIn),
                typeof(bool),
                typeof(Login),
                new PropertyMetadata(default(bool)));


        private readonly IAuthenticationService _authenticationService;


        public Login()
        {
            InitializeComponent();

#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
#endif

            _authenticationService = App.Bootstrapper.Container.Resolve<IAuthenticationService>();
        }


        public bool IsLoggedIn
        {
            get => (bool) GetValue(IsLoggedInProperty);
            set => SetValue(IsLoggedInProperty, value);
        }


        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (_authenticationService.Authenticate(UserNameTextBox.Text, PasswordBox.Password))
                SetValue(IsLoggedInProperty, true);
            else
            {
                SetValue(IsLoggedInProperty, false);
                DialogHost.IsOpen = true;
            }
        }
    }
}