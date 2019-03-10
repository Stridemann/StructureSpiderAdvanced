using System;
using System.Windows;
using System.Windows.Data;


namespace StructureSpiderAdvanced
{
    public sealed class ScanTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (DataType)value;
            if (parameter.Equals("StringLength"))
            {
                if (type == DataType.String || type == DataType.StringU)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            else if (parameter.Equals("NotString"))
            {
                if (type == DataType.String || type == DataType.StringU)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
