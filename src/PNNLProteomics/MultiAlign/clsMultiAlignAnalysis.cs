using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;

using System.Xml;
using MultiAlignEngine;
using MultiAlignEngine.MassTags;
using MultiAlignEngine.Features;
using MultiAlignEngine.Alignment;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.PeakMatching;

using PNNLProteomics.IO;
using PNNLProteomics.SMART;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Factors;
using PNNLProteomics.Data.Alignment;
using PNNLProteomics.MultiAlign;
using PNNLProteomics.MultiAlign.Hibernate;
using PNNLProteomics.MultiAlign.Hibernate.Domain.DAOHibernate;


namespace PNNLProteomics.Data.Analysis
{
	/// <summary>
	/// The class is the class where all the analysis steps get called from and tracked. 
	/// This includes loading of the files, loading of mass tag databases, alignments of the files,
	/// clustering of files, peak matching etc. Each of these steps are of course done through their processors
	/// (or mobjUMCCreator for loading of UMCs or mobjMassTagDBLoader in case of mass tag databases).
	/// Message passing note. Because of the issues related to passing messages
	/// back from unmanaged classes (i.e. not possible!), we will create monitoring threads, 
	/// whose goal will be to call status messages on processing classes and pass this back to 
	/// whoever passed in the events for monitoring through the IMessageClass
	/// </summary>
	[Serializable()]
	public class clsMultiAlignAnalysis : IMessageClass, IDisposable
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

        #region Delegates and Events
        /// <summary>
        /// Fired when an isotope peak is loaded for a file.
        /// </summary>
        [field: NonSerialized]
        public event DelegateIsotopePeaksLoadedForFile IsotopePeaksLoadedForFile;
        public delegate void DelegateIsotopePeaksLoadedForFile(string fileName, clsIsotopePeak[] isotopePeaks);
        /// <summary>
        /// Fired when an isotope peak is loaded for a file.
        /// </summary>
        [field: NonSerialized]
        public event DelegateUMCSLoadedForFile UMCSLoadedForFile;
        public delegate void DelegateUMCSLoadedForFile(string fileName, clsUMC[] umcs);
        /// <summary>
        /// Fired when a UMC is calculated for a file.
        /// </summary>
        [field: NonSerialized] public event DelegateUMCsCalculatedForFile UMCsCalculatedForFile;
        public delegate void   DelegateUMCsCalculatedForFile(string fileName,  clsUMC[] umcs);
        /// <summary>
        /// Event fired when a dataset is aligned.
        /// </summary>        
        [field: NonSerialized] public event DelegateDatasetAligned DatasetAligned;                
        /// <summary>
        /// Delegate definition for when a dataset is aligned.
        /// </summary>
        /// <param name="alignmentFnc">Alignment function for this part of the alignment.</param>
        /// <param name="fileName">Dataset path</param>
        /// <param name="mScores">Heat scores</param>
        /// <param name="minX">Min. X - scan baseline</param>
        /// <param name="maxX">Max. X - scan baseline</param>
        /// <param name="minY">Min. Y - scan alignee</param>
        /// <param name="maxY">Max. Y - scan alignee</param>
        /// <param name="part">If > -1, The alignment was split over a given m/z range., if -1, then alignment was performed over full m/z range.</param>
        public delegate void    DelegateDatasetAligned(clsAlignmentFunction alignmentFnc, 
                                                        string fileName,
                                                        ref float[,] mScores,
                                                        float minX,
                                                        float maxX,
                                                        float minY,            
                                                        float maxY,
                                                        int   part);
        /// <summary>
        /// Fired when progress has changed during an analysis.
        /// </summary>
        [field: NonSerialized] public static event     DelegateProgressChangedEventHandler ProgressChanged;
        public delegate void DelegateProgressChangedEventHandler(object o, int progress, string message);
        /// <summary>
        /// Fired when the analysis has completed.
        /// </summary>
        [field: NonSerialized] public event            DelegateAnalysisComplete AnalysisComplete;
        public delegate void DelegateAnalysisComplete(object sender);
        /// <summary>
        /// Current step the analysis is performing.
        /// </summary>
        [field: NonSerialized] public event DelegateCurrentStep CurrentStep;
        public delegate void DelegateCurrentStep(int index, string step);
        /// <summary>
        /// Fires and sends the list of steps required for performing the analysis.
        /// </summary>
        [field: NonSerialized] public event DelegateListOfSteps ListOfSteps;
        public delegate void DelegateListOfSteps(object sender, List<string> steps);
        /// <summary>
        /// Fired when a percentage of the analysis is complete.
        /// </summary>
        [field: NonSerialized] 
        public event PNNLProteomics.EventModel.DelegateSetPercentComplete PercentComplete;
        /// <summary>
        /// Fired when status can be displayed.
        /// </summary>
        [field: NonSerialized]
        public event PNNLProteomics.EventModel.DelegateSetStatusMessage StatusMessage;
        /// <summary>
        /// Fired for a title as a category of the analysis.
        /// </summary>
        [field: NonSerialized]
        public event PNNLProteomics.EventModel.DelegateSetStatusMessage TitleMessage;
        #endregion                
                
        #region Members
        #region Serialized Members  
        /// <summary>
        /// Flag indicating whether or not to calculate the SMART Scores.
        /// </summary>
        private bool mbool_calculateSMARTScores;
        /// <summary>
		/// Array of MultiAlignEngine.clsDatasetInfo for datasets used in analysis.
		/// </summary>
		private ArrayList marrFiles ; 
		/// <summary>
		/// Array of MultiAlignEngine.Alignment.clsAlignmentOptions for datasets used in analysis.
		/// </summary>
		private List<clsAlignmentOptions> marrDatasetAlignmentOptions ; 
		/// <summary>
		/// Array of MultiAlignEngine.Alignment.clsAlignmentFunction for datasets aligned in the analysis.
		/// </summary>
		private List<clsAlignmentFunction> marrDatasetAlignmentFunctions ; 		
        /// <summary>
        /// This should be removed in the long run and replaced with the ArrayList of clsDatasetInfo.  
        /// </summary>
        private List<string> marrFileNames;
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
        /// A managed object that holds all of the UMC data
        /// </summary>
		private clsUMCData                  mobjUMCData ; 
        /// <summary>
        /// Object that holds how to find the UMC's
        /// </summary>
		private clsUMCFindingOptions        mobjUMCFindingOptions ; 
        /// <summary>
        /// UMC Creator object that finds UMC's for a given dataset loaded from Decon2ls output.
        /// </summary>
		private clsUMCCreator               mobjUMCCreator ; 
        /// <summary>
        /// Object that performs the alignment.
        /// </summary>
		private clsAlignmentProcessor       mobjAlignmentProcessor ; 
        /// <summary>
        /// Object that holds the options for performing an alignment.
        /// </summary>
		private clsAlignmentOptions         mobjAlignmentOptions ; 
        /// <summary>
        /// Object that performs UMC clustring for peak matching.
        /// </summary>
        private clsClusterProcessor mobjClusterProcessor;
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
        /// Object that loads the MTDB for AMT alignment and peak matching.
        /// </summary>
		private clsMTDBLoader               mobjMassTagDBLoader; 
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
        /// Object that processes the MTDB and UMC's to perform peak matching.
        /// </summary>
		private clsPeakMatchingProcessor    mobjPeakMatcher ;
        /// <summary>
        /// Object that holds the smart scoring options.
        /// </summary>
        private classSMARTOptions mobj_smartOptions;
        /// <summary>
        /// List of smart results with summaries for each dataset.
        /// </summary>
        private classSMARTResults mobj_smartResults;
        #endregion

        #region NonSerialized Members
        [field: NonSerialized] 
        private string           mstring_pathname;
        [field: NonSerialized]
        private Thread           mthread_currentStatus; 
		[field: NonSerialized] 
        private classTreeNode    mobj_treeRoot;
        [field: NonSerialized] 
        private classFactorTree  mobj_factorTree = new classFactorTree();
        /// <summary>
        /// Flag indicating whether an analysis is currently running.
        /// </summary>
        [field: NonSerialized] private bool mbool_processing;        
        /// <summary>
        /// Thread in charge of performing the analysis.
        /// </summary>
        [field: NonSerialized] private Thread mobj_analysisThread; 
        /// <summary>
        /// Integer variable that tells what level of the total of the analysis is being performed.
        /// </summary>
        [field: NonSerialized] private int mint_statusLevel;        
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for a MultiAlign analysis object.
        /// </summary>
        /// <param name="evntPercentComplete"></param>
        /// <param name="evntStatusMessage"></param>
        /// <param name="evntTitleMessage"></param>
        public clsMultiAlignAnalysis(DelegateSetPercentComplete evntPercentComplete, DelegateSetStatusMessage evntStatusMessage, DelegateSetStatusMessage evntTitleMessage)
		{			
			PercentComplete = evntPercentComplete ; 
			StatusMessage   = evntStatusMessage ; 
			TitleMessage    = evntTitleMessage ; 

			mobjAlignmentOptions         = new MultiAlignEngine.Alignment.clsAlignmentOptions() ; 
			mobjAlignmentProcessor       = new MultiAlignEngine.Alignment.clsAlignmentProcessor() ; 
			mobjUMCCreator               = new MultiAlignEngine.Features.clsUMCCreator() ;
			mobjUMCFindingOptions        = new MultiAlignEngine.Features.clsUMCFindingOptions() ; 
			mobjUMCData                  = new MultiAlignEngine.Features.clsUMCData() ; 
			mobjClusterProcessor         = new MultiAlignEngine.Clustering.clsClusterProcessor() ; 
			mobjClusteringOptions        = new MultiAlignEngine.Clustering.clsClusterOptions() ;
            mobjPeakMatchingOptions         = new clsPeakMatchingOptions();
			mobjMassTagDBOptions         = new MultiAlignEngine.MassTags.clsMassTagDatabaseOptions() ; 
			mobjPeakMatcher              = new MultiAlignEngine.PeakMatching.clsPeakMatchingProcessor() ; 

			mbool_useMassTagDBAsBaseline    = false ; 
			menmState                       = enmState.IDLE ; 
			marrDatasetAlignmentFunctions   = new List<clsAlignmentFunction>(); 
			marrDatasetAlignmentOptions     = new List<clsAlignmentOptions>(); 
			mbool_peakMatchedToMasstagDB    = false ; 
			mobj_treeRoot                   = null;
			mstring_analysisName            = string.Empty;
            mlist_alignmentData             = new List<classAlignmentData>();

            /// 
            /// Smart options for SMART Scoring and a collection to store the results.
            /// 
            mobj_smartOptions               = new classSMARTOptions();
            mobj_smartResults               = new classSMARTResults();

            /// 
            /// This only matters if peptide peak matching was performed
            /// 
            mbool_calculateSMARTScores = false;
        }
        #endregion

        #region Disposing/Abort/Destruction
        ~clsMultiAlignAnalysis()
		{
			if (mthread_currentStatus != null)
			{
				if (mthread_currentStatus.IsAlive)
					mthread_currentStatus.Abort() ; 
				mthread_currentStatus = null ; 
			}

            AbortAnalysisThread();
		}
        /// <summary>
        /// Dispose method that will kill the analysis thread.
        /// </summary>
		public void Dispose()
		{
			if (mthread_currentStatus != null)
			{
				if (mthread_currentStatus.IsAlive)
					mthread_currentStatus.Abort() ; 
				mthread_currentStatus = null ; 
			}

            marrDatasetAlignmentOptions.Clear();
            marrFileNames.Clear();
            marrFiles.Clear();
            this.FactorTree.ClearFactors();
            this.UMCData.Clear();

            
			// because this class needs a destructor, it also needs this
			// idiomatic Dispose method
			System.GC.SuppressFinalize(this);
            
            /// 
            /// Force a collction
            /// 
            System.GC.Collect();			
		}
        /// <summary>
        /// Aborts the analysis thread.
        /// </summary>
		public void Abort()
		{
            /// 
            /// Kill any current thread statuses
            /// 
			if (mthread_currentStatus != null)
			{
				if (mthread_currentStatus.IsAlive)
					mthread_currentStatus.Abort() ; 
				mthread_currentStatus = null ; 
			}

            /// 
            /// Kill any persisting analysis objects.
            /// 
            AbortAnalysisThread();
        }
        #endregion

        #region Loading mass tag database
        /// <summary>
        /// Monitors the loading of the mass tag database for status messages.
        /// </summary>
        private void MonitorMassTagDBLoading()
        {
            try
            {
                // we only want to call the status message when a new message arrives, otherwise
                // there will be updates with the SAME message. So lets track last message sent.
                string lastMessage = "";
                while (menmState == enmState.LOADING_MASSTAGS)
                {
                    string currentMessage = mobjMassTagDBLoader.StatusMessage;
                    if (currentMessage == null)
                        currentMessage = "";
                    if (StatusMessage != null && currentMessage != lastMessage)
                    {
                        lastMessage = currentMessage;
                        StatusMessage(0, currentMessage);
                    }

                    if (PercentComplete != null)
                        PercentComplete((int)mobjMassTagDBLoader.PercentComplete);

                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// Loads the mass tag database.
        /// </summary>
        public void LoadMassTagDB()
        {
            menmState = enmState.LOADING_MASSTAGS;            
            /// 
            /// Make sure the path is not to an XAMT database
            /// 
            if (mobjMassTagDBOptions.menm_databaseType == MassTagDatabaseType.ACCESS &&
                        System.IO.Path.GetExtension(mobjMassTagDBOptions.mstr_databaseFilePath) == ".txt")
            {
                XAMTReader reader = new XAMTReader();
                mobjMassTagDB = reader.ReadXAMTDatabase(mobjMassTagDBOptions.mstr_databaseFilePath);
            }
            else
            {
                Thread procThread = new Thread(new ThreadStart(MonitorMassTagDBLoading));
                mthread_currentStatus = procThread;
                procThread.Name = "Loading Mass Tag Database Thread Monitor";

                mobjMassTagDBLoader = new MultiAlignEngine.MassTags.clsMTDBLoader(mobjMassTagDBOptions);
                procThread.Start();

                mobjMassTagDB = mobjMassTagDBLoader.LoadMassTagDatabase();
                StatusMessage(0, mobjMassTagDBLoader.StatusMessage);
            }            
            menmState = enmState.LOADING_MASSTAGS_COMPLETE;
        }
        #endregion
		
		#region Loading of UMC files
		private void MonitorLoading()
		{
			try
			{
				/// 
                /// We only want to call the status message when a new message arrives, otherwise
				/// there will be updates with the SAME message. So lets track last message sent.
                /// 
				string lastMessage = ""; 
				while(menmState == enmState.LOADING_FEATURES)
				{
					string currentMessage = mobjUMCCreator.StatusMessage; 
					
                    if (currentMessage == null)
						currentMessage = ""; 

					if (StatusMessage != null && currentMessage != lastMessage)
					{
						lastMessage = currentMessage; 
						StatusMessage(mint_statusLevel, currentMessage); 
					}

					if (PercentComplete!= null)					
                        PercentComplete((int)mobjUMCCreator.PercentComplete); 

					Thread.Sleep(200) ; 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace) ; 
			}
		}
        /// <summary>
        /// Load the data from the files.  
        /// </summary>
		public void LoadData()
		{
			try
			{
				menmState                        = enmState.LOADING_FEATURES ; 
				Thread procThread                = new  Thread(new System.Threading.ThreadStart(MonitorLoading));
				mthread_currentStatus            = procThread ; 
				procThread.Name                  = "Loading Thread Monitor";
                mobjUMCCreator.UMCFindingOptions = mobjUMCFindingOptions;
				procThread.Start() ;

                int highestChargeState = 0;
				clsUMC[] loadedUMCs = null;

				for (int fileNum = 0 ; fileNum < FileNames.Length ; fileNum++)
				{
					bool preDefinedUMCs = false;
					mobjUMCCreator.FileName = FileNames[fileNum] ;
                    if (StatusMessage != null)
                    {
                        StatusMessage(mint_statusLevel, "Loading " + FileNames[fileNum]);
                    }

                    mint_statusLevel++;
					string extension = Path.GetExtension(FileNames[fileNum]).ToUpper();

					// If we are using a UMC or Feature file
					if (extension == ".TXT")
					{
						// Grabs UMCs from UMC or Feature file
						UmcReader umcReader = new UmcReader(FileNames[fileNum]);
						loadedUMCs = umcReader.GetUmcList().ToArray();
						preDefinedUMCs = true;

                        /// 
                        /// At this point we dont know what the NET really is. 
                        /// Instead, we are forced to use the mid point of the scans found in.  
                        /// The UMC Creator finds this NET value as the most intense scan of the MS Features.
                        ///  
                        int minScan = int.MaxValue;
                        int maxScan = int.MinValue;

                        foreach (clsUMC umc in loadedUMCs)
                        {
                            //umc.mdouble_abundance = umc.mdouble_max_abundance;
                            minScan = Math.Min(umc.mint_start_scan, minScan);
                            maxScan = Math.Max(umc.mint_end_scan, maxScan);                            
                        }
                        foreach (clsUMC umc in loadedUMCs)
                        {
                            umc.Net = Convert.ToDouble(umc.mint_scan - minScan) / Convert.ToDouble(maxScan - minScan);                            
                        }
                        
                        if (UMCSLoadedForFile != null)
                        {
                            UMCSLoadedForFile(FileNames[fileNum], loadedUMCs);
                        }
					}
					
					// SQLite DB
					else if (extension == ".DB3" || extension == ".SQLITE")
					{
						NHibernateUtil.SetDbLocationForRead(FileNames[fileNum]);

						try
						{
							StatusMessage(mint_statusLevel, "Trying to load pre-made UMCs.");
							UmcDAOHibernate umcDAOHibernate = new UmcDAOHibernate();
							loadedUMCs = umcDAOHibernate.FindAll().ToArray();
							preDefinedUMCs = true;
						}
						// IF UMC table does not exist
						catch (NHibernate.ADOException adoException)
						{
							StatusMessage(mint_statusLevel, "No UMCs found. Creating UMCs from provided data.");
							loadedUMCs = new clsUMC[0];
						}

						// If no UMCs were loaded from the SQLite DB, then we need to create UMCs using MSFeature data from the DB
						if (loadedUMCs.Length < 1)
						{
							MSFeatureDAOHibernate msFeatureDAOHibernate = new MSFeatureDAOHibernate();
							clsIsotopePeak[] msFeatureArray = msFeatureDAOHibernate.FindAll().ToArray();

							mobjUMCCreator.SetIsotopePeaks(ref msFeatureArray);
							mobjUMCCreator.FindUMCs();
							loadedUMCs = mobjUMCCreator.GetUMCs();

                            
						}
					}

					// Else we are using a PEK or CSV file
					else
					{
						if (extension == ".PEK")
						{
							mobjUMCCreator.LoadUMCs(true);
						}
						else if (extension == ".CSV")
						{
							mobjUMCCreator.LoadUMCs(false);
						}
						else
						{
							throw new ArgumentException("Incorrect extension for file. Please use pek or csv files as inputs.");
						}

						if (IsotopePeaksLoadedForFile != null)
						{
							IsotopePeaksLoadedForFile(FileNames[fileNum], mobjUMCCreator.GetIsotopePeaks());
						}

						/// 
						/// Finds and calculates all UMCS
						/// 
						mobjUMCCreator.FindUMCs();
						loadedUMCs = mobjUMCCreator.GetUMCs();

					}

                    /// 
                    /// Find the highest Charge State
                    /// 
					if (preDefinedUMCs)
					{
						foreach (clsUMC umc in loadedUMCs)
						{
							if (umc.ChargeMax != 0)
							{
								if (umc.ChargeMax > highestChargeState)
								{
									highestChargeState = umc.ChargeMax;
								}
							}
							else
							{
								if (umc.ChargeRepresentative > highestChargeState)
								{
									highestChargeState = umc.ChargeRepresentative;
								}
							}
						}
					}
					else
					{
						highestChargeState = Math.Max(highestChargeState, mobjUMCCreator.HighestChargeState);
					}

                    /// 
                    /// Notify listeners that we have loaded UMCS's for this dataset.
                    /// 
					if (UMCsCalculatedForFile != null)					
						UMCsCalculatedForFile(FileNames[fileNum], loadedUMCs) ; 
					
                    /// 
                    /// Set the UMC Object with more umcs
                    /// 
					mobjUMCData.SetUMCS(FileNames[fileNum], ref loadedUMCs);                 
                    mint_statusLevel--;
				}

                ///
                /// Maintain the highest charge state was
                ///  
                mobjUMCData.HighestChargeState  = highestChargeState;
				menmState                       = enmState.DONE_LOADING_FEATURES ; 
			}
			catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
				menmState = enmState.ERROR;
			}
		}
		#endregion 

        #region Alignment
        public void  MonitorAlignments()
        {
            try
            {
                // we only want to call the status message when a new message arrives, otherwise
                // there will be updates with the SAME message. So lets track last message sent.
                string lastMessage = "";
                while (menmState == enmState.ALIGNING)
                {
                    string currentMessage = mobjAlignmentProcessor.StatusMessage;
                    if (currentMessage == null)
                        currentMessage = "";
                    if (StatusMessage != null && currentMessage != lastMessage)
                    {
                        lastMessage = currentMessage;
                        StatusMessage(mint_statusLevel, currentMessage);
                    }
                    if (PercentComplete != null)
                        PercentComplete((int)mobjAlignmentProcessor.PercentComplete);
                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
		/// <summary>
		/// Default alignment options are the same for every dataset, which is mobjAlingmentOptions
		/// Copies of this are made an inserted into the array of alignment options
		/// </summary>
		public  void  SetDefaultAlignmentOptions()
		{
			marrDatasetAlignmentOptions.Clear() ;                          
			for (int fileNum = 0 ; fileNum < marrFileNames.Count ; fileNum++)
			{
				MultiAlignEngine.Alignment.clsAlignmentOptions alignmentOptions = mobjAlignmentOptions ; 
				marrDatasetAlignmentOptions.Add(alignmentOptions) ;
			}
		}
        /// <summary>
        /// Aligns the dataset to the baseline reference features.  
        /// </summary>
        /// <param name="datasetIndex">Dataset to align to the baseline.</param>
		public  void  AlignDataset(int datasetIndex)
		{
			menmState                               = enmState.ALIGNING ; 
			clsAlignmentOptions alignmentOptions    = marrDatasetAlignmentOptions[datasetIndex] as clsAlignmentOptions; 			
            mobjAlignmentProcessor.AlignmentOptions = alignmentOptions ; 

			int minScanRef = 0;
            int maxScanRef = 0;

            /// 
            /// Add space for storing the alignment functions, then store it.
            /// We add space because we skip the baseline dataset alignment since
            /// it would be a perfect R = 1.0 alignment value.
            /// 
            while (marrDatasetAlignmentFunctions.Count < datasetIndex + 1)
            {
                marrDatasetAlignmentFunctions.Add(null);
                mlist_alignmentData.Add(null);
            }


            /// 
            /// If the baseline is a MTDB then make sure its loaded.
            /// 
			if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
			{
				if (!mobjAlignmentProcessor.MassTagDBLoaded)
					mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjMassTagDB) ;                 
			}
			else
			{
                /// 
                /// Otherwise, set the baseline dataset features.
                /// 
				int baselineIndex = mobjUMCData.GetDatasetIndex(alignmentOptions.AlignmentBaselineName) ; 
				if (baselineIndex == datasetIndex)
					return ; 

				mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjUMCData, baselineIndex) ; 
				mobjUMCData.GetMinMaxScan(baselineIndex, ref minScanRef, ref maxScanRef) ;
			}

            /// 
            /// Tell the listeners that we are aligning X to Y.
            /// 
			string aligneeDataset = (string) marrFileNames[datasetIndex] ; 
			if (StatusMessage != null)
			    StatusMessage(mint_statusLevel, "Aligning " + aligneeDataset + " to " + alignmentOptions.AlignmentBaselineName) ;
            mint_statusLevel++;

            /// 
            /// Determine if we need to split the data or not before aligning
            ///     See ticket #126 on exactive mass calibration code.
            /// 
            int totalBoundaries = 1;            
            if (alignmentOptions.SplitAlignmentInMZ == true)
            {
			    totalBoundaries = 2;
            }

            /// 
            /// For the exactive fix, we have a given set of boundaries.  Load only the features for the given
            /// boundary ranges here.
            /// 
            List<clsAlignmentFunction> alignmentFunctions     = new List<clsAlignmentFunction>();
            List<double[,]> netErrorHistograms                = new List<double[,]>();
            List<double[,]> massErrorHistograms               = new List<double[,]>();
            List<classAlignmentData> alignmentData            = new List<classAlignmentData>();
            List<float[,]> heatScores                         = new List<float[,]>();
            List<float[]> xIntervals                          = new List<float[]>();
            List<float[]> yIntervals                          = new List<float[]>();

            /// 
            /// Then get the reference NET RANGE values?
            /// 
            float minMTDBNET    = 0.0F;
            float maxMTDBNET    = 1.0F;
            mobjAlignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);

            int minScanBaseline = int.MaxValue;
            int maxScanBaseline = int.MinValue;

            int alignmentPart = -1;            
            for (int i = 0; i < totalBoundaries; i++)
            {
                if (StatusMessage != null)
                {
                    int step = i + 1;
                    StatusMessage(mint_statusLevel, "Aligning part " + step.ToString() + " of " + totalBoundaries.ToString());
                }

                /// 
                /// Set the baseline features for only the ones of interest
                /// 
                mobjAlignmentProcessor.SetAligneeDatasetFeatures(mobjUMCData,
                                                                datasetIndex,
                                                                mobjAlignmentOptions.MZBoundaries[i]);



                /// 
                /// Then perform the alignment.
                /// 
                mobjAlignmentProcessor.PerformAlignmentToMSFeatures();
                
                /// 
                /// Grab the alignment function
                /// 
                clsAlignmentFunction alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction();
        
                ///
                /// BLL marrDatasetAlignmentFunctions[datasetIndex] = alignmentFunction;
                ///
                alignmentFunctions.Add(alignmentFunction);

                /// 
                /// Then apply the net and mass function to the UMC data.
                /// 
                mobjAlignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref mobjUMCData);

                /// 
                /// Find the minimum scan used in reference between alignee and baseline dataset
                /// 
                int tempMinScanBaseline = 0;
                int tempMaxScanBaseline = 0;
                mobjUMCData.GetMinMaxScan(datasetIndex, ref tempMinScanBaseline, ref tempMaxScanBaseline);
                minScanBaseline         = Math.Min(minScanBaseline, tempMinScanBaseline);
                maxScanBaseline         = Math.Max(maxScanBaseline, tempMaxScanBaseline);

                /// 
                /// Pull out the heat maps...
                /// 
                float[,] heatScore = new float[1, 1];
                float[] xInterval = new float[1];
                float[] yInterval = new float[1];
                mobjAlignmentProcessor.GetAlignmentHeatMap(ref heatScore, ref xInterval, ref yInterval);

                xIntervals.Add(xInterval);
                yIntervals.Add(yInterval);
                heatScores.Add(heatScore);

                /// 
                /// 7-22-2009
                /// Mass and net error histograms!  
                /// 
                double[,] massErrorHistogram  = new double[1, 1];
                double[,] netErrorHistogram   = new double[1, 1];

                //clsAlignmentOptions alignmentOptions = alignmentOptions[i] as clsAlignmentOptions;
                mobjAlignmentProcessor.GetErrorHistograms(  alignmentOptions.MassBinSize,
                                                            alignmentOptions.NETBinSize,
                                                            ref massErrorHistogram,
                                                            ref netErrorHistogram);
                massErrorHistograms.Add(massErrorHistogram);
                netErrorHistograms.Add(netErrorHistogram);

                /// 
                /// Get the residual data from the warp.
                /// 
                float [,] linearNet              = new float[1,1];
                float [,] customNet              = new float[1,1];
                float [,] linearCustomNet        = new float[1,1];
                float [,] massError              = new float[1, 1];
                float [,] massErrorCorrected     = new float[1, 1];
                float [,] mzMassError            = new float[1, 1];
                float [,] mzMassErrorCorrected   = new float[1, 1];

                classAlignmentResidualData residualData = mobjAlignmentProcessor.GetResidualData();      
                /*s(linearNet,
                                                     customNet,
                                                     linearCustomNet,
                                                     massError,
                                                     massErrorCorrected,
                                                     mzMassError,
                                                     mzMassErrorCorrected); 
               */
                
                
                /// 
                /// Set all of the data now 
                /// 
                classAlignmentData data = new classAlignmentData();
                data.massErrorHistogram = massErrorHistogram;
                data.netErrorHistogram  = netErrorHistogram;
                data.aligneeDataset     = aligneeDataset;
                data.alignmentFunction  = alignmentFunction;
                data.heatScores         = heatScore;
                data.minScanBaseline    = minScanBaseline;
                data.maxScanBaseline    = maxScanBaseline;
                data.NETIntercept       = mobjAlignmentProcessor.NETIntercept;
                data.NETRsquared        = mobjAlignmentProcessor.NETLinearRSquared;
                data.NETSlope           = mobjAlignmentProcessor.NETSlope;
                data.ResidualData               = residualData;
                data.MassMean                   = mobjAlignmentProcessor.GetMassMean();
                data.MassStandardDeviation      = mobjAlignmentProcessor.GetMassStandardDeviation();
                data.NETMean                    = mobjAlignmentProcessor.GetNETMean();
                data.NETStandardDeviation       = mobjAlignmentProcessor.GetNETStandardDeviation();

                /// 
                /// Find out the max scan or NET value to use for the range depending on what 
                /// type of baseline dataset it was (MTDB or dataset).
                /// 
                if (alignmentOptions.IsAlignmentBaselineAMasstagDB == true)
                {
                    data.minMTDBNET = minMTDBNET;
                    data.maxMTDBNET = maxMTDBNET;
                }
                else
                {
                    data.minMTDBNET = minScanRef;
                    data.maxMTDBNET = maxScanRef;
                }

                alignmentData.Add(data);

                if (totalBoundaries > 1)
                    alignmentPart = i;                                

                /// 
                /// Fire the event to listeners that the alignment has been done for this dataset.
                /// 
                /// 
                if (DatasetAligned != null)
                {
                    if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
                    {
                        DatasetAligned( alignmentFunction,
                                        aligneeDataset,
                                        ref heatScore,
                                        (float)minScanBaseline,
                                        (float)maxScanBaseline,
                                        minMTDBNET,
                                        maxMTDBNET,
                                        alignmentPart);
                    }
                    else
                    {
                        DatasetAligned( alignmentFunction,
                                        aligneeDataset,
                                        ref heatScore,
                                        (float)minScanBaseline,
                                        (float)maxScanBaseline,
                                        (float)minScanRef,
                                        (float)maxScanRef,
                                        alignmentPart);
                    }
                }
            } 

            mint_statusLevel--;
            classAlignmentData   mergedData                 = new classAlignmentData();
            clsAlignmentFunction mergedAlignmentFunction    = new clsAlignmentFunction(enmCalibrationType.HYBRID_CALIB, enmAlignmentType.NET_MASS_WARP);
            float[,] mergedHeatScores                       = new float[1, 1];

            /// ////////////////////////////////////////////////////////////
            /// Merge the mass error histogram data.
            /// ////////////////////////////////////////////////////////////
            int maxMassHistogramLength = 0;
            int maxNetHistogramLength  = 0;
            for (int i = 0; i < alignmentData.Count; i++)
            {
                maxMassHistogramLength = Math.Max(maxMassHistogramLength, alignmentData[0].massErrorHistogram.GetLength(0));
                maxNetHistogramLength = Math.Max(maxNetHistogramLength, alignmentData[0].netErrorHistogram.GetLength(0));
            }

            double[,] massErrorHistogramData = new double[maxMassHistogramLength, 2];
            MergeHistogramData(massErrorHistogramData, alignmentData[0].massErrorHistogram, false);

            /// 
            /// The residual arrays are the same size, here we start the process to count the 
            /// size so we can merge all of the results back into one array.
            /// 
            int countMassResiduals = 0;
            int countNETResiduals  = 0;

            for (int i = 0; i < alignmentData.Count; i++)
            {
                if (i > 0)
                    MergeHistogramData(massErrorHistogramData, alignmentData[i].massErrorHistogram, true);

                countMassResiduals += alignmentData[i].ResidualData.mz.Length;
                countNETResiduals  += alignmentData[i].ResidualData.scans.Length;
            }
            
            /// //////////////////////////////////////////////////////////// 
            /// Merge:
            ///     NET error histogram data
            ///     Mass Residual Data            
            /// ////////////////////////////////////////////////////////////
            double[,] netErrorHistogramData = new double[maxNetHistogramLength, 2];            
            MergeHistogramData(netErrorHistogramData, alignmentData[0].netErrorHistogram, false);

            mergedData.ResidualData                         = new classAlignmentResidualData();
            mergedData.ResidualData.customNet               = new float[countNETResiduals];
            mergedData.ResidualData.linearCustomNet         = new float[countNETResiduals];
            mergedData.ResidualData.linearNet               = new float[countNETResiduals];
            mergedData.ResidualData.scans                   = new float[countNETResiduals];
            mergedData.ResidualData.massError               = new float[countMassResiduals];
            mergedData.ResidualData.massErrorCorrected      = new float[countMassResiduals];
            mergedData.ResidualData.mz                      = new float[countMassResiduals];
            mergedData.ResidualData.mzMassError             = new float[countMassResiduals];
            mergedData.ResidualData.mzMassErrorCorrected    = new float[countMassResiduals];
            

            int copyNETBlocks  = 0; 
            int copyMassBlocks = 0; 
                                       
            for (int i = 0; i < alignmentData.Count; i++)
            {
                /// 
                /// Merge the residual data
                /// 
                alignmentData[i].ResidualData.customNet.CopyTo(mergedData.ResidualData.customNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearCustomNet.CopyTo(mergedData.ResidualData.linearCustomNet, copyNETBlocks);
                alignmentData[i].ResidualData.linearNet.CopyTo(mergedData.ResidualData.linearNet, copyNETBlocks);
                alignmentData[i].ResidualData.scans.CopyTo(mergedData.ResidualData.scans, copyNETBlocks);    

                alignmentData[i].ResidualData.massError.CopyTo(mergedData.ResidualData.massError, copyMassBlocks);
                alignmentData[i].ResidualData.massErrorCorrected.CopyTo(mergedData.ResidualData.massErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassError.CopyTo(mergedData.ResidualData.mzMassError, copyMassBlocks);
                alignmentData[i].ResidualData.mzMassErrorCorrected.CopyTo(mergedData.ResidualData.mzMassErrorCorrected, copyMassBlocks);
                alignmentData[i].ResidualData.mz.CopyTo(mergedData.ResidualData.mz, copyMassBlocks);

                copyNETBlocks  += alignmentData[i].ResidualData.scans.Length;
                copyMassBlocks += alignmentData[i].ResidualData.mz.Length;

                mergedData.NETMean               = alignmentData[i].NETMean;
                mergedData.MassMean              = alignmentData[i].MassMean;
                mergedData.MassStandardDeviation = alignmentData[i].MassStandardDeviation;
                mergedData.NETStandardDeviation  = alignmentData[i].NETStandardDeviation;

                if (i > 0)                
                    MergeHistogramData(netErrorHistogramData, alignmentData[i].netErrorHistogram, true);                
            }

            /// 
            /// Grab the heat scores!
            /// 
            mergedData.heatScores               = alignmentData[alignmentData.Count - 1].heatScores;
            mergedData.massErrorHistogram       = massErrorHistogramData;
            mergedData.netErrorHistogram        = netErrorHistogramData;
            mlist_alignmentData[datasetIndex]   = mergedData;   
         
		}
        /// <summary>
        /// Merges the histogram data leaving the result in old.
        /// </summary>
        /// <param name="histogramOld">Data to retain merged data.</param>
        /// <param name="histogramNew">Data to copy.</param>
        /// <param name="checkClosestBin">Flag indicating whether to use the closest bin or to just assume that the x values match between dest and src.</param>
        private void MergeHistogramData(double[,] histogramDest, double[,] histogramSource, bool checkClosestBin)
        {                        
            for (int i = 0; i < histogramSource.GetLength(0) && i < histogramDest.GetLength(0); i++)
            {
                int     bestIndex   = 0;
                double  massDiff    = double.MaxValue;

                if (checkClosestBin == false)
                {
                    bestIndex = i;
                    histogramDest[i, 0] = histogramSource[bestIndex, 0];
                }
                else
                {
                    int length = Math.Min(histogramDest.GetLength(0), histogramSource.GetLength(0));
                    
                    /// 
                    /// Find the best mass item if the previous mass items are skewed or changed
                    /// 
                    for (int j = 0; j < length; j++)
                    {
                        double diff = Math.Abs(histogramDest[j, 0] - histogramSource[j, 0]);
                        if (diff < massDiff)
                        {
                            bestIndex = j;
                            massDiff  = diff;
                        }
                    }
                }                
                histogramDest[i, 1] += histogramSource[bestIndex, 1];
            }
        }
        /// <summary>
        /// Retrieves the alignment data for display.
        /// </summary>
        public bool GetAlignmentData(  int datasetIndex, 
                                        ref clsAlignmentFunction alignmentFunction,
                                        ref string aligneeDataset,
                                        ref float[,] heatScores,
                                        ref int minScanBaseline,
                                        ref int maxScanBaseline,
                                        ref float minMTDBNET,
                                        ref float maxMTDBNET)
        {
            clsAlignmentOptions alignmentOptions;            
            float[] xIntervals  = new float[1];
            float[] yIntervals  = new float[1];

            heatScores = new float[1, 1];

            /// 
            /// Get the alignment options
            /// 
            alignmentOptions                        = marrDatasetAlignmentOptions[datasetIndex] as clsAlignmentOptions;
            mobjAlignmentProcessor.AlignmentOptions = alignmentOptions;

            /// 
            /// Get the UMC Data
            /// 
            mobjUMCData.GetMinMaxScan(datasetIndex, ref minScanBaseline, ref maxScanBaseline);
            mobjAlignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals);

            minMTDBNET = 0.0F;
            maxMTDBNET = 1.0F;


            int min, max;
            min = 0;
            max = 1;

            /// 
            /// Test to see if it's a database or dataset baseline
            /// 
            if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
            {
                mobjAlignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET);
            }
            else
            {
                int baselineIndex = mobjUMCData.GetDatasetIndex(alignmentOptions.AlignmentBaselineName);
                if (baselineIndex == datasetIndex)
                    return false;
                mobjUMCData.GetMinMaxScan(baselineIndex, ref min, ref max);
                minMTDBNET = Convert.ToSingle(min);
                maxMTDBNET = Convert.ToSingle(max);
            }


            aligneeDataset    = (string)marrFileNames[datasetIndex];            
            alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction(); 

            return true;
        }
        /// <summary>
        /// Aligns all of the datasets.
        /// </summary>
		public  void  AlignDatasets()
		{
			menmState                           = enmState.ALIGNING ; 
			System.Threading.Thread procThread  = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorAlignments));
			mthread_currentStatus               = procThread ; 
			procThread.Name                     = "Alignment Thread Monitor";
			procThread.Start();

            for (int datasetNum = 0; datasetNum < marrFileNames.Count; datasetNum++)
			{
                try
                {
                    AlignDataset(datasetNum);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
			}
            menmState = enmState.DONE_ALIGNING; 
		}
        /// <summary>
        /// Align the UMC clusters found after an alignment and clustering used for peak matching.
        /// </summary>
		private void  AlignClustersToMassTagDB() 
		{
			try
			{
				menmState = enmState.ALIGNING ; 
				System.Threading.Thread procThread = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorAlignments));
				mthread_currentStatus = procThread ; 
				procThread.Name = "Alignment Thread Monitor";
				procThread.Start() ; 

				if (StatusMessage != null)
				{
					StatusMessage(0, "Aligning Clusters to Mass Tag Database") ; 
				}

				mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjMassTagDB) ; 
				mobjAlignmentProcessor.SetAligneeDatasetFeatures(   mobjUMCData.mobjClusterData,
                                                                    mobjAlignmentOptions.MZBoundaries[0]);  

				mobjAlignmentProcessor.PerformAlignmentToMassTagDatabase() ; 
				mobjAlignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref mobjUMCData.mobjClusterData) ;  
				MultiAlignEngine.Alignment.clsAlignmentFunction alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction() ; 

				float [,] heatScores = new float[1,1] ; 
				float [] xIntervals  = new float[1] ; 
				float [] yIntervals  = new float[1] ; 
				mobjAlignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals) ;
                                

				float minClusterNET=0.0F , maxClusterNET = 1.0F ; 
				mobjUMCData.mobjClusterData.GetMinMaxNET(ref minClusterNET, ref maxClusterNET) ; 
				float minMTDBNET=0.0F , maxMTDBNET = 1.0F ; 
				mobjAlignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET) ;


                float[,] linearNet = new float[1, 1];
                float[,] customNet = new float[1, 1];
                float[,] linearCustomNet = new float[1, 1];
                float[,] massError = new float[1, 1];
                float[,] massErrorCorrected = new float[1, 1];
                float[,] mzMassError = new float[1, 1];
                float[,] mzMassErrorCorrected = new float[1, 1];

                classAlignmentResidualData residualData = mobjAlignmentProcessor.GetResidualData();    

                double[,] netErrorHistogram                = new double[1,1];
                double[,] massErrorHistogram               = new double[1,1];
                
                mobjAlignmentProcessor.GetErrorHistograms(  mobjAlignmentOptions.MassBinSize,
                                                            mobjAlignmentOptions.NETBinSize,
                                                            ref massErrorHistogram,
                                                            ref netErrorHistogram);

                mobj_clusterAlignmentData                    = new classAlignmentData();
                mobj_clusterAlignmentData.alignmentFunction  = alignmentFunction;
                mobj_clusterAlignmentData.heatScores         = heatScores;
                mobj_clusterAlignmentData.massErrorHistogram = massErrorHistogram;
                mobj_clusterAlignmentData.netErrorHistogram  = netErrorHistogram;
                mobj_clusterAlignmentData.NETIntercept       = mobjAlignmentProcessor.NETIntercept;
                mobj_clusterAlignmentData.NETRsquared        = mobjAlignmentProcessor.NETLinearRSquared;
                mobj_clusterAlignmentData.NETSlope           = mobjAlignmentProcessor.NETSlope;
                mobj_clusterAlignmentData.ResidualData          = residualData;
                mobj_clusterAlignmentData.MassMean              = mobjAlignmentProcessor.GetMassMean();
                mobj_clusterAlignmentData.MassStandardDeviation = mobjAlignmentProcessor.GetMassMean();
                mobj_clusterAlignmentData.NETMean               = mobjAlignmentProcessor.GetNETMean();
                mobj_clusterAlignmentData.NETStandardDeviation  = mobjAlignmentProcessor.GetNETStandardDeviation();


				if (DatasetAligned != null)
				{
					DatasetAligned(alignmentFunction, 
                                    "Feature (UMC) Clusters", 
                                    ref heatScores,
                                    minClusterNET,
                                    maxClusterNET, 
                                    minMTDBNET, 
                                    maxMTDBNET,
                                    -1) ; 
				}
				menmState = enmState.DONE_ALIGNING ; 
			}
			catch (System.Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace) ; 
				menmState = enmState.ERROR ; 
			}
		}
        #endregion

        #region Clustering
        private void MonitorClustering()
        {
            try
            {
                // we only want to call the status message when a new message arrives, otherwise
                // there will be updates with the SAME message. So lets track last message sent.
                string lastMessage = "";
                while (menmState == enmState.CLUSTERING)
                {
                    string currentMessage = mobjClusterProcessor.StatusMessage;
                    if (currentMessage == null)
                        currentMessage = "";
                    if (StatusMessage != null && currentMessage != lastMessage)
                    {
                        lastMessage = currentMessage;
                        StatusMessage(mint_statusLevel, currentMessage);
                    }
                    if (PercentComplete != null)
                        PercentComplete((int)mobjClusterProcessor.PercentComplete);
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
        /// <summary>
        /// For single datasets this will convert the umc's into a cluster.
        /// </summary>
        /// <param name="umcdata"></param>
        /// <param name="index"></param>
        private void ConstructClustersFromDataset(clsUMCData umcdata, int index)
        {
            clsUMC[] umcs = UMCData.GetUMCS(index);

            int i = 0;

            clsClusterData clusters             = new clsClusterData();
            clusters.marrClusterIntensity       = new double[umcs.Length];
            clusters.marrClusterMainMemberIndex = new int[umcs.Length];
            umcdata.mobjClusterData = clusters;
            umcdata.mobjClusterData.mintNumDatasets = 1;

            double minNET = double.MaxValue;
            double maxNET = double.MinValue;
            foreach (clsUMC umc in umcs)
            {
                clsCluster cluster      = new clsCluster();
               
                cluster.mdouble_mass_calibrated = umc.MassCalibrated;
                cluster.mdouble_mass            = umc.Mass;
                cluster.mdouble_net             = umc.Net;
                cluster.mshort_charge           = umc.mshort_class_rep_charge;
                cluster.mint_scan               = umc.ScanAligned;

                minNET = Math.Min(cluster.Net, minNET);
                maxNET = Math.Max(cluster.Net, maxNET);

                cluster.mshort_num_dataset_members      = 1;
                clusters.marrClusterMainMemberIndex[i]  = i;
                clusters.marrClusterIntensity[i]        = umc.AbundanceSum;
                cluster.mint_cluster_index              = i++;
                cluster.mdouble_driftTime               = umc.DriftTime;
                umcdata.mobjClusterData.AddCluster(cluster);                                          
            }
            clusters.mdblMinNET = minNET;
            clusters.mdblMaxNET = maxNET;    
        }
        public void PerformClustering()
        {
            menmState = enmState.CLUSTERING;
            System.Threading.Thread procThread = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorClustering));
            mthread_currentStatus = procThread;
            procThread.Name = "Clustering Thread Monitor";
            procThread.Start();

            mint_statusLevel++;

            mobjClusterProcessor.ClusterOptions = mobjClusteringOptions;
            if (StatusMessage != null)
            {
                StatusMessage(0, "Performing Clustering of data points");
            }
            if (marrFileNames.Count > 0)
            {
                mobjClusterProcessor.PerformClustering(mobjUMCData);
            }
            else
            {
                ConstructClustersFromDataset(mobjUMCData, 0);
            }

            mint_statusLevel--;
            // lets write it out.
            menmState = enmState.DONE_CLUSTERING;
        }
        #endregion

        #region Peak Matching
        private void MonitorPeakMatching()
        {
            try
            {
                // we only want to call the status message when a new message arrives, otherwise
                // there will be updates with the SAME message. So lets track last message sent.
                string lastMessage = "";
                while (menmState == enmState.PEAKMATCHING)
                {
                    string currentMessage = mobjPeakMatcher.StatusMessage;
                    if (currentMessage == null)
                        currentMessage = "";
                    if (StatusMessage != null && currentMessage != lastMessage)
                    {
                        lastMessage = currentMessage;
                        StatusMessage(0, currentMessage);
                    }
                    if (PercentComplete != null)
                        PercentComplete(mobjPeakMatcher.PercentComplete);
                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog elog = new System.Diagnostics.EventLog();
                elog.Log = "Application";
                elog.Source = "MultiAlign";
                elog.WriteEntry(ex.Message + ex.StackTrace);
                elog.Close();
                Console.WriteLine(ex.Message + ex.StackTrace);
                menmState = enmState.ERROR;
            }
        }        
        /// <summary>
        /// Performs the peak matching of UMC's to the MTDB and inherent scoring.
        /// </summary>
        public void PerformPeakMatching()
        {
            mint_statusLevel++;
            try
            {
                /// 
                /// Datasets are aligned to the database individually and then clustered.                
                /// These clusters could have variation in them still.  Perform a cluster alignment 
                /// to correct for centroidization.
                /// 
                /// Also, only do this alignment if we have more than one dataset.  This way
                /// we dont over align the problem.
                /// 
                if (mobjMassTagDBOptions.mstrDatabase != null && marrFileNames.Count > 1)
                {
                    if (mobjClusteringOptions.AlignClusters == true)
                    {

                        AlignClustersToMassTagDB();
                    }
                    else
                    {
                        /// 
                        /// Since we are not aligning then we will fake the cluster aligned data to be 
                        /// its mass, NET.
                        /// 
                        for (int i = 0; i < mobjUMCData.mobjClusterData.NumClusters; i++)
                        {
                            clsCluster cluster      = mobjUMCData.mobjClusterData.GetCluster(i);
                            cluster.NetAligned      = cluster.Net;
                            cluster.MassCalibrated  = cluster.Mass;
                        }
                    }
                }

                if (mbool_calculateSMARTScores == false)
                {

                    menmState                           = enmState.PEAKMATCHING;
                    System.Threading.Thread procThread  = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorPeakMatching));
                    mthread_currentStatus               = procThread;
                    procThread.Name                     = "Peak Matching Thread Monitor";
                    procThread.Start();

                    if (StatusMessage != null)
                    {
                        StatusMessage(0, "Performing Peak Matching");
                    }
                    
                    mobjPeakMatcher.MassTolerance       = mobjPeakMatchingOptions.MassTolerance;
                    mobjPeakMatcher.NETTolerance        = mobjPeakMatchingOptions.NETTolerance;
                    mobjPeakMatcher.DriftTimeTolerance  = mobjPeakMatchingOptions.DriftTimeTolerance; 
                    mobjPeakMatchingResults             = mobjPeakMatcher.PerformPeakMatching(mobjUMCData.mobjClusterData, mobjMassTagDB);
                    if (StatusMessage != null)
                    {
                        StatusMessage(0, "Shifting data by 11 Daltons to compute FDR");
                    }
                    mobj_shiftedPeakMatchingResults = mobjPeakMatcher.PerformPeakMatching(  mobjUMCData.mobjClusterData,
                                                                                            mobjMassTagDB,
                                                                                            11.0);                                                             
                }
                else
                {
                    CalculateSMARTScores();
                }
                mbool_peakMatchedToMasstagDB = true;

                menmState = enmState.PEAKMATCHING_DONE;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
                menmState = enmState.ERROR;
            }
            mint_statusLevel--;
        }
        #endregion 

        #region SMART - Scoring 
        /// <summary>
        /// Calculates the SMART Scores if matched to a AMTDB 
        /// for peptide identification.
        /// </summary>
        public void CalculateSMARTScores()
        {

            if (StatusMessage != null)
            {
                StatusMessage(0, "Performing STAC");
            }

            classSMARTProcessor processor = new classSMARTProcessor();
                        
            /// 
            /// Get all of the peptide information, find the
            /// count first so we dont
            /// make N number of native-managed calls.
            /// 
            List<classSMARTMassTag>  massTags   = 
                new List<classSMARTMassTag>();
            int totalTags                       = 
                mobjMassTagDB.GetMassTagCount();
            for(int i = 0; i < totalTags; i++)
            {
                clsMassTag tag                  = 
                    mobjMassTagDB.GetMassTagFromIndex(i);

                classSMARTMassTag msFeature     = new classSMARTMassTag();
                msFeature.mdouble_monoMass      = tag.mdblMonoMass;
                msFeature.mdouble_NET           = tag.NetAverage;
                msFeature.mint_ID               = tag.mintMassTagId;
                msFeature.mint_count            = tag.mintNumObsPassingFilter;
                
                msFeature.mdouble_probability   = tag.HighPeptideProphetProbability;
                massTags.Add(msFeature);                         
            }

            /// 
            /// For each dataset calculate the SMART results 
            /// 
            int             totalClusters   = 
                mobjUMCData.mobjClusterData.NumClusters;
            clsClusterData  clusters        = mobjUMCData.mobjClusterData;

            List<classSMARTUMC> smartFeatures = new List<classSMARTUMC>();
            for (int i = 0; i < totalClusters; i++)
            {
                clsCluster cluster = clusters.GetCluster(i);
                                       
                classSMARTUMC feature    = new classSMARTUMC();
                feature.mdouble_NET      = cluster.NetAligned;
                feature.mdouble_monoMass = cluster.MassCalibrated;
                feature.mint_id          = i;
                smartFeatures.Add(feature);
            }

            mobj_smartResults            = 
                processor.ScoreUMCMatches(massTags,
                                          smartFeatures,
                                          mobj_smartOptions);

            mobjPeakMatchingResults      = 
                ConvertSmartResultsToPeakResults(mobj_smartResults);            
        }
        /// <summary>
        /// Converts the SMART Results into peak matching results.
        /// </summary>
        /// <param name="smart">Results computed using SMART.</param>
        /// <returns>Peak matching results.</returns>
        private clsPeakMatchingResults 
            ConvertSmartResultsToPeakResults(classSMARTResults smart)
        {
            /// 
            /// Create a new results object for holding peak matching data.
            /// 
            clsPeakMatchingResults results = new clsPeakMatchingResults();

            /// 
            /// Then, look through the UMC cluster keys, 
            /// and pull out the clusters and MTID's from each.
            /// 
            int [] smartKeys = smart.GetUMCMatchIndices();
            foreach (int key in smartKeys)
            {
                /// 
                /// Get the list of all the UMC' data that matched to MTID's
                /// 
                List<classSMARTProbabilityResult> umcMatches = 
                    smart.GetResultFromUMCIndex(key);
                /// 
                /// Just in case, make sure we have result data here...
                /// 
                if (umcMatches != null)
                {
                    /// 
                    /// Then since we can have multiple MTID's 
                    /// match to UMC's,
                    /// pull out all possible probabilities and 
                    /// add to the result
                    /// matching data object to construct the 
                    /// protein to UMC - MTID relationships                    
                    /// 
                    /// First - we grab the cluster data, since 
                    /// it should be the same for all probability
                    /// matches
                    /// 
                    /// Second - we enumerate all the matches 
                    /// of umc cluster to mtid adding them
                    /// to the results object.
                    ///                     
                    clsCluster cluster  = 
                        mobjUMCData.mobjClusterData.GetCluster(key);
                    foreach (classSMARTProbabilityResult probability in
                        umcMatches)
                    {
                        clsMassTag tag      =
                            mobjMassTagDB.GetMassTag(probability.MassTagID);
                        results.AddPeakMatchResult(tag, cluster, key);
                    }
                }
            }
            /// 
            /// Finally, now that we have matches, we pull
            /// out all of the protein
            /// information that was retrieved and stored 
            /// in the mass tag database object.            
            /// 
            results.ExtractProteinInformation(mobjMassTagDB);

            /// 
            /// And last, we finally tie together all of 
            /// the information.
            ///     MTID - Protein - UMC Cluster Index.  
            /// This method will generate the triplet 
            /// structures that are used to key off of each other.
            /// 
            results.UpdatePeakMatchArrays();

            return results;
        }
        #endregion

        #region Analysis
        /// <summary>
        /// Aborts the analysis thread.
        /// </summary>
        private void AbortAnalysisThread()
        {
            try
            {
                mobj_analysisThread.Abort();
            }
            catch
            {
            }
            finally
            {
                mbool_processing = false;
                mobj_analysisThread = null;
            }
        }
        /// <summary>
        /// Starts a multi-Align analysis job.
        /// </summary>
        public void StartAnalysis(string saveFilePath)
        {            
            mstring_pathname     = saveFilePath;
            mstring_analysisName = Path.GetFileNameWithoutExtension(saveFilePath);            

            /// 
            /// Kill the thread in case it already exists.
            /// 
            AbortAnalysisThread();
            ThreadStart tStart  = new ThreadStart(PerformAnalysis);
            mobj_analysisThread = new Thread(tStart);
            mobj_analysisThread.Start();
        }


        /// <summary>
        /// Starts the main analysis.
        /// </summary>
        private void PerformAnalysis()
        {
            mbool_processing = true;

            try
            {
                string parameterPath = System.IO.Path.Combine(Path.GetDirectoryName(mstring_pathname), mstring_analysisName + "_parameters.xml");
                SaveParametersToFile(parameterPath);
            }
            catch{
            }
            string massTagDBName = MassTagDBOptions.mstrDatabase;

            /// /////////////////////////////////////////////////////////
            /// Create a list of steps to complete            
            ///     Here are the list of steps to perform
            ///         1.  Load MTDB*
            ///         2.  Load Data 
            ///         3.  Align
            ///         4.  Cluster
            ///         5.  Peak Match*
            ///         6.  Save Data to file.
            /// /////////////////////////////////////////////////////////
            List<string> listSteps = new List<string>();
            string stepLoadMTDB  = "Load MTDB";
            string stepLoadData  = "Load Datasets";
            string stepAlignment = "Align";
            string stepCluster   = "Cluster";
            string stepPeakMatch = "Peak Match";
            string stepSave      = "Save";
            listSteps.AddRange(new string[] { stepLoadData, stepAlignment, stepCluster });

            /// 
            /// Determine what steps to run
            /// 
            if (MassTagDBOptions.menm_databaseType != MultiAlignEngine.MassTags.MassTagDatabaseType.None)
            {
                listSteps.Add(stepPeakMatch);
            }
            if (massTagDBName != null && massTagDBName != "" || 
                MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
            {
                listSteps.Insert(0, stepLoadMTDB);
            }
            listSteps.Add(stepSave);

            /// 
            /// Tell the listener what we are going to do.
            /// 
            if (ListOfSteps != null)
                ListOfSteps(this, listSteps);

            
            /// ////////////////////////////////////////////// 
            /// Part One: 
            ///     Load Mass Tag Database 
            /// //////////////////////////////////////////////    
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (massTagDBName != null && massTagDBName != "" ||
                MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
            {
                if (CurrentStep != null)
                    CurrentStep(listSteps.IndexOf(stepLoadMTDB), stepLoadData);
                LoadMassTagDB();                
            }

            /// ////////////////////////////////////////////// 
            /// Part Two:
            ///     Loading Data
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (CurrentStep != null)
                CurrentStep(listSteps.IndexOf(stepLoadData), stepLoadData);
            LoadData();

            /// ////////////////////////////////////////////// 
            /// Part Three:
            ///     Setting alignment options
            /// //////////////////////////////////////////////             
            if (BaselineDataset != null)
            {
                DefaultAlignmentOptions.AlignmentBaselineName           = BaselineDataset;
                DefaultAlignmentOptions.IsAlignmentBaselineAMasstagDB   = false;
            }
            else
            {                
                DefaultAlignmentOptions.AlignmentBaselineName           = massTagDBName;
                DefaultAlignmentOptions.IsAlignmentBaselineAMasstagDB   = true;
                UseMassTagDBAsBaseline                                  = true;
            }
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (CurrentStep != null)
                CurrentStep(listSteps.IndexOf(stepAlignment), stepAlignment);
            SetDefaultAlignmentOptions();

            
            /// ////////////////////////////////////////////// 
            /// Part Four:
            ///     Alignment
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            AlignDatasets();


            /// ////////////////////////////////////////////// 
            /// Part Four:
            ///         Clustering 
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (CurrentStep != null)
                CurrentStep(listSteps.IndexOf(stepCluster), stepCluster);
            PerformClustering();

            /// ////////////////////////////////////////////// 
            /// Part Five:
            ///     Perform Peak Matching
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (MassTagDBOptions.menm_databaseType != MultiAlignEngine.MassTags.MassTagDatabaseType.None)
            {
                if (CurrentStep != null)
                    CurrentStep(listSteps.IndexOf(stepPeakMatch), stepPeakMatch);
                /// 
                /// Run the peak matching steps and scoring
                /// 
                PerformPeakMatching();
            }

            /// ////////////////////////////////////////////// 
            /// Part Six:
            ///     Serialize the analysis to file.
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (CurrentStep != null)
                CurrentStep(listSteps.IndexOf(stepSave), stepSave);

            try
            {
                /// 
                /// Get the path name and make a directory for the analysis.
                /// 
                string pathName     = System.IO.Path.GetDirectoryName(mstring_pathname);
                string newPath      = System.IO.Path.Combine(pathName, mstring_analysisName);
                System.IO.Directory.CreateDirectory(newPath);

                /// 
                /// Update the path name
                /// 
                mstring_pathname    = System.IO.Path.Combine(newPath, mstring_analysisName + ".mln");                   
                SerializeAnalysisToFile(mstring_pathname);
                WriteAlignmentDataToFile(newPath, 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Could not save results to file. " + ex.Message);
            }
            
            if (AnalysisComplete != null)
                AnalysisComplete(this);

            /// 
            /// Tell the user that processing is complete
            /// 
            mbool_processing = false;
        }
        #endregion

        private void WriteAlignmentDataToFile(string directoryName, int id)
        {
            string alignmentDataPath = mstring_analysisName + id.ToString() + "-alignmentData.csv";
            alignmentDataPath = System.IO.Path.Combine(directoryName, alignmentDataPath);


            using (TextWriter writer = File.CreateText(alignmentDataPath))
            {
                writer.WriteLine("Filename, Mass Mean, Mass Std, Mass Kurtosis, NET Mean, NET Std, NET Kurtosis");
                for(int i = 0; i < AlignmentData.Count; i++)
                {
                    classAlignmentData data = AlignmentData[i];
                    writer.Write("{0},", System.IO.Path.GetFileNameWithoutExtension(this.FileNames[i]));
                    if (data == null)
                    {
                        writer.WriteLine("Baseline dataset");
                    }
                    else
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                            data.MassMean,
                            data.MassStandardDeviation,
                            data.MassKurtosis,
                            data.NETMean,
                            data.NETStandardDeviation,
                            data.NETKurtosis);
                    }
                }
            }
        }

		#region Factors
		/// <summary>
		/// Builds the factor tree from the internal factor and dataset 
        /// information, and builds the tree node heirarchy associated with
		/// the dataset.
		/// </summary>
		/// <returns>Factor Tree Structure</returns>
		public classTreeNode BuildFactorTreeNode()
		{			
			if (mobj_factorTree == null)
                BuildFactorTree();

			return mobj_factorTree.BuildTree();
		}
		/// <summary>
		/// Builds the factor tree from the internal factor and dataset information.
		/// </summary>
		/// <returns>Factor tree</returns>
		public classFactorTree BuildFactorTree()
		{			
			if (mobj_factorTree == null)
				mobj_factorTree = new classFactorTree();

			mobj_factorTree.Data.Clear();
			mobj_factorTree.Factors.Clear();
			mobj_factorTree.ClearFactors();
			
			foreach(MultiAlignEngine.clsDatasetInfo info in  Files)
			{
				/// 
				/// Check to make sure we have factors first
				/// 
				if (info.factorsDefined == true)
				{					
					string datasetName = info.mstrDatasetName;				
					Hashtable factors  = new Hashtable();					
					for (int i =0 ; i < info.AssignedFactorValues.Count && i < info.Factors.Count; i++)
					{
						MultiAlignEngine.clsFactorInfo factorInfo = info.Factors[i] as clsFactorInfo;
						factors.Add(factorInfo.mstrFactor, info.AssignedFactorValues[i]);						
					}										
					mobj_factorTree.AddData(datasetName, factors);		// add to dataset. 
				}
			}
			MultiAlignEngine.clsDatasetInfo datasetInfo = Files[0] as MultiAlignEngine.clsDatasetInfo;
            if (datasetInfo != null && datasetInfo.Factors != null)
            {
                foreach (clsFactorInfo factInfo in datasetInfo.Factors)
                {
                    StringCollection factorNames = new StringCollection();
                    foreach (string values in factInfo.marrValues)
                    {
                        factorNames.Add(values);
                    }
                    mobj_factorTree.AddFactor(factInfo.mstrFactor, factorNames);
                }
            }
			return mobj_factorTree;
		}
		#endregion

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
        /// Gets the SMART results calculated.
        /// </summary>
        public classSMARTResults SMARTResults
        {
            get
            {
                return mobj_smartResults;
            }
        }
        /// <summary>
        /// Gets or sets the SMART Options to use.
        /// </summary>
        public classSMARTOptions SMARTOptions
        {
            get
            {
                return mobj_smartOptions;
            }
            set
            {
                mobj_smartOptions = value;
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
        /// Gets or sets whether an analysis is currently running.
        /// </summary>
        public bool Processing
        {
            get
            {
                return mbool_processing;
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
        /// Gets or sets the factor tree associated with the datasets.
        /// </summary>
		public classFactorTree FactorTree
		{
			get
			{
				return mobj_factorTree;
			}
			set
			{
				mobj_factorTree = value;

				if (value == null)
				{
					mobj_treeRoot = null;
					return;
				}
				// Build it to update the tree node structure so we have a fresh copy.
				mobj_treeRoot	= value.BuildTree();
			}
		}
        /// <summary>
        /// Gets or sets the list of files associated with this analysis.
        /// </summary>
		public System.Collections.ArrayList Files
		{
			get
			{
                //TODO: See what is currently stored in this array list, actual file references?
				return marrFiles ; 
			}
			set
			{
				marrFiles = value ; 
			}
		}
        /// <summary>
        /// Gets or sets the array of filenames associated with this analysis.
        /// </summary>
		public string [] FileNames 
		{
			get
			{
				return marrFileNames.ToArray();
			}
			set
			{
                marrFileNames = new List<string>(); 
				marrFileNames.InsertRange(0, value); 
			}
		}
        /// <summary>
        /// Gets or sets the name of the baseline dataset.
        /// </summary>
		[MultiAlignEngine.clsDataSummaryAttribute("Baseline Dataset")]
		public string BaselineDataset
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
		/// Gets or sets the default alignment options.
		/// </summary>
		public MultiAlignEngine.Alignment.clsAlignmentOptions DefaultAlignmentOptions
		{
			get
			{
				return mobjAlignmentOptions ; 
			}
			set
			{
				mobjAlignmentOptions = value ; 
			}
		}
        /// <summary>
        /// Gets or sets whether to use SMART Scores or not.
        /// </summary>
        [MultiAlignEngine.clsDataSummaryAttribute("Calculate SMART Scores")]
        public bool UseSMART
        {
            get
            {
                return mbool_calculateSMARTScores;
            }
            set
            {
                mbool_calculateSMARTScores = value;
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
		[MultiAlignEngine.clsDataSummaryAttribute("Use MTDB As Baseline")]		
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
		/// Gets the UMC Data feature options.
		/// </summary>
		public MultiAlignEngine.Features.clsUMCData UMCData
		{
			get
			{
				return mobjUMCData ; 
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
        }	
        /// <summary>
        /// Gets the flag whether the results were peaked matched against the Mass Tag Database.
        /// </summary>
		[MultiAlignEngine.clsDataSummaryAttribute("Peaks Matched to MTDB")]		
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
        }
		#endregion

        #region Analysis File 
        /// <summary>
        /// Handles the progress changed event from the analysis object file loader to display progress results.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="progress"></param>
        private static void progressStream_ProgressChanged(object o, int progress)
		{
			if (clsMultiAlignAnalysis.ProgressChanged != null)
			{
				clsMultiAlignAnalysis.ProgressChanged(o,progress, "loading analysis file...");
			}
        }       
        /// <summary>
        /// Gets or sets the name of the analysis.
        /// </summary>
        [MultiAlignEngine.clsDataSummaryAttribute("Analysis Name")]
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
        public string PathName
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
        /// <summary>
        /// Serializes the analysis to the path provided.
        /// </summary>
        /// <param name="fileName">Path to save the analysis to.</param>
        public void SerializeAnalysisToFile(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (mstring_analysisName == string.Empty)
                mstring_analysisName = fi.Name;

            // To serialize the hashtable and its key/value pairs,  
            // you must first open a stream for writing. 
            // In this case, use a file stream.
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, this);
                fs.Close();
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);                
            }
            finally
            {
                fs.Close();
                fs.Dispose();                
            }
        }
        /// <summary>
        /// Deserializes the binary file from the path provided into an analysis object.
        /// </summary>
        /// <param name="fileName">Path of the file to load the analysis from.</param>
        /// <returns> A new analysis object found in the file.</returns>
        public static clsMultiAlignAnalysis DeserializeAnalysisFromFile(string fileName)
        {
            
            clsMultiAlignAnalysis analysis = null;

            try
            {

                // Open the file containing the data that you want to deserialize.
                System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
                try
                {
                    if (fs == null)
                        throw new ArgumentNullException("analysis stream");

                    clsReadProgressStream progressStream = new clsReadProgressStream(fs);
                    progressStream.ProgressChanged      += new clsReadProgressStream.ProgressChangedEventHandler(progressStream_ProgressChanged);

                    const int defaultBufferSize = 4096;
                    int onePercentSize = (int)Math.Ceiling(progressStream.Length / 100.0);

                    BufferedStream bs = new BufferedStream(progressStream,
                            onePercentSize > defaultBufferSize ? defaultBufferSize : onePercentSize);
                    BinaryFormatter formatter = new BinaryFormatter();
                    analysis = formatter.Deserialize(bs) as clsMultiAlignAnalysis;
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
                finally
                {
                    fs.Close();
                }
            }
            catch 
            {
                ////System.Windows.Forms.MessageBox.Show("MultiAlign could not read the file.");                
            }

            if (analysis != null)
            {
                if (analysis.AnalysisName == null)
                    analysis.AnalysisName = string.Empty;                
                analysis.PathName = fileName;
                if (analysis.AlignmentData == null)
                    analysis.AlignmentData = new List<classAlignmentData>();
            }
            return analysis;
        }
        
        #endregion

        #region Parameter Options Reflection
        /// <summary>
        /// Reflects the object o for parameter options to load.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="node"></param>
        private void ReflectParameterOptions(object o, MetaNode node)
		{		
			if (o != null)
			{			
				/// 
				/// Iterate all the properties
				/// 
				foreach(PropertyInfo prop in o.GetType().GetProperties())
				{					
					// Recurse to get parameters.
					if (prop.CanRead)
					{																						
						object[] customAttributes = prop.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
						object potential = null;
						if (customAttributes.Length > 0)
							potential = prop.GetValue(o,
								BindingFlags.GetProperty,
								null,
								null,
								null);
						for (int i = 0; i < customAttributes.Length; i++)
						{
                            
							clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;
                            
							if (potential != null && attr != null && attr.Description != "")
							{										
								try
								{
                                    
									node.SetValue(attr.Description, potential);
								}
								catch(Exception ex)
								{
                                    System.Diagnostics.Debug.WriteLine(ex.Message);									
								}
							}
						}						
					}
				}					
				
				/// 
				/// Iterate all the fields
				/// 
				foreach(FieldInfo field in o.GetType().GetFields())
				{					
					object[] customAttributes	= field.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
					object objectValue = null;
					if (customAttributes.Length > 0)
						objectValue	   = field.GetValue(o);											
					for (int i = 0; i < customAttributes.Length; i++)
					{
						clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute ;
						if (objectValue != null && attr != null)
						{		
							try
							{
								node.SetValue(attr.Description, objectValue);
							}
							catch
							{
								//System.Windows.Forms.MessageBox.Show("Could not save " + attr.Description + " parameter. " + ex.Message);
							}
						}
					}					
				}	
			}
		}
		private void LoadParameterOptions(object o,  MetaNode node)
		{		
			if (o != null)
			{			
				foreach(PropertyInfo prop in o.GetType().GetProperties())
				{					
					// Recurse to get parameters.
					if (prop.CanWrite)
					{																						
						object[] customAttributes = prop.GetCustomAttributes(typeof(clsParameterFileAttribute),true);						
						for (int i = 0; i < customAttributes.Length; i++)
						{
							clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute;
							if (attr != null && attr.Description != "")
							{										
								try
								{
									object val;
									val = node.GetValue(attr.Description);
									prop.SetValue(o, val, BindingFlags.SetProperty, null, null, null);
								}
								catch
								{
									//System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
								}
							}
						}						
					}
				}	
				foreach(FieldInfo field in o.GetType().GetFields())
				{					
					object[] customAttributes	= field.GetCustomAttributes(typeof(clsParameterFileAttribute),true);
					for (int i = 0; i < customAttributes.Length; i++)
					{
						clsParameterFileAttribute attr = customAttributes[i] as clsParameterFileAttribute ;
						if (attr != null)
						{		
							try
							{
								object val = node.GetValue(attr.Description);
								if (val != null)
								{
									field.SetValue(o, val);
								}
							}
							catch
							{
								//System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
							}
						}
					}					
				}	
			}
        }
        #endregion

        #region Parameter File
        /// <summary>
		/// Saves parameters to file specified in XML format
		/// </summary>
		/// <param name="filename">Parameter file name to save parameters to</param>
		public void SaveParametersToFile(string filename)
		{
			// Extract parameters for now.			
			MetaData metaData = new MetaData("PNNLProteomics");
            ReflectParameterOptions(UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));
            ReflectParameterOptions(DefaultAlignmentOptions, metaData.OpenChild("DefaultAlignmentOptions"));
			ReflectParameterOptions(MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
            ReflectParameterOptions(ClusterOptions, metaData.OpenChild("ClusterOptions"));
            ReflectParameterOptions(PeakMatchingOptions, metaData.OpenChild("PeakMatchingOptions"));
            metaData.WriteFile(filename);
		}
		/// <summary>
		/// Loads parameters from file specified in XML format
		/// </summary>
		/// <param name="filename">Parameter file to load parameters</param>
		public void LoadParametersFromFile(string filename)
		{
			MetaData metaData = new MetaData("PNNLProteomics");
			metaData.ReadFile(filename);
						
			//ReflectParameterOptions(AlignmentOptions[0], metaData.OpenChild("AlignmentOptions"));
            LoadParameterOptions(AlignmentOptions, metaData.OpenChild("AlignmentOptions"));
            LoadParameterOptions(ClusterOptions, metaData.OpenChild("ClusterOptions"));
            LoadParameterOptions(PeakMatchingOptions, metaData.OpenChild("PeakMatchingOptions"));
            
            List<classAlignmentMZBoundary> boundaries = DefaultAlignmentOptions.MZBoundaries;
			LoadParameterOptions(DefaultAlignmentOptions, metaData.OpenChild("DefaultAlignmentOptions"));
            DefaultAlignmentOptions.MZBoundaries = boundaries;

			LoadParameterOptions(MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
			LoadParameterOptions(UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));
        }
        #endregion        
    }
}
