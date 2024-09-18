using System.Globalization;
using System.Windows.Controls;

namespace ImagesDownloader.ValidationRules;

internal class AbsoluteUriValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string? original = value as string;
        if (string.IsNullOrEmpty(original)
            || Uri.TryCreate(original, UriKind.Absolute, out _)) 
            return ValidationResult.ValidResult;
        return new ValidationResult(false, "Invalid absolute url");
    }
}
