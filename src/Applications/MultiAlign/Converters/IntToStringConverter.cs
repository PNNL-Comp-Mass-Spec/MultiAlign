using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    /// <summary>
    /// Converts an integer value to a visibility item.
    /// </summary>
    public class IntToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            var x = (int) value;

            return x.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(
                "There is no way to convert back a visibility value to the original integer with this converter.");
        }

        #endregion
    }
}