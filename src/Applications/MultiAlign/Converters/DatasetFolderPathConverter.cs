using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class DatasetFolderPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            string data = value.ToString();
            return Path.GetDirectoryName(data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}