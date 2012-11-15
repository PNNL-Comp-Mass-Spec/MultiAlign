using System;
using System.Windows;
using System.Windows.Data;
using Manassa.Windows;
using Manassa.Data;
using System.ComponentModel;

namespace Manassa.Converters
{

    public class AnalysisStepVisibilityConverter : DependencyObject, IValueConverter
    {
        public AnalysisSetupStep VisibleStep
        {
            get { return (AnalysisSetupStep)GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("VisibleStep", typeof(AnalysisSetupStep), typeof(AnalysisStepVisibilityConverter));

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Hidden;
            }

            AnalysisSetupStep step  = (AnalysisSetupStep)value;
            bool? vis          = false;

            if (step == VisibleStep)
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
