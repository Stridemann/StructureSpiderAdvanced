using System;
using System.Windows.Data;

namespace StructureSpiderAdvanced
{
    public class OffsetConverter : IValueConverter
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
                int val = System.Convert.ToInt32(value);
                if (val == -1)
                    return "";

                return val.ToString("X");
            }
            else
            {
                long output = 0;
                try
                {
                    output = System.Convert.ToInt32(value.ToString(), 16);
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
