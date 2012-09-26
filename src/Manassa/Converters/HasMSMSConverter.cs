using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Windows.Controls;
using PNNLOmics.Data.Features;

namespace Manaassa.Converters
{
    public class HasMSMSConverter: IValueConverter
    {       
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UMCLight feature = (UMCLight)value;
            List<MSFeatureLight> msFeatures = feature.MSFeatures.FindAll(delegate(MSFeatureLight msFeature)
            {
                return msFeature.MSnSpectra.Count > 0;
            }
            );

            int? returnValue = 0;
            if (msFeatures == null)
            {
                return returnValue;
            }
            returnValue = msFeatures.Count;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UMCLight feature = (UMCLight)value;            
            List<MSFeatureLight> msFeatures = feature.MSFeatures.FindAll(delegate(MSFeatureLight msFeature)
                {
                    return msFeature.MSnSpectra.Count > 0;
                }
            );

            int? returnValue = 0;
            if (msFeatures == null)
            {
                return returnValue;
            }
            returnValue = msFeatures.Count;
            return returnValue;
        }

        #endregion
    }
}
