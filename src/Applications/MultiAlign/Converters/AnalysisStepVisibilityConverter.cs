using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MultiAlign.Data.States;

namespace MultiAlign.Converters
{
    public class AnalysisStepVisibilityConverter : DependencyObject, IValueConverter
    {
        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("VisibleStep", typeof (AnalysisSetupStep),
                typeof (AnalysisStepVisibilityConverter));

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Hidden;
            }

            var step = (AnalysisSetupStep) value;
            bool? vis = false;

            if (step == VisibleStep)
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

        public AnalysisSetupStep VisibleStep
        {
            get { return (AnalysisSetupStep) GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }
    }
}