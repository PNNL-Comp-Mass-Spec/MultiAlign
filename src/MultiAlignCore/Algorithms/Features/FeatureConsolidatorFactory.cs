using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.Features
{
    public static class FeatureConsolidatorFactory
    {
        /// <summary>
        /// Create a LCMS Feature consolidator.
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static LCMSFeatureConsolidator CreateConsolidator(enmAbundanceReportingType report)
        {
            LCMSFeatureConsolidator consolidator = null;
            switch(report)
            {
                case enmAbundanceReportingType.PeakArea:
                    consolidator = new UMCAbundanceSumConsolidator();
                    break;
                case enmAbundanceReportingType.PeakMax:
                    consolidator = new UMCAbundanceConsolidator();
                    break;
            }
            return consolidator;
        }
    }
}
