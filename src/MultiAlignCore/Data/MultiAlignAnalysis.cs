using System;
using System.Collections.Generic;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;
using MultiAlignEngine.PeakMatching;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Algorithms.FeatureFinding;

namespace MultiAlignCore.Data
{
	[Serializable()]
	public class MultiAlignAnalysis : IDisposable
    {                             
        #region Constructor
        /// <summary>
        /// Default constructor for a MultiAlign analysis object.
        /// </summary>
        /// <param name="evntPercentComplete"></param>
        /// <param name="evntStatusMessage"></param>
        /// <param name="evntTitleMessage"></param>
        public MultiAlignAnalysis()
        {
            // Meta Data Information about the analysis and datasets.
            MetaData                        = new AnalysisMetaData();
            MetaData.AnalysisName           = string.Empty;
            Options                         = new AnalysisOptions();
            
            // Alignment options and data.
            AlignmentData                   = new List<classAlignmentData>();
            MatchResults                    = null;
        }
        #endregion       
	                      
        /// <summary>
        /// Dispose method that will kill the analysis thread.
        /// </summary>
        public void Dispose()
        {
            MetaData.Datasets.Clear();
        }

        #region Properties
        /// <summary>
        /// Gets or sets what kind of analysis to do.  
        /// </summary>
        public MultiAlignCore.Algorithms.AnalysisType AnalysisType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or est the analysis options.
        /// </summary>
        public AnalysisOptions Options
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the data providers to the underlying data cache.
        /// </summary>
        public FeatureDataAccessProviders DataProviders
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster alignment data.
        /// </summary>
        public classAlignmentData ClusterAlignmentData
        {
            get;
            set;
        }   
        /// <summary>
        /// Gets or sets the alignment data.
        /// </summary>
        public List<classAlignmentData> AlignmentData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the analysis meta-data.
        /// </summary>
        public AnalysisMetaData MetaData
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the peak matching results 
        /// </summary>
        public PeakMatchingResults<UMCClusterLight, MassTagLight> MatchResults
        {
            get;
            set;
        }        
        /// <summary>
        /// Gets or sets the mass tag database.
        /// </summary>
        public MassTagDatabase MassTagDatabase
        {
            get;
            set;
        }    
        #endregion
    }    
}
