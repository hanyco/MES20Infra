using System.Globalization;
using System.Windows.Controls;

namespace HanyCo.Infra.UI.ValidationRules;

public sealed class TypeMemberNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var memberName = value?.ToString();
        if (memberName is null)
        {
            return new ValidationResult(false, "Cannot be empty");
        }
        else if (memberName.StartsWithAny(Enumerable.Range(0, 9).Select(x => x.ToString(CultureInfo.InvariantCulture))))
        {
            return new ValidationResult(false, "Illegal character.");
        }
        else if (memberName.Contains(' '))
        {
            return new ValidationResult(false, "Illegal character.");
        }
        else if (memberName.StartsWithAny("$", "!", "#", "@", "%", "^", "&", "*", "(", ")", "-", "+", "/", "\\"))
        {
            return new ValidationResult(false, "Illegal character.");
        }
        else
        {
            return ValidationResult.ValidResult;
        }
    }
}
