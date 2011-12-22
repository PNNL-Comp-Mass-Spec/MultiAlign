using System;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.IO.Parameters;
using MultiAlignCore.Data;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms;
using System.ComponentModel;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Options for clustering LC-MS Features.
    /// </summary>
    public class LCMSFeatureClusteringOptions
    {       
        public LCMSFeatureClusteringOptions()
        {            
			MassTolerance		    = 6.0; 
			NETTolerance		    = 0.03; 
			DriftTimeTolerance      = 50.0;			
			ClusterCentroid         = ClusterCentroidRepresentation.Median;
			IgnoreCharge		    = true;			
            AlignClusters	        = false;			
        }

        [ParameterFileAttribute("AlignClusters", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Align clusters to database")]
        [Category("AMT")]
        [Description("This is only valid if you have a mass tag database.  Setting to True will align each cluster to the database.")]
        public bool AlignClusters { get; set; }

        [ParameterFileAttribute("ClusterRepresentativeType", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Cluster Representative Type")]
        [Category("Centroid")]
        [Description("Determines how the centroid should be calculated.")]
        public ClusterCentroidRepresentation ClusterCentroid { get; set; }

        [ParameterFileAttribute("IgnoreCharge", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Ignore Charge")]
        [Category("Ion Mobility")]
        [Description("Set Ignore Charge to True if you are not using Ion Mobility (IMS).  Set to False if you are using IMS.")]
        public bool IgnoreCharge { get; set; }

        [ParameterFileAttribute("DriftTimeTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Drift Time Tolerance")]
        [Category("Tolerances")]
        [Description("For Ion Mobility Data.  Tolerance in drift time (ms) set to a large value if not using LC-MS")]
        public double DriftTimeTolerance { get; set; }


        [ParameterFileAttribute("MassTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("Mass Tolerance")]
        [Category("Tolerances")]
        [Description("Mass Tolerance in parts per million (PPM).")]
        public double MassTolerance { get; set; }

        [ParameterFileAttribute("NETTolerance", "LCMSFeatureClustering")]
        [DataSummaryAttribute("NET Tolerance")]
        [Category("Tolerances")]
        [Description("Normalized elution time (NET) tolerance.")]
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
