using System;
using System.Windows;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    /// <summary>
    /// Converts an integer value to a visibility item.
    /// </summary>
    public class IntVisibilityConverter : DependencyObject, IValueConverter
    {




        public int FalseValue
        {
            get { return (int)GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FalseValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register("FalseValue", typeof(int), typeof(IntVisibilityConverter), new UIPropertyMetadata(0));

        
        

        public int TrueValue
        {
            get { return (int)GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrueValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register("TrueValue", typeof(int), typeof(IntVisibilityConverter), new UIPropertyMetadata(128));

        
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasData = (bool)value;

            if (hasData)
            {
                return TrueValue;
            }
            else
            {
                return FalseValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("There is no way to convert back a visibility value to the original integer with this converter.");
        }
        #endregion
    }
}
