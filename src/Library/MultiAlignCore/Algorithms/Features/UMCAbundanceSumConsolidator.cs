using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureFinding;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Features
{
    /// <summary>
    /// Consolidates similar features from the same dataset into one representative LCMS Feature 
    /// based on max abundance.
    /// </summary>
    public class UMCAbundanceSumConsolidator: LCMSFeatureConsolidator
    {
        public UMCAbundanceSumConsolidator()
        {
            AbundanceType = AbundanceReportingType.Sum;
        }

        /// <summary>
        /// Organizes a list of UMC's into a dataset represented group.
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public override Dictionary<int, UMCLight> ConsolidateUMCs(List<UMCLight> features)
        {
            // Maps the UMC abundances to sum across the groups.
            var umcAbundanceMap = new Dictionary<int, long>();
            var umcAbundanceSumMap = new Dictionary<int, long>();

            // Map the UMC's to datasets so we can choose only one.
            var umcs = new Dictionary<int, UMCLight>();
            foreach (var umc in features)
            {
                var group = umc.GroupId;
                if (!umcs.ContainsKey(group))
                {
                    umcs.Add(group, umc);
                    umcAbundanceMap.Add(group, umc.Abundance);
                    umcAbundanceSumMap.Add(group, umc.AbundanceSum);
                }
                else
                {
                    
                    umcAbundanceMap[group]    += umc.Abundance; 
                    umcAbundanceSumMap[group] += umc.AbundanceSum;                         
                              
                    switch(AbundanceType)                    
                    {
                        case AbundanceReportingType.Max:
                            if (umc.Abundance > umcs[group].Abundance)
                            {                        
                                umcs[group]             = umc;
                            }
                            break;
                        case AbundanceReportingType.Sum:
                            if (umc.AbundanceSum > umcs[group].AbundanceSum)
                            {                        
                                umcs[group]             = umc;
                            }
                            break;
                    }
                }
            }
            // Setting the abundance value for each used UMC after summing.
            foreach (var key in umcs.Keys)
            {
                umcs[key].Abundance    = umcAbundanceMap[key];
                umcs[key].AbundanceSum = umcAbundanceSumMap[key];
            }
            return umcs;
        }

        /// <summary>
        /// Gets or sets the way the UMC is selected.
        /// </summary>
        public AbundanceReportingType AbundanceType
        {
            get;
            set;
        }
    }    
}
