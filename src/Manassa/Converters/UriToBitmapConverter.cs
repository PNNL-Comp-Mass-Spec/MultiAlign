using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Manassa.Converters
{
    public class UriToBitmapConverter : IValueConverter
    {
       public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {

           if (value == null)
               return null;
           
           BitmapImage bi       = new BitmapImage();
           bi.BeginInit();
           bi.DecodePixelWidth  = 128;
           bi.CacheOption       = BitmapCacheOption.OnLoad;
           bi.UriSource         = new Uri( value.ToString() );
           bi.EndInit();
           return bi;
       }
    
       public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           throw new Exception("The method or operation is not implemented.");
       }
   }
}
