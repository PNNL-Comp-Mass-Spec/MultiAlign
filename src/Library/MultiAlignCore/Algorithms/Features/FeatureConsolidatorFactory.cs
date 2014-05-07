using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine.Features;
using MultiAlignCore.Algorithms.FeatureFinding;

namespace MultiAlignCore.Algorithms.Features
{
    public static class FeatureConsolidatorFactory
    {
        /// <summary>
        /// Create a LCMS Feature consolidator.
        /// </summary>
        /// <param name="report">Determines what type of UMC Consolidator to build.</param>
        /// <param name="umcBuilding">Determines how the UMC abundance was reported</param>
        /// <returns>Conslidator object.</returns>
        public static LCMSFeatureConsolidator CreateConsolidator(   AbundanceReportingType report, 
                                                                    AbundanceReportingType umcBuilding)
        {
            LCMSFeatureConsolidator consolidator = null;
            switch(report)
            {
                case AbundanceReportingType.Sum:
                    UMCAbundanceSumConsolidator sumConsolidate  = new UMCAbundanceSumConsolidator();
                    sumConsolidate.AbundanceType                = umcBuilding; 
                    consolidator                                = sumConsolidate;
                    break;
                case AbundanceReportingType.Max:
                    UMCAbundanceConsolidator maxConsolidate     = new UMCAbundanceConsolidator();
                    maxConsolidate.AbundanceType                = umcBuilding;
                    consolidator                                = maxConsolidate;
                    break;
            }
            return consolidator;
        }
    }
}
