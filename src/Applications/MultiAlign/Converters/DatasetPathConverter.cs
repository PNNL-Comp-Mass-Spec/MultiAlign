using System;
using System.Windows.Data;
using MultiAlignCore.Data.MetaData;

namespace MultiAlign.Converters
{

    public class DatasetPathConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var data = value.ToString();
            return DatasetInformation.ExtractDatasetName(data);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
