using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModuleSettingsEditor.WPF.Helpers;

namespace ModuleSettingsEditor.WPF.Views.ColorPicker
{
    [TemplatePart(Name = ElementSpectrumDisplay, Type = typeof(Rectangle))]
    public class ColorSpectrumSlider : Slider
    {
        #region FIELDS

        private const string ElementSpectrumDisplay = "PART_SpectrumDisplay";

        private Border _spectrumDisplay;
        private LinearGradientBrush _pickerBrush;

        #endregion FIELDS


        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color), typeof(ColorSpectrumSlider), new PropertyMetadata(Colors.Transparent));

        #endregion DEPENDENCY PROPERTIES


        #region PROPERTIES

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        #endregion PROPERTIES


        #region CONSTRUCTORS

        static ColorSpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSpectrumSlider),
                new FrameworkPropertyMetadata(typeof(ColorSpectrumSlider)));
        }

        #endregion CONSTRUCTORS


        #region METHODS

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _spectrumDisplay = (Border)GetTemplateChild(ElementSpectrumDisplay);
            CreateSpectrum();
            OnValueChanged(double.NaN, Value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            var color = ColorUtilities.ConvertHsvToRgb(newValue, 1, 1);
            SelectedColor = color;
        }

        private void CreateSpectrum()
        {
            _pickerBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0.5),
                EndPoint = new Point(1, 0.5),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };

            var colorsList = ColorUtilities.GenerateHsvSpectrum();

            var stopIncrement = (double)1 / colorsList.Count;

            int i;
            for (i = 0; i < colorsList.Count; i++)
                _pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i * stopIncrement));

            _pickerBrush.GradientStops[i - 1].Offset = 1.0;
            if (_spectrumDisplay != null)
                _spectrumDisplay.Background = _pickerBrush;
        }

        #endregion METHODS
    }
}