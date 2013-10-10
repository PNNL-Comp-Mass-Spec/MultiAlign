using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using MultiAlignCore.Data;
using System.IO;

namespace MultiAlign.Converters
{

    public class SearchOptionConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null)
                return false;

            SearchOption option = SearchOption.TopDirectoryOnly;
            try
            {
                option = (SearchOption)value;
            }
            catch
            {
            }

            if (option == SearchOption.AllDirectories)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return SearchOption.TopDirectoryOnly;

            bool status = false;

            try
            {
                status = (bool)value;
            }catch
            {
            }

            if (status)
                return SearchOption.AllDirectories;
            else
                return SearchOption.TopDirectoryOnly;
        }
        #endregion
    }
}
