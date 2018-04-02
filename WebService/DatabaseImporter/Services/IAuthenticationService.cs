using System;
using DatabaseImporter.Helpers.Events;

namespace DatabaseImporter.Services
{
    public interface IAuthenticationService
    {
        string Token { get; }

        bool Authenticate(string userName, string password);

        event EventHandler<ValueChangedEventArgs<string>> GotToken;
        event EventHandler<ValueChangedEventArgs<string>> LostToken;
    }
}