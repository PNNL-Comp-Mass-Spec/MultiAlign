using System;
using System.Globalization;
using System.Windows.Data;
using MultiAlignCore.Data.MetaData;

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

            string data = value.ToString();
            return DatasetInformation.ExtractDatasetName(data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}