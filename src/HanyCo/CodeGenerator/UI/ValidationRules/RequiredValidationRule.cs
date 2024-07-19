using System.Globalization;
using System.Windows.Controls;

namespace HanyCo.Infra.UI.ValidationRules;

public sealed class RequiredValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
        value is null || value.ToString().IsNullOrEmpty()
            ? new ValidationResult(false, "Required.")
            : ValidationResult.ValidResult;
}
