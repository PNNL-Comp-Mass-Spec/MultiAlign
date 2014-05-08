using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class OrientationMsMsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            var orient = (Orientation) value;


            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}