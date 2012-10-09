using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using MultiAlignCore.Data;

namespace Manassa.Converters
{

    public class BoolListSizeConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null)
                return false;

            ObservableCollection<MassTagToCluster> bValue = value as ObservableCollection<MassTagToCluster>;

            if (bValue == null)
                return false;

            return bValue.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {           
            return false;
        }
        #endregion
    }
}
