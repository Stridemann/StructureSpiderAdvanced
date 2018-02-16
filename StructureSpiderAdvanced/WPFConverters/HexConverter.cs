using System;
using System.Windows.Data;

namespace StructureSpiderAdvanced
{
    public class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ExplConvert(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ExplConvert(value, targetType);
        }


        private object ExplConvert(object value, Type targetType)
        {
            if (targetType == typeof(string))
            {
                long longVal = System.Convert.ToInt64(value);
                return "0x" + longVal.ToString("X");
            }
            else
            {
                long output = 0;
                try
                {
                    output = System.Convert.ToInt64(value.ToString(), 16);
                }
                catch
                {
                    // MessageBox.Show("Can't convert " + value + " to long: " + ex.Message);
                }
                return output;
            }
        }
    }
}
