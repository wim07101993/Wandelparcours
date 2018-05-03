using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModuleSettingsEditor.WPF.Helpers;

namespace ModuleSettingsEditor.WPF.Views.ColorPicker
{
    [TemplatePart(Name = ElementColorShadingCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementColorShadeSelector, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementSpectrumSlider, Type = typeof(ColorSpectrumSlider))]
    public class ColorPicker : Control
    {
        #region FIELDS

        private const string ElementColorShadingCanvas = "PART_ColorShadingCanvas";
        private const string ElementColorShadeSelector = "PART_ColorShadeSelector";
        private const string ElementSpectrumSlider = "PART_SpectrumSlider";

        private readonly TranslateTransform _colorShadeSelectorTransform = new TranslateTransform();
        private Canvas _colorShadingCanvas;
        private Canvas _colorShadeSelector;
        private ColorSpectrumSlider _spectrumSlider;
        private Point? _currentColorPosition;
        private bool _updateSpectrumSliderValue = true;

        #endregion FIELDS


        #region REGISTRATIONS

        public static readonly RoutedEvent SelectedColorChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectedColorChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color?>), typeof(ColorPicker));


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorPicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedColorChanged));

        #endregion REGISTRATIONS


        #region PROPERTIES

        public Color? SelectedColor
        {
            get => (Color?)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        #endregion PROPERTIES


        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker),
                new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        #endregion Constructors


        #region METHODS

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown -= ColorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp -= ColorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove -= ColorShadingCanvas_MouseMove;
            }

            _colorShadingCanvas = GetTemplateChild(ElementColorShadingCanvas) as Canvas;

            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown += ColorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp += ColorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove += ColorShadingCanvas_MouseMove;
            }

            _colorShadeSelector = GetTemplateChild(ElementColorShadeSelector) as Canvas;

            if (_colorShadeSelector != null)
                _colorShadeSelector.RenderTransform = _colorShadeSelectorTransform;

            if (_spectrumSlider != null)
                _spectrumSlider.ValueChanged -= SpectrumSlider_ValueChanged;

            _spectrumSlider = GetTemplateChild(ElementSpectrumSlider) as ColorSpectrumSlider;

            if (_spectrumSlider != null)
                _spectrumSlider.ValueChanged += SpectrumSlider_ValueChanged;

            UpdateColorShadeSelectorPosition(SelectedColor);
        }

        private void UpdateColorShadeSelectorPositionAndCalculateColor(Point p, bool calculateColor)
        {
            if (_colorShadingCanvas == null || _colorShadeSelector == null)
                return;

            if (p.Y < 0)
                p.Y = 0;

            if (p.X < 0)
                p.X = 0;

            if (p.X > _colorShadingCanvas.ActualWidth)
                p.X = _colorShadingCanvas.ActualWidth;

            if (p.Y > _colorShadingCanvas.ActualHeight)
                p.Y = _colorShadingCanvas.ActualHeight;

            _colorShadeSelectorTransform.X = p.X - _colorShadeSelector.Width / 2;
            _colorShadeSelectorTransform.Y = p.Y - _colorShadeSelector.Height / 2;

            p.X = p.X / _colorShadingCanvas.ActualWidth;
            p.Y = p.Y / _colorShadingCanvas.ActualHeight;

            _currentColorPosition = p;

            if (calculateColor)
                CalculateColor(p);
        }

        private void UpdateColorShadeSelectorPosition(Color? color)
        {
            if (_spectrumSlider == null || _colorShadingCanvas == null || color == null)
                return;

            _currentColorPosition = null;

            var hsv = ColorUtilities.ConvertRgbToHsv(color.Value.R, color.Value.G, color.Value.B);

            if (_updateSpectrumSliderValue)
                _spectrumSlider.Value = hsv.H;

            var p = new Point(hsv.S, 1 - hsv.V);

            _currentColorPosition = p;

            _colorShadeSelectorTransform.X = p.X * _colorShadingCanvas.Width - 5;
            _colorShadeSelectorTransform.Y = p.Y * _colorShadingCanvas.Height - 5;
        }

        private void CalculateColor(Point p)
        {
            if (_spectrumSlider == null)
                return;

            var hsv = new HsvColor(_spectrumSlider.Value, 1, 1)
            {
                S = p.X,
                V = 1 - p.Y
            };
            var currentColor = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            currentColor.A = 255;

            _updateSpectrumSliderValue = false;
            SelectedColor = currentColor;
            _updateSpectrumSliderValue = true;
        }

        private void SpectrumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_currentColorPosition != null && SelectedColor != null)
                CalculateColor((Point)_currentColorPosition);
        }

        #region mouse events

        private void ColorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;

            var p = e.GetPosition(_colorShadingCanvas);
            UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
            _colorShadingCanvas.CaptureMouse();
            //Prevent from closing ColorPicker after mouseDown in ListView
            e.Handled = true;
        }

        private void ColorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            => _colorShadingCanvas?.ReleaseMouseCapture();

        private void ColorShadingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_colorShadingCanvas == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            var p = e.GetPosition(_colorShadingCanvas);
            UpdateColorShadeSelectorPositionAndCalculateColor(p, true);
            Mouse.Synchronize();
        }

        #endregion mouse events

        #region dependency property changes

        private static void OnSelectedColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is ColorPicker This))
                return;

            var newValue = (Color?)e.NewValue;
            var oldValue = (Color?)e.OldValue;

            This.UpdateColorShadeSelectorPosition(newValue);

            var args = new RoutedPropertyChangedEventArgs<Color?>(oldValue, newValue)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            This.RaiseEvent(args);
        }

        #endregion dependency property changes

        #endregion METHODS


        #region Events

        public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, value);
            remove => RemoveHandler(SelectedColorChangedEvent, value);
        }

        #endregion Events
    }
}
