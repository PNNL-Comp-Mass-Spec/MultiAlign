using System.Collections.Generic;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Features
{
    /// <summary>
    /// Consolidates similar features from the same dataset into one representative LCMS Feature 
    /// based on max abundance.
    /// </summary>
    public class UMCAbundanceSumConsolidator: LCMSFeatureConsolidator
    {
        /// <summary>
        /// Organizes a list of UMC's into a dataset represented group.
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public override Dictionary<int, UMCLight> ConsolidateUMCs(List<UMCLight> features)
        {
            // Maps the UMC abundances to sum across the groups.
            Dictionary<int, long> umcAbundanceMap = new Dictionary<int, long>();

            // Map the UMC's to datasets so we can choose only one.
            Dictionary<int, UMCLight> umcs = new Dictionary<int, UMCLight>();
            foreach (UMCLight umc in features)
            {
                int group = umc.GroupID;
                if (!umcs.ContainsKey(group))
                {
                    umcs.Add(group, umc);
                    umcAbundanceMap.Add(group, umc.Abundance);
                }
                else
                {
                    // Sum the abundance.
                    umcAbundanceMap[group] += umc.Abundance;                                            
                    if (umc.Abundance > umcs[group].Abundance)
                    {                        
                        umcs[group]             = umc;
                    }
                }
            }
            // Setting the abundance value for each used UMC after summing.
            foreach (int key in umcs.Keys)
            {
                umcs[key].Abundance = umcAbundanceMap[key];
            }
            return umcs;
        }
    }    
}
