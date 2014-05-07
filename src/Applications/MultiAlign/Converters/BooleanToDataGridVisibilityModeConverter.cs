using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class BooleanToDataGridVisibilityModeConverter: IValueConverter
    {       
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DataGridRowDetailsVisibilityMode.Collapsed;

            var boolValue = System.Convert.ToBoolean(value);

            return (boolValue) ? DataGridRowDetailsVisibilityMode.Visible: DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var vis = (DataGridRowDetailsVisibilityMode)value;
            
            if (vis == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
