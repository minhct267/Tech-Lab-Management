using System.Globalization;
using System.Windows.Controls;

namespace TechLabManagement.Validations;

public sealed class MinLengthValidationRule : ValidationRule
{
	public int MinLength { get; set; } = 1;

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var text = value as string ?? string.Empty;
		return text.Trim().Length >= MinLength
			? ValidationResult.ValidResult
			: new ValidationResult(false, $"Minimum {MinLength} characters.");
	}
}


