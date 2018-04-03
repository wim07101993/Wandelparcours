using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DatabaseImporter.Views.Converters
{
  public  class ObjectToPropertyListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value?.GetType().GetProperties().Select(x => $"{x.Name}: {x.GetValue(value)}");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
