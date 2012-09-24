using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;

namespace MultiAlignCore.Extensions
{
    public static class FeatureExtensions
    {
        /// <summary>
        /// Creates a charge map for a given ms feature list.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<MSFeatureLight>> CreateChargeMap(this UMCLight feature)
        {
            Dictionary<int, List<MSFeatureLight>> chargeMap = new Dictionary<int, List<MSFeatureLight>>();
            foreach (MSFeatureLight msFeature in feature.MSFeatures)
            {
                if (!chargeMap.ContainsKey(msFeature.ChargeState))
                {
                    chargeMap.Add(msFeature.ChargeState, new List<MSFeatureLight>());
                }
                chargeMap[msFeature.ChargeState].Add(msFeature);
            }

            return chargeMap;
        }
        /// <summary>
        /// Creates SIC's mapped by charge state for the MS Features in the feature.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static Dictionary<int, List<XYZData>> CreateChargeSIC(this UMCLight feature)
        {
            Dictionary<int, List<MSFeatureLight>> chargeMap = feature.CreateChargeMap();
            Dictionary<int, List<XYZData>> sicMap = new Dictionary<int, List<XYZData>>();
                        
            foreach (int charge in chargeMap.Keys)
            {
                chargeMap[charge].Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                {
                    return x.Scan.CompareTo(y.Scan);
                }
                );
                List<XYZData> data = chargeMap[charge].ConvertAll<XYZData>(x => new XYZData(x.Scan, x.Abundance, x.Mz));
                sicMap.Add(charge, data);
            }
            return sicMap;
        }

        
    }
}
