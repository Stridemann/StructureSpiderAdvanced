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
                    catch { return new ValidationResult(false, "Please enter a valid Int64 value"); }
                    break;
                case DataType.Pointer:
                    try { Convert.ToUInt64(value.ToString(), 16); }
                    catch { return new ValidationResult(false, "Please enter a valid hex value"); }
                    break;
                case DataType.Int:
                    try { Convert.ToInt32(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid Int32 value."); }
                    break;
                case DataType.UInt:
                    try { Convert.ToUInt32(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid UInt32 value."); }
                    break;
                case DataType.Short:
                    try { Convert.ToInt16(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid Int16 value."); }
                    break;
                case DataType.UShort:
                    try { Convert.ToUInt16(value.ToString()); }
                    catch { return new ValidationResult(false, "Please enter a valid UInt16 value."); }
                    break;
                case DataType.Byte:
                    try { Convert.ToByte(value.ToString()); }
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