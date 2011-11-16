using System.Collections.Generic;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Features
{
    /// <summary>
    /// Consolidates similar features from the same dataset into one representative LCMS Feature 
    /// based on max abundance.
    /// </summary>
    public class UMCAbundanceConsolidator: LCMSFeatureConsolidator
    {
        /// <summary>
        /// Organizes a list of UMC's into a dataset represented group.
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public override Dictionary<int, UMCLight> ConsolidateUMCs(List<UMCLight> features)
        {
            // Map the UMC's to datasets so we can choose only one.
            Dictionary<int, UMCLight> umcs = new Dictionary<int, UMCLight>();
            foreach (UMCLight umc in features)
            {
                int group = umc.GroupID;
                if (!umcs.ContainsKey(group))
                {
                    umcs.Add(group, umc);
                }
                else
                {
                    // choose the max abundance...
                    if (umc.Abundance > umcs[group].Abundance)
                    {
                        umcs[group] = umc;
                    }
                }
            }
            return umcs;
        }
    }
}
