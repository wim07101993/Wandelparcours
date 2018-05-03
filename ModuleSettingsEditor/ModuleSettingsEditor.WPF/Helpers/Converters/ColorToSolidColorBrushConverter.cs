using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ModuleSettingsEditor.WPF.Helpers.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null
                ? new SolidColorBrush((Color)value)
                : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ((SolidColorBrush)value)?.Color;
    }
}
