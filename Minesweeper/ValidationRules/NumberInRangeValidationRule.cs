using System.Globalization;
using System.Windows.Controls;

namespace Minesweeper.ValidationRules
{
    /// <summary>
    /// The Validation rule for max and min number allowed in text box.
    /// </summary>
    public class NumberInRangeValidationRule : ValidationRule
    {
        public ulong Max { get; set; }
        public ulong Min { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "number cannot be null");

            if (value is string str)
            {
                ulong.TryParse(str, out var number);

                if (number > Max || number < Min)
                    return new ValidationResult(false, "number too big or small");

                return new ValidationResult(true, "");
            }

            return new ValidationResult(false, "unknown error while validating");
        }
    }
}