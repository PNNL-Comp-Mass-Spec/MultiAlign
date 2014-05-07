using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiAlign.Converters
{
    public class DatasetColorConverter: DependencyObject, IValueConverter      
    {
        public bool?  IsDataset

        {
            get { return (bool?)GetValue(IsDatasetProperty); }
            set { SetValue(IsDatasetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Analysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDatasetProperty =
            DependencyProperty.Register("IsDataset", typeof(bool?),
            typeof(DatasetColorConverter));

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            var isAlignedToDatabase = (bool) value;
            var x = Colors.Green;

            if (IsDataset == true)
            {
                if (isAlignedToDatabase)
                {
                    x = Colors.LightGray;
                }
            }
            else
            {
                if (isAlignedToDatabase == false)
                {
                    x = Colors.LightGray;
                }
            }

            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
