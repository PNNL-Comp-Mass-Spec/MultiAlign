using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Options;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO.Features;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using System;
using System.Collections.Generic;

namespace MultiAlignCore.Data
{
	[Serializable()]
	public class MultiAlignAnalysis :  IDisposable
    {                             
        #region Constructor
        /// <summary>
        /// Default constructor for a MultiAlign analysis object.
        /// </summary>
        public MultiAlignAnalysis()
        {
            // Meta Data Information about the analysis and datasets.
            MetaData                        = new AnalysisMetaData {AnalysisName = string.Empty};
            Options                         = new MultiAlignAnalysisOptions();
            MassTagDatabase                 = new MassTagDatabase();
            
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
        /// Objects that access data from the databases.
        /// </summary>
        public FeatureDataAccessProviders DataProviders { get; set; }   
        /// <summary>
        /// Gets or sets the list of data providers.
        /// </summary>
        public List<UMCClusterLight> Clusters
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets what kind of analysis to do.  
        /// </summary>
        public AnalysisType AnalysisType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or est the analysis options.
        /// </summary>
        public MultiAlignAnalysisOptions Options
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
