using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace ModuleSettingsEditor.WPF.Helpers
{
    public class IPValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var strValue = value?.ToString();

            return string.IsNullOrEmpty(strValue) || IPAddress.TryParse(strValue, out var _)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "De ingegeven waarde is geen IP-adres");
        }
    }
}