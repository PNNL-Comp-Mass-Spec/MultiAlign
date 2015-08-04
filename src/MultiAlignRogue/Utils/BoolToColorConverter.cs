using System;

namespace MultiAlignRogue.Utils
{
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts a boolean flag value to a color.
    /// True = red, False = white
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert boolean value to color.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="parameter">Parameter of value to convert.</param>
        /// <param name="culture">Culture of value to convert.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = Brushes.Transparent;
            try
            {
                bool boolValue = (bool)value;
                color = boolValue ? Brushes.Red : Brushes.Transparent;
            }
            catch (InvalidCastException)
            {
            }

            return color;
        }

        /// <summary>
        /// Convert color value to boolean value..
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="parameter">Parameter of value to convert.</param>
        /// <param name="culture">Culture of value to convert.</param>
        /// <returns>Converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolFlag = false;
            try
            {
                var colorValue = (Brush)value;
                boolFlag = colorValue.Equals(Brushes.Red);
            }
            catch (InvalidCastException)
            {
            }

            return boolFlag;
        }
    }
}
