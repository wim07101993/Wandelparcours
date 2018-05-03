using System;
using System.Globalization;
using System.Windows.Data;

namespace ModuleSettingsEditor.WPF.Helpers.Converters
{
    public class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var substractor = System.Convert.ToDouble(value);
                var d = System.Convert.ToDouble(parameter);
                return substractor - d;
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var substractor = System.Convert.ToDouble(value);
                var d = System.Convert.ToDouble(parameter);
                return substractor + d;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
