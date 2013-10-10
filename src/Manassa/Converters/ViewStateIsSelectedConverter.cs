﻿using System;
using System.Windows;
using System.Windows.Data;
using MultiAlign.Windows;
using MultiAlign.Data;
using System.ComponentModel;
using MultiAlign;
using MultiAlign.Data.States;

namespace MultiAlign.Converters
{

    public class ViewStateIsSelectedConverter : DependencyObject, IValueConverter
    {
        public ViewState ViewState
        {
            get { return (ViewState)GetValue(VisibleStepProperty); }
            set { SetValue(VisibleStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleStepProperty =
            DependencyProperty.Register("ViewState", typeof(ViewState), typeof(ViewStateIsSelectedConverter));

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            ViewState step = (ViewState)value;
            bool? vis = false;

            if (step == ViewState)
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
