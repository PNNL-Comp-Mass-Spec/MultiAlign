using System;
using System.Collections.Generic;
using System.Reflection;
using MultiAlignEngine;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.PeakMatching;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.IO;
using PNNLProteomics.SMART;

using PNNLProteomics.MultiAlign.Hibernate.Domain.DAO;

namespace PNNLProteomics.Data
{
	[Serializable()]
	public class MultiAlignAnalysis : IDisposable
    {
        /// <summary>
        /// Constant of the first level
        /// </summary>
        private const int CONST_FIRST_LEVEL = 0;
        
        #region Enumerations
        public enum enmState
        {
            IDLE, LOADING_FEATURES, DONE_LOADING_FEATURES, LOADING_MASSTAGS, LOADING_MASSTAGS_COMPLETE,
            ALIGNING, DONE_ALIGNING, CLUSTERING, DONE_CLUSTERING, PEAKMATCHING, PEAKMATCHING_DONE, DONE, ERROR
        } ;
        private enmState menmState;
        #endregion
                
        #region Members
        /// <summary>
        /// Flag indicating whether or not to calculate the SMART Scores.
        /// </summary>
        private bool mbool_calculateSTACScores;
        /// <summary>
		/// Datasets used in analysis.
		/// </summary>
		private List<DatasetInformation> m_datasetInformation; 
		/// <summary>
		/// Array of MultiAlignEngine.Alignment.clsAlignmentOptions for datasets used in analysis.
		/// </summary>
		private List<clsAlignmentOptions> marrDatasetAlignmentOptions ; 
		/// <summary>
		/// Array of MultiAlignEngine.Alignment.clsAlignmentFunction for datasets aligned in the analysis.
		/// </summary>
		private List<clsAlignmentFunction> marrDatasetAlignmentFunctions ; 		        
        /// <summary>
        /// String that holds the name of the baseline dataset.
        /// </summary>
        private string mstring_baselineDataset;
        /// <summary>
        /// String that holds the name of the analysis.
        /// </summary>
        private string mstring_analysisName = "";
        /// <summary>
        /// Flag whether the MTDB was used as the baseline.
        /// </summary>
        private bool mbool_useMassTagDBAsBaseline;
        /// <summary>
        /// Flag whether the MTDB was used for peak matching.
        /// </summary>
        private bool mbool_peakMatchedToMasstagDB;
        /// <summary>
        /// maintained list of alignment data.
        /// </summary>
        List<classAlignmentData>            mlist_alignmentData;
        /// <summary>
        /// Clustered alignment data, null if clusters are not aligned to a MTDB.
        /// </summary>
        classAlignmentData mobj_clusterAlignmentData;
        /// <summary>
        /// Object that holds how to find the UMC's
        /// </summary>
		private clsUMCFindingOptions        mobjUMCFindingOptions ;         
        /// <summary>
        /// Object that holds the options for performing an alignment.
        /// </summary>
		private clsAlignmentOptions         mobjAlignmentOptions ; 
        /// <summary>
        /// Object that holds the options for UMC clustering.
        /// </summary>
        private clsClusterOptions mobjClusteringOptions;
        /// <summary>
        /// Object that holds the options for peak matching.
        /// </summary>
        private clsPeakMatchingOptions mobjPeakMatchingOptions;        
        /// <summary>
        /// Object that holds the options for performing Mass Tag Database peak matching.
        /// </summary>
		private clsMassTagDatabaseOptions   mobjMassTagDBOptions; 
        /// <summary>
        /// Object that maintains the Mass Tags for a given MTDB.
        /// </summary>
		private clsMassTagDB                mobjMassTagDB; 
        /// <summary>
        /// Peak matching results object.
        /// </summary>
		private clsPeakMatchingResults      mobjPeakMatchingResults ;
        /// <summary>
        /// Peak matching results object with 11 Da shift.
        /// </summary>
        private clsPeakMatchingResults      mobj_shiftedPeakMatchingResults;
        /// <summary>
        /// Object that holds the smart scoring options.
        /// </summary>
        private classSMARTOptions mobj_stacOptions;
        /// <summary>
        /// List of smart results with summaries for each dataset.
        /// </summary>
        private classSMARTResults mobj_stacResults;
        [field: NonSerialized] 
        private string           mstring_pathname; 
		[field: NonSerialized] 
        private classTreeNode    mobj_treeRoot;        
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for a MultiAlign analysis object.
        /// </summary>
        /// <param name="evntPercentComplete"></param>
        /// <param name="evntStatusMessage"></param>
        /// <param name="evntTitleMessage"></param>
        public MultiAlignAnalysis()
        {
            mbool_useMassTagDBAsBaseline = false; 

			mobjAlignmentOptions            = new MultiAlignEngine.Alignment.clsAlignmentOptions() ; 
			mobjUMCFindingOptions           = new MultiAlignEngine.Features.clsUMCFindingOptions() ; 
			mobjClusteringOptions           = new MultiAlignEngine.Clustering.clsClusterOptions() ;
            mobjPeakMatchingOptions         = new clsPeakMatchingOptions();
			mobjMassTagDBOptions            = new MultiAlignEngine.MassTags.clsMassTagDatabaseOptions() ; 
			marrDatasetAlignmentOptions     = new List<clsAlignmentOptions>(); 

			menmState                       = enmState.IDLE ; 
			marrDatasetAlignmentFunctions   = new List<clsAlignmentFunction>(); 
			mbool_peakMatchedToMasstagDB    = false ; 
			mobj_treeRoot                   = null;
			mstring_analysisName            = string.Empty;
            mlist_alignmentData             = new List<classAlignmentData>();

            /// 
            /// Smart options for SMART Scoring and a collection to store the results.
            /// 
            mobj_stacOptions               = new classSMARTOptions();
            mobj_stacResults               = new classSMARTResults();
            m_datasetInformation            = new List<DatasetInformation>();
            /// 
            /// This only matters if peptide peak matching was performed
            /// 
            mbool_calculateSTACScores = false;
        }
        #endregion

        /// <summary>
        /// Dispose method that will kill the analysis thread.
        /// </summary>
		public void Dispose()
		{
            marrDatasetAlignmentOptions.Clear();            
            m_datasetInformation.Clear();            			
		}

        /// <summary>
        /// Gets or sets the data providers to the underlying data cache.
        /// </summary>
        public FeatureDataAccessProviders DataProviders
        {
            get;set;
        }

        #region Loading mass tag database
        /// <summary>
        /// Monitors the loading of the mass tag database for status messages.
        /// </summary>
        //private void MonitorMassTagDBLoading()
        //{
        //    try
        //    {
        //        // we only want to call the status message when a new message arrives, otherwise
        //        // there will be updates with the SAME message. So lets track last message sent.
        //        string lastMessage = "";
        //        while (menmState == enmState.LOADING_MASSTAGS)
        //        {
        //            string currentMessage = mobjMassTagDBLoader.StatusMessage;
        //            if (currentMessage == null)
        //                currentMessage = "";
        //            if (StatusMessage != null && currentMessage != lastMessage)
        //            {
        //                lastMessage = currentMessage;
        //                StatusMessage(0, currentMessage);
        //            }

        //            if (PercentComplete != null)
        //                PercentComplete((int)mobjMassTagDBLoader.PercentComplete);

        //            System.Threading.Thread.Sleep(200);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + ex.StackTrace);
        //    }
        //}
        ///// <summary>
        ///// Loads the mass tag database.
        ///// </summary>
        //public void LoadMassTagDB()
        //{
        //    menmState = enmState.LOADING_MASSTAGS;            
        //    /// 
        //    /// Make sure the path is not to an XAMT database
        //    /// 
        //    if (mobjMassTagDBOptions.menm_databaseType == MassTagDatabaseType.ACCESS &&
        //                System.IO.Path.GetExtension(mobjMassTagDBOptions.mstr_databaseFilePath) == ".txt")
        //    {
        //        XAMTReader reader = new XAMTReader();
        //        mobjMassTagDB = reader.ReadXAMTDatabase(mobjMassTagDBOptions.mstr_databaseFilePath);
        //    }
        //    else
        //    {
        //        Thread procThread = new Thread(new ThreadStart(MonitorMassTagDBLoading));
        //        mthread_currentStatus = procThread;
        //        procThread.Name = "Loading Mass Tag Database Thread Monitor";

        //        mobjMassTagDBLoader = new MultiAlignEngine.MassTags.clsMTDBLoader(mobjMassTagDBOptions);
        //        procThread.Start();

        //        mobjMassTagDB = mobjMassTagDBLoader.LoadMassTagDatabase();
        //        StatusMessage(0, mobjMassTagDBLoader.StatusMessage);
        //    }            
        //    menmState = enmState.LOADING_MASSTAGS_COMPLETE;
        //}
        #endregion
	                      
		/// <summary>
		/// Default alignment options are the same for every dataset, which is mobjAlingmentOptions
		/// Copies of this are made an inserted into the array of alignment options
		/// </summary>
        [Obsolete]
		public  void  SetDefaultAlignmentOptions()
		{
			marrDatasetAlignmentOptions.Clear() ;                          
			for (int fileNum = 0 ; fileNum < Datasets.Count ; fileNum++)
			{
				MultiAlignEngine.Alignment.clsAlignmentOptions alignmentOptions = mobjAlignmentOptions ; 
				marrDatasetAlignmentOptions.Add(alignmentOptions) ;
			}
		}
        #region Properties
        /// <summary>
        /// Gets or sets the cluster alignment data.
        /// </summary>
        public classAlignmentData ClusterAlignmentData
        {
            get { return mobj_clusterAlignmentData; }
            set { mobj_clusterAlignmentData = value; }
        }
        /// <summary>
        /// Gets the lower bound false discovery rate calculated by 11 Dalton Shift.
        /// </summary>
        public double FDRLowerBound
        {
            get
            {
                if (mobjPeakMatchingResults == null)
                    return double.NaN;
                if (mobj_shiftedPeakMatchingResults == null)
                    return double.NaN;

                double matches      = Convert.ToDouble(mobjPeakMatchingResults.NumMassTagsMatched);
                double shiftMatches = Convert.ToDouble(mobj_shiftedPeakMatchingResults.NumMassTagsMatched);

                return shiftMatches / (matches + shiftMatches);
            }
        }
        /// <summary>
        /// Gets the upper bound false discovery rate calculated by 11 Dalton Shift.
        /// </summary>
        public double FDRUpperBound
        {
            get
            {
                if (mobjPeakMatchingResults == null)
                    return double.NaN;
                if (mobj_shiftedPeakMatchingResults == null)
                    return double.NaN;

                double matches      = Convert.ToDouble(mobjPeakMatchingResults.NumMassTagsMatched);
                double shiftMatches = Convert.ToDouble(mobj_shiftedPeakMatchingResults.NumMassTagsMatched);

                return (2*shiftMatches) / (matches + shiftMatches);
            }
        }
        /// <summary>
        /// Gets or sets the SMART results calculated.
        /// </summary>
        public classSMARTResults STACTResults
        {
            get
            {
                return mobj_stacResults;
            }
            set
            {
                mobj_stacResults = value;
            }
        }
        /// <summary>
        /// Gets or sets the SMART Options to use.
        /// </summary>
        public classSMARTOptions SMARTOptions
        {
            get
            {
                return mobj_stacOptions;
            }
            set
            {
                mobj_stacOptions = value;
            }
        }
        /// <summary>
        /// Gets or sets the alignment data.
        /// </summary>
        public List<classAlignmentData> AlignmentData
        {
            get
            {
                return mlist_alignmentData;
            }
            set
            {
                mlist_alignmentData = value;
            }
        }
		/// <summary>
		/// Get/Set the analysis as a hiearchy of datasets with related factor information for grouping.
		/// </summary>
		public classTreeNode FactorTreeNode
		{
			get
			{
				return mobj_treeRoot;
			}
			set
			{
				mobj_treeRoot = value;
			}
		}
        /// <summary>
        /// Gets or sets the list of files associated with this analysis.
        /// </summary>
		public List<DatasetInformation> Datasets
		{
			get
			{                
				return m_datasetInformation ; 
			}
			set
			{
				m_datasetInformation = value ; 
			}
		}
        /// <summary>
        /// Gets or sets the name of the baseline dataset.
        /// </summary>
		[clsDataSummaryAttribute("Baseline Dataset")]
		public string BaselineDatasetName
		{
			get
			{
				return mstring_baselineDataset ;
			}
			set
			{
				mstring_baselineDataset = value ; 
			}
		}	
        /// <summary>
        /// Gets or sets the Alignment Options.
        /// </summary>
		public List<clsAlignmentOptions> AlignmentOptions
		{
			get
			{
				return marrDatasetAlignmentOptions ; 
			}
			set
			{
				marrDatasetAlignmentOptions = value ; 
			}
		}
        /// <summary>
        /// Gets or sets the mass tag database options.
        /// </summary>
		public MultiAlignEngine.MassTags.clsMassTagDatabaseOptions MassTagDBOptions
		{
			get
			{
				return mobjMassTagDBOptions ; 
			}
			set
			{
				mobjMassTagDBOptions = value ; 
			}
		}
        /// <summary>
        /// Gets or sets whether to use the mass tag database as the baseline dataset.
        /// </summary>
		[clsDataSummaryAttribute("Use MTDB As Baseline")]		
		public bool UseMassTagDBAsBaseline
		{
			get
			{
				return mbool_useMassTagDBAsBaseline ; 
			}
			set
			{
				mbool_useMassTagDBAsBaseline = value ; 
			}
		}
        /// <summary>
        /// Gets or sets the UMC Finding Options.
        /// </summary>
		public MultiAlignEngine.Features.clsUMCFindingOptions UMCFindingOptions
		{
			get
			{
				return mobjUMCFindingOptions ;
			}
			set
			{
				mobjUMCFindingOptions = value ;
			}
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public MultiAlignEngine.Clustering.clsClusterOptions ClusterOptions
        {
            get
            {
                return mobjClusteringOptions;
            }
            set
            {
                mobjClusteringOptions = value;
            }
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public MultiAlignEngine.PeakMatching.clsPeakMatchingOptions PeakMatchingOptions
        {
            get
            {
                return mobjPeakMatchingOptions;
            }
            set
            {
                mobjPeakMatchingOptions = value;
            }
        }
        /// <summary>
        /// Gets the peak matching results 
        /// </summary>
		public MultiAlignEngine.PeakMatching.clsPeakMatchingResults PeakMatchingResults
		{
			get
			{
				return mobjPeakMatchingResults ; 
			}
            set
            {
                mobjPeakMatchingResults = value;
            }
        }
        /// <summary>
        /// Gets the peak matching results from the 11 Da shift.
        /// </summary>
        public MultiAlignEngine.PeakMatching.clsPeakMatchingResults PeakMatchingResultsShifted
        {
            get
            {
                return mobj_shiftedPeakMatchingResults;
            }
            set
            {
                mobj_shiftedPeakMatchingResults = value;
            }
        }	
        /// <summary>
        /// Gets the flag whether the results were peaked matched against the Mass Tag Database.
        /// </summary>
		[clsDataSummaryAttribute("Peaks Matched to MTDB")]		
		public bool PeakMatchedToMassTagDB
		{
			get
			{
				return mbool_peakMatchedToMasstagDB ; 
			}
		}
        public clsMassTagDB MassTagDatabase
        {
            get
            {
                return mobjMassTagDB;
            }
            set
            {
                mobjMassTagDB = value;
            }
        }

        [clsDataSummaryAttribute("Default Alignment Options")]
        public MultiAlignEngine.Alignment.clsAlignmentOptions DefaultAlignmentOptions
        {
            get
            {
                return mobjAlignmentOptions;
            }
            set
            {
                mobjAlignmentOptions = value;
            }
        }
		#endregion

        #region Analysis File 
        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        [clsDataSummaryAttribute("Analysis Name")]
        public string AnalysisName
        {
            get
            {
                return mstring_analysisName;
            }
            set
            {
                mstring_analysisName = value;
            }
        }
        /// <summary>
        /// Gets or sets the pathname associated with the analysis.
        /// </summary>
        [clsDataSummaryAttribute("Analysis Path")]
        public string AnalysisPath
        {
            get
            {
                return mstring_pathname;
            }
            set
            {
                mstring_pathname = value;
            }
        }
        #endregion

        #region Parameter Files Options Reflection
        #endregion        
    }
}
