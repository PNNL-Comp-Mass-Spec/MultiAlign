using System;
using System.Windows;
using System.Windows.Data;
using Manassa.Windows;
using Manassa.Data;
using System.ComponentModel;
using Manassa;

namespace Manassa.Converters
{

    public class ApplicationStateVisibilityConverter : DependencyObject, IValueConverter
    {
        public ApplicationAnalysisState ApplicationState
        {
            get { return (ApplicationAnalysisState)GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("ApplicationState", typeof(ApplicationAnalysisState), typeof(ApplicationStateVisibilityConverter));

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Hidden;
            }

            ApplicationAnalysisState step   = (ApplicationAnalysisState)value;
            Visibility vis                  = Visibility.Hidden;

            if (step == ApplicationState)
            {
                vis = Visibility.Visible;
            }
            return vis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
