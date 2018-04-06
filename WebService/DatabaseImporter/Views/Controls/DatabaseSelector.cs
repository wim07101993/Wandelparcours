using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DatabaseImporter.Services.Data;

namespace DatabaseImporter.Views.Controls
{
    public class DatabaseSelector : Control
    {
        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty IpAddressProperty
            = DependencyProperty.Register(
                nameof(IpAddress),
                typeof(string),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(string), OnIpAddressChanged));


        public static readonly DependencyProperty DatabasesProperty
            = DependencyProperty.Register(
                nameof(Databases),
                typeof(IEnumerable<string>),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(IEnumerable<string>), OnDatabasesChanged));

        public static readonly DependencyProperty TablesProperty
            = DependencyProperty.Register(
                nameof(Tables),
                typeof(IEnumerable<string>),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(IEnumerable<string>), OnTablesChanged));


        public static readonly DependencyProperty DatabaseProperty
            = DependencyProperty.Register(
                nameof(Database),
                typeof(string),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(string), OnDatabaseChanged));

        public static readonly DependencyProperty TableProperty
            = DependencyProperty.Register(
                nameof(Table),
                typeof(string),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(string)));


        public static readonly DependencyProperty ServiceProperty
            = DependencyProperty.Register(
                nameof(Service),
                typeof(IDatabaseService),
                typeof(DatabaseSelector),
                new PropertyMetadata(default(IDatabaseService)));

        #endregion DEPENDENCYPROPERTIES


        #region CONSTRUCTOR

        static DatabaseSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatabaseSelector),
                new FrameworkPropertyMetadata(typeof(DatabaseSelector)));
        }

        #endregion CONSTRUCTOR


        #region PROPERTIES

        public string IpAddress
        {
            get => (string) GetValue(IpAddressProperty);
            set => SetValue(IpAddressProperty, value);
        }


        public IEnumerable<string> Databases
        {
            get => (IEnumerable<string>) GetValue(DatabasesProperty);
            set => SetValue(DatabasesProperty, value);
        }

        public IEnumerable<string> Tables
        {
            get => (IEnumerable<string>) GetValue(TablesProperty);
            set => SetValue(TablesProperty, value);
        }


        public string Database
        {
            get => (string) GetValue(DatabaseProperty);
            set => SetValue(DatabaseProperty, value);
        }

        public string Table
        {
            get => (string) GetValue(TableProperty);
            set => SetValue(TableProperty, value);
        }


        public IDatabaseService Service
        {
            get => (IDatabaseService) GetValue(ServiceProperty);
            set => SetValue(ServiceProperty, value);
        }

        #endregion PROPERTIES


        #region METHDOS

        private static async void OnIpAddressChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o.GetValue(ServiceProperty) is IDatabaseService service))
                return;

            var ip = e.NewValue as string;
            if (string.IsNullOrEmpty(ip))
                o.SetValue(DatabasesProperty, DatabaseProperty.DefaultMetadata.DefaultValue);
            else
                o.SetValue(DatabasesProperty, await service.GetDatabasesAsync(ip));
        }

        private static void OnDatabasesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            o.SetValue(DatabaseProperty, DatabaseProperty.DefaultMetadata.DefaultValue);
        }

        private static void OnTablesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            o.SetValue(TableProperty, TableProperty.DefaultMetadata.DefaultValue);
        }

        private static async void OnDatabaseChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o.GetValue(ServiceProperty) is IDatabaseService service))
                return;

            var ip = o.GetValue(IpAddressProperty) as string;
            var db = e.NewValue as string;

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(db))
                o.SetValue(TablesProperty, TablesProperty.DefaultMetadata.DefaultValue);
            else
                o.SetValue(TablesProperty, await service.GetTables(ip, db));
        }

        #endregion METHODS
    }
}