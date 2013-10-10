using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiAlign.Converters
{
    public class DatasetColorConverter: DependencyObject, IValueConverter      
    {

        public DatasetColorConverter()
        {
            
        }

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

            bool isAlignedToDatabase = (bool) value;
            Color x = Colors.Green;

            if (IsDataset == true)
            {
                if (isAlignedToDatabase == true)
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
