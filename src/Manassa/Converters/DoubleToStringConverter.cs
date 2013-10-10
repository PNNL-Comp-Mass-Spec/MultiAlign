using System;
using System.Windows;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    /// <summary>
    /// Converts an integer value to a visibility item.
    /// </summary>
    public class DoubleToStringConverter : DependencyObject,  IValueConverter
    {
        
        public DoubleToStringConverter()
        {
            Precision = 5;
        }

        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecisionProperty = 
            DependencyProperty.Register("Precision", typeof(int), typeof(DoubleToStringConverter));

        private string m_format = null; 
        private string GetFormat()
        {
            if (m_format == null)
            {
                m_format = string.Format("N{0}", Precision);
            }
            return m_format;
        }

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";

            double x = (double)value;
            string format = GetFormat();
            return x.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("There is no way to convert back a visibility value to the original integer with this converter.");
        }
        #endregion
    }
}
