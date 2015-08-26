using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Extensions
{
    public static class FeatureListExtensions
    {
        /// <summary>
        /// Determines the magnitude of the scan range beyond what is defined by the MSFeatures min and max scan based on percentage of total range. 
        /// </summary>
        /// <param name="features"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static int ExtendedScanRange(this List<MSFeatureLight> features, double percentage)
        {
            var max = features.Max(feature => feature.Scan);
            var min = features.Min(feature => feature.Scan);

            var delta = Math.Abs(max - min);

            return Convert.ToInt32(Convert.ToDouble(delta) * percentage);
        }

        public static int MaxScan<T>(this List<T> features) where T : MSFeatureLight
        {
            return features.Max(feature => feature.Scan);
        }
        public static int MinScan<T>(this List<T> features) where T : MSFeatureLight
        {
            return features.Min(feature => feature.Scan);
        }
        public static double MaxAbundance<T>(this List<T> features) where T : MSFeatureLight
        {
            return features.Max(feature => feature.Abundance);
        }
        public static double MinAbundance<T>(this List<T> features) where T : MSFeatureLight
        {
            return features.Min(feature => feature.Abundance);
        }
        public static double Median<T>(this List<T> features) where T : MSFeatureLight
        {
            features.Sort(delegate(T x, T y)
            {
                return x.Mz.CompareTo(y.Mz);
            }
            );
            var count = features.Count / 2;
            return features[count].Mz;
        }
    }

    public static class UMCLightExtensions
    {

        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSFeatureLight>> CreateChargeMap(this UMCLight feature)
        {
            var chargeMap = new Dictionary<int, List<MSFeatureLight>>();
            foreach (var msFeature in feature.MsFeatures)
            {
                if (!chargeMap.ContainsKey(msFeature.ChargeState))
                {
                    chargeMap.Add(msFeature.ChargeState, new List<MSFeatureLight>());
                }
                chargeMap[msFeature.ChargeState].Add(msFeature);
            }

            var newChargeMap = new Dictionary<int, List<MSFeatureLight>>();            
            foreach (var charge in chargeMap.Keys)
            {
                var ordered       = chargeMap[charge].OrderBy(x => x.Scan);
                newChargeMap.Add(charge, ordered.ToList());                
            }
            return newChargeMap;
        }
    }


    public static class XYDataListExtensions
    {
        /// <summary>
        /// Finds the closest matching XY data value based on mz assumes list is sorted in ascending order.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XYData FindByMZ(this List<XYData> data, double mz)
        {
            var i = 0;
            for(var j = 0; j < data.Count; j++)
            {
                if (data[j].X > mz)
                {
                    var diffI = Math.Abs(data[i].X - mz);
                    var diffJ = Math.Abs(data[j].X - mz);

                    if (diffI < diffJ)
                        return data[i];
                    return data[j];
                }
                i = j;                
            }
            return data[data.Count - 1];
        }
    }
}
