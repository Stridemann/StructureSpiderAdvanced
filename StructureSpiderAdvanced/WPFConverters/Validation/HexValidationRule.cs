using System;
using System.Windows.Controls;

namespace StructureSpiderAdvanced
{
    public class HexValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                Convert.ToInt64(value.ToString(), 16);
            }
            catch
            {
                return new ValidationResult(false, "Please enter a valid hex value.");
            }

            return new ValidationResult(true, null);
        }
    }
}
