#region

using MultiAlignCore.Algorithms.FeatureFinding;

#endregion

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
        public static LCMSFeatureConsolidator CreateConsolidator(AbundanceReportingType report,
            AbundanceReportingType umcBuilding)
        {
            LCMSFeatureConsolidator consolidator = null;
            switch (report)
            {
                case AbundanceReportingType.Sum:
                    var sumConsolidate = new UMCAbundanceSumConsolidator();
                    sumConsolidate.AbundanceType = umcBuilding;
                    consolidator = sumConsolidate;
                    break;
                case AbundanceReportingType.Max:
                    var maxConsolidate = new UMCAbundanceConsolidator();
                    maxConsolidate.AbundanceType = umcBuilding;
                    consolidator = maxConsolidate;
                    break;
            }
            return consolidator;
        }
    }
}