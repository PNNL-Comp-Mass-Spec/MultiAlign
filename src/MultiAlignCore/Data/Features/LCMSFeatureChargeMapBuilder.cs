using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    /// Given a UMC builds charge maps of it's features.
    /// </summary>
    public static class LCMSFeatureChargeMapBuilder
    {
        public static Dictionary<int, List<MSFeatureLight>> BuildChargeMap(UMCLight feature)
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
    }
}
