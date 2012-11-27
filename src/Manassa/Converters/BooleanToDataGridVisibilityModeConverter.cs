using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Windows.Controls;

namespace Manassa.Converters
{
    public class BooleanToDataGridVisibilityModeConverter: IValueConverter
    {       
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DataGridRowDetailsVisibilityMode.Collapsed;

            bool boolValue = System.Convert.ToBoolean(value);

            return (boolValue) ? DataGridRowDetailsVisibilityMode.Visible: DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridRowDetailsVisibilityMode vis = (DataGridRowDetailsVisibilityMode)value;

            if (vis == DataGridRowDetailsVisibilityMode.Collapsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
