using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class SearchOptionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var option = SearchOption.TopDirectoryOnly;
            try
            {
                option = (SearchOption) value;
            }
            catch
            {
            }

            if (option == SearchOption.AllDirectories)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SearchOption.TopDirectoryOnly;

            bool status = false;

            try
            {
                status = (bool) value;
            }
            catch
            {
            }

            if (status)
                return SearchOption.AllDirectories;
            return SearchOption.TopDirectoryOnly;
        }

        #endregion
    }
}