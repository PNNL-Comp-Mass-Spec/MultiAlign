using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    public class BoolWidthConverter : DependencyObject, IValueConverter
    {
        // Using a DependencyProperty as the backing store for TrueWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrueWidthProperty =
            DependencyProperty.Register("TrueWidth", typeof (GridLength), typeof (BoolWidthConverter),
                new UIPropertyMetadata(new GridLength(1, GridUnitType.Star)));


        // Using a DependencyProperty as the backing store for TrueVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FalseVisibilityProperty =
            DependencyProperty.Register("FalseWidth", typeof (GridLength), typeof (BoolWidthConverter),
                new UIPropertyMetadata(new GridLength(1, GridUnitType.Star)));

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = new GridLength(1, GridUnitType.Star);
            if (value == null)
            {
                return x;
            }


            var boolValue = (bool) value;
            if (!boolValue)
            {
                return new GridLength(0, GridUnitType.Pixel);
            }
            return x;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

        #endregion

        public GridLength TrueWidth
        {
            get { return (GridLength) GetValue(TrueWidthProperty); }
            set { SetValue(TrueWidthProperty, value); }
        }

        public GridLength FalseWidth
        {
            get { return (GridLength) GetValue(FalseVisibilityProperty); }
            set { SetValue(FalseVisibilityProperty, value); }
        }
    }
}