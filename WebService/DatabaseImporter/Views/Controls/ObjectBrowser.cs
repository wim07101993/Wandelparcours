using System.Windows;
using System.Windows.Controls;

namespace DatabaseImporter.Views.Controls
{
    public class ObjectBrowser : Control
    {
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(
                nameof(Value),
                typeof(object),
                typeof(ObjectBrowser),
                new PropertyMetadata(default(object)));


        static ObjectBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectBrowser),
                new FrameworkPropertyMetadata(typeof(ObjectBrowser)));
        }


        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}