using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MultiAlign.Data.States;

namespace MultiAlign.Converters
{
    public class ViewStateIsSelectedConverter : DependencyObject, IValueConverter
    {
        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("ViewState", typeof (ViewState), typeof (ViewStateIsSelectedConverter));

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var step = (ViewState) value;
            bool? vis = false;

            if (step == ViewState)
            {
                vis = true;
            }
            return vis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion

        public ViewState ViewState
        {
            get { return (ViewState) GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }
    }
}