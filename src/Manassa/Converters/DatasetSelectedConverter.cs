using System;
using System.Windows;
using System.Windows.Data;

namespace MultiAlign.Converters
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

            //BLL Make sure that we validate it's working.
            bool isEnabled = true;
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
