using System;
using System.Globalization;
using System.Windows.Data;
using MultiAlignCore.Data;

namespace MultiAlign.Converters
{
    public class DatasetPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var data = value.ToString();
            return DatasetLoader.ExtractDatasetName(data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}