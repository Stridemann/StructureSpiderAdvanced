using System;
using System.Windows.Controls;

namespace StructureSpiderAdvanced
{
    public class ScanValueValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return ExternalValidate(value);
        }

        public static ValidationResult ExternalValidate(object value)
        {
            switch (MainViewModel.Instance.SelectedDataType)
            {
                case DataType.Long:
                    try { Convert.ToInt64(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid Long value"); }
                    break;
                case DataType.Pointer:
                    try { Convert.ToUInt64(value.ToString(), 16); }
                    catch { return new ValidationResult(false, "Please enter a valid hex value"); }
                    break;
                case DataType.Int:
                    try { Convert.ToInt32(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid integer value."); }
                    break;
                case DataType.UInt:
                    try { Convert.ToUInt32(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid unsigned integer value."); }
                    break;
                case DataType.Byte:
                    try { Convert.ToInt32(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid byte value."); }
                    break;
                case DataType.Float:
                    try { Convert.ToSingle(value.ToString().Replace(".", ",")); }
                    catch { return new ValidationResult(false, "Please enter a valid float value."); }
                    break;
            }


            return new ValidationResult(true, null);
        }
    }
}