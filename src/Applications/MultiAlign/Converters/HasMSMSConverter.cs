using System;
using System.Windows.Data;
using PNNLOmics.Data.Features;

namespace MultiAlign.Converters
{
    public class HasMSMSConverter: IValueConverter
    {       
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var feature = value as UMCLight;
            if (feature != null)
            {
                var msFeatures = feature.MsFeatures.FindAll(delegate(MSFeatureLight msFeature)
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
            var featureLight = value as MSFeatureLight;
            if (featureLight != null)
            {
                return featureLight.MSnSpectra.Count;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var feature = (UMCLight)value;            
            var msFeatures = feature.MsFeatures.FindAll(delegate(MSFeatureLight msFeature)
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
