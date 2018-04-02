using System;
using DatabaseImporter.Helpers.Events;

namespace DatabaseImporter.Services.Mocks
{
    public class AuthenticationService : IAuthenticationService
    {
        private string _token;

        public string Token
        {
            get => _token;
            private set
            {
                if (Equals(_token, value))
                    return;

                var oldTokenIsNull = _token == null;
                var newTokenIsNull = value == null;

                _token = value;
                if (oldTokenIsNull)
                    GotToken?.Invoke(this, new ValueChangedEventArgs<string>(null, _token));
                else if (newTokenIsNull)
                    LostToken?.Invoke(this, new ValueChangedEventArgs<string>(_token, null));
            }
        }


        public bool Authenticate(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return false;

            Token = "token";
            return true;
        }

        public event EventHandler<ValueChangedEventArgs<string>> GotToken;
        public event EventHandler<ValueChangedEventArgs<string>> LostToken;
    }
}