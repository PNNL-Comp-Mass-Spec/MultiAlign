#region

using System.Collections.Generic;

#endregion

namespace MultiAlignCore.Data.Features
{
    /// <summary>
    ///     Given a UMC builds charge maps of it's features.
    /// </summary>
    public static class LCMSFeatureChargeMapBuilder
    {
        public static Dictionary<int, List<MSFeatureLight>> BuildChargeMap(UMCLight feature)
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

            return chargeMap;
        }
    }
}