#region

using System.ComponentModel;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.IO.Parameters;

#endregion

namespace MultiAlignCore.Algorithms.Features
{
    public class FeatureConsolidatorOptions
    {
        public FeatureConsolidatorOptions()
        {
            AbundanceType = AbundanceReportingType.Sum;
        }

        /// <summary>
        /// </summary>
        [ParameterFile("AbundanceType", "FeatureConsolidator")]
        [Description(
            "Determines how the abundance from features of the same dataset in a cluster will be reported.  Either by summing their abundance, or by taking the maximum"
            )]
        [Category("Abundance")]
        public AbundanceReportingType AbundanceType { get; set; }
    }
}