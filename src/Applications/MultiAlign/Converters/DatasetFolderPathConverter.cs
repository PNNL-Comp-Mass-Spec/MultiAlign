using System;
using System.Windows.Data;

namespace MultiAlign.Converters
{

    public class DatasetFolderPathConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var data = value.ToString();
            return System.IO.Path.GetDirectoryName(data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
