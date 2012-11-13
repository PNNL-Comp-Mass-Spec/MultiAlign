using System;
using System.Windows;
using System.Windows.Data;

namespace Manassa.Converters
{
    
    public class DatasetSelectedConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            bool isEnabled = false;
            try
            {
                int datasets = (int)value;
                if (datasets > 0)
                {
                    isEnabled = true;
                }
            }
            catch
            {

            }
            return isEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
