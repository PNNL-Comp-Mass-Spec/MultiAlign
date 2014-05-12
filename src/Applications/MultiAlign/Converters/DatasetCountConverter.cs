using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class DatasetCountConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "No Datasets are selected.";
            }

            var message = "No datasets are selected.";
            try
            {
                var datasets = (int) value;
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}