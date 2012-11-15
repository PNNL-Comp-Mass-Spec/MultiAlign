using System;
using System.Windows;
using System.Windows.Data;
using Manassa.Windows;
using Manassa.Data;
using System.ComponentModel;
using Manassa;

namespace Manassa.Converters
{

    public class ApplicationStateIsSelectedConverter : DependencyObject, IValueConverter
    {
        public ApplicationAnalysisState ApplicationState
        {
            get { return (ApplicationAnalysisState)GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("ApplicationState", typeof(ApplicationAnalysisState), typeof(ApplicationStateIsSelectedConverter));

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            ApplicationAnalysisState step   = (ApplicationAnalysisState)value;
            bool? vis = false;

            if (step == ApplicationState)
            {
                vis = true;
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
