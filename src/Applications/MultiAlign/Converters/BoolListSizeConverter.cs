using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using MultiAlignCore.Data;

namespace MultiAlign.Converters
{
    public class BoolListSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            var bValue = value as ObservableCollection<MassTagToCluster>;

            if (bValue == null)
                return false;

            return bValue.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

        #endregion
    }
}