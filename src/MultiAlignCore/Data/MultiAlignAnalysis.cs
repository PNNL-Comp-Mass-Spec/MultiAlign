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
using PNNLProteomics.SMART;
using MultiAlignCore.Algorithms.PeakMatching;
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

            // UMC Finding options
			UMCFindingOptions               = new UMCFeatureFinderOptions();
            
            // Alignment options and data.
            AlignmentData                   = new List<classAlignmentData>();
            AlignmentOptions                = new List<clsAlignmentOptions>(); 
			DefaultAlignmentOptions         = new MultiAlignEngine.Alignment.clsAlignmentOptions() ; 
			DriftTimeAlignmentOptions       = new DriftTimeAlignmentOptions();
            UseMassTagDBAsBaseline          = false; 

            // Clustering Options
			ClusterOptions                  = new MultiAlignEngine.Clustering.clsClusterOptions() ;

            // Peak Matching and MTDB
            PeakMatchingOptions             = new clsPeakMatchingOptions();
			MassTagDBOptions                = new MultiAlignEngine.MassTags.clsMassTagDatabaseOptions() ; 
			PeakMatchedToMassTagDB          = false; 
            
            // STAC Options and results
            STACOptions                     = new STACOptions();
            STACResults                     = new classSMARTResults();
            MSLinkerOptions                 = new Algorithms.MSLinker.MSLinkerOptions();
            FeatureFilterOptions            = new FeatureFilterOptions();
        }
        #endregion

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
        /// Dispose method that will kill the analysis thread.
        /// </summary>
        public void Dispose()
        {
            AlignmentOptions.Clear();
            MetaData.Datasets.Clear();
        }

        #region Properties   
        /// <summary>
        /// Gets or sets the filter criteria for loading features.
        /// </summary>
        public FeatureFilterOptions FeatureFilterOptions
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
        /// Gets or sets the SMART results calculated.
        /// </summary>
        public classSMARTResults STACResults
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the SMART Options to use.
        /// </summary>
        public STACOptions STACOptions
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
        /// Gets or sets the name of the baseline dataset.
        /// </summary>
        [clsDataSummaryAttribute("Baseline Dataset")]
        public string BaselineDatasetName
        {
            get;
            set;
        }	
        /// <summary>
        /// Gets or sets the Alignment Options.
        /// </summary>
		public List<clsAlignmentOptions> AlignmentOptions
		{
			get;
            set;
		}
        /// <summary>
        /// Gets or sets the options for linking MS features to MSMS Spectra.
        /// </summary>
        public MultiAlignCore.Algorithms.MSLinker.MSLinkerOptions MSLinkerOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mass tag database options.
        /// </summary>
        public MultiAlignEngine.MassTags.clsMassTagDatabaseOptions MassTagDBOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets whether to use the mass tag database as the baseline dataset.
        /// </summary>
        [clsDataSummaryAttribute("Use MTDB As Baseline")]
        public bool UseMassTagDBAsBaseline
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the UMC Finding Options.
        /// </summary>
        public UMCFeatureFinderOptions UMCFindingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public clsClusterOptions ClusterOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cluster options.
        /// </summary>
        public clsPeakMatchingOptions PeakMatchingOptions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the peak matching results 
        /// </summary>
        public List<FeatureMatchLight<UMCClusterLight, MassTagLight>> PeakMatchingResults
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the peak matching results from the 11 Da shift.
        /// </summary>
        public List<FeatureMatchLight<UMCClusterLight, MassTagLight>> ShiftedPeakMatchingResults
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the flag whether the results were peaked matched against the Mass Tag Database.
        /// </summary>
        [clsDataSummaryAttribute("Peaks Matched to MTDB")]
        public bool PeakMatchedToMassTagDB
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets or sets the mass tag database.
        /// </summary>
        public MassTagDatabase MassTagDatabase
        {
            get;
            set;
        }
        [clsDataSummaryAttribute("Default Alignment Options")]
        public clsAlignmentOptions DefaultAlignmentOptions
        {
            get;
            set;
        }
        [clsDataSummaryAttribute("Drift Time Options")]
        public DriftTimeAlignmentOptions DriftTimeAlignmentOptions
        {
            get;
            set;
        }       
        #endregion
    }    
}
