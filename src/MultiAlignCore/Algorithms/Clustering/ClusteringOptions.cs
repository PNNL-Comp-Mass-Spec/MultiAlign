using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class LCMSFeatureClusteringOptions
    {       
        public LCMSFeatureClusteringOptions()
        {            
			MassTolerance		    = 6.0; 
			NETTolerance		    = 0.03; 
			DriftTimeTolerance      = 50.0;			
			ClusterCentroid         = ClusterCentroidRepresentation.Median;
			IgnoreCharge		    = true;			
            AlignClusters	        = true;			
        }

        [ParameterFileAttribute("AlignClusters", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Align clusters to database")]
        public bool AlignClusters { get; set; }

        [ParameterFileAttribute("ClusterRepresentativeType", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Cluster Representative Type")]
        public ClusterCentroidRepresentation ClusterCentroid { get; set; }

        [ParameterFileAttribute("DriftTimeTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Drift Time Tolerance")]
        public double DriftTimeTolerance { get; set; }

        [ParameterFileAttribute("IgnoreCharge", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Ignore Charge")]
        public bool IgnoreCharge { get; set; }

        [ParameterFileAttribute("MassTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Mass Tolerance")]
        public double MassTolerance { get; set; }

        [ParameterFileAttribute("NETTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("NET Tolerance")]
        public double NETTolerance { get; set; }

        public static FeatureClusterParameters<UMCLight> ConvertToOmics(LCMSFeatureClusteringOptions options)
        {            
            FeatureTolerances tolerances                        = new FeatureTolerances();
            FeatureClusterParameters<UMCLight> parameters       = new FeatureClusterParameters<UMCLight>();
            tolerances.DriftTime                                = options.DriftTimeTolerance;
            tolerances.Mass                                     = options.MassTolerance;
            tolerances.RetentionTime                            = options.NETTolerance;            
            parameters.Tolerances                               = tolerances;            
            parameters.OnlyClusterSameChargeStates              = (options.IgnoreCharge == false);
            parameters.CentroidRepresentation                   = options.ClusterCentroid;  

            return parameters;
        }
    }
    
}
