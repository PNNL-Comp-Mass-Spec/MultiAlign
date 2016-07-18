using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiAlignCore.Algorithms.Clustering.ClusterPostProcessing
{
    using InformedProteomics.Backend.Data.Spectrometry;

    public class ClusterPostProcessingOptions
    {
        public enum ClusterComparisonType
        {
            MsMsSpectra,
            MsMsIdentifications,
        }

        public ClusterPostProcessingOptions()
        {
            this.ComparisonType = ClusterComparisonType.MsMsSpectra;
            this.MsMsComparisonTolerance = 5;
            this.MsMsComparisonToleranceUnit = ToleranceUnit.Ppm;
        }

        public bool ShouldPerformClusterRefinement { get; set; }

        public ClusterComparisonType ComparisonType { get; set; }

        public double MsMsComparisonTolerance { get; set; }

        public ToleranceUnit MsMsComparisonToleranceUnit { get; set; }
    }
}
