﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiAlign.Converters
{
    /// <summary>
    /// Converts whether the path is null or not to yes or no string.
    /// </summary>
    public class HasPathConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "No";
            return "Yes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}