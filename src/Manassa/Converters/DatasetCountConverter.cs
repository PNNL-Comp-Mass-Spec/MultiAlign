using System;
using System.Windows;
using System.Windows.Data;

namespace Manassa.Converters
{
    
    public class DatasetCountConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "No Datasets are selected.";
            }

            string message = "No datasets are selected.";
            try
            {
                int datasets = (int)value;
                if (datasets > 0)
                {
                    message = string.Format("{0} datasets selected.", datasets);
                }
            }
            catch
            {

            }
            return message;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
