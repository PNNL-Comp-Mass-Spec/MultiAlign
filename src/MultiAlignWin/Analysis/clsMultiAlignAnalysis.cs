using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;

using System.Xml;
using MultiAlign;
using PNNLControls;
using MultiAlign.Features;
using MultiAlign.Alignment;

using MultiAlignWin.Data.Alignment;

namespace MultiAlignWin
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
        [field: NonSerialized] public event DelegateIsotopePeaksLoadedForFile IsotopePeaksLoadedForFile;
        public delegate void DelegateIsotopePeaksLoadedForFile(string fileName, clsIsotopePeak[] isotopePeaks);
        /// <summary>
        /// Fired when a UMC is calculated for a file.
        /// </summary>
        [field: NonSerialized] public event DelegateUMCsCalculatedForFile UMCsCalculatedForFile;
        public delegate void   DelegateUMCsCalculatedForFile(string fileName,  clsUMC[] umcs);
        /// <summary>
        /// Event fired when a dataset is aligned.
        /// </summary>
        [field: NonSerialized] public event DelegateDatasetAligned DatasetAligned;                
        public delegate void    DelegateDatasetAligned(clsAlignmentFunction alignmentFnc, 
                                                        string fileName,
                                                        ref float[,] mScores,
                                                        float minX,
                                                        float maxX,
                                                        float minY,            
                                                        float maxY);
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
        [field: NonSerialized] public event MultiAlignWin.DelegateSetPercentComplete PercentComplete;
        /// <summary>
        /// Fired when status can be displayed.
        /// </summary>
        [field: NonSerialized] public event MultiAlignWin.DelegateSetStatusMessage StatusMessage;
        /// <summary>
        /// Fired for a title as a category of the analysis.
        /// </summary>
        [field: NonSerialized] public event MultiAlignWin.DelegateSetStatusMessage TitleMessage;
        #endregion                
        
        #region Members
        #region Serialized Members
        /// <summary>
		/// Array of MultiAlign.clsDatasetInfo for datasets used in analysis.
		/// </summary>
		private System.Collections.ArrayList marrFiles ; 
		/// <summary>
		/// Array of MultiAlign.Alignment.clsAlignmentOptions for datasets used in analysis.
		/// </summary>
		private System.Collections.ArrayList marrDatasetAlignmentOptions ; 
		/// <summary>
		/// Array of MultiAlign.Alignment.clsAlignmentFunction for datasets aligned in the analysis.
		/// </summary>
		private System.Collections.ArrayList marrDatasetAlignmentFunctions ; 
		// this should be removed in the long run and replaced with the ArrayList of clsDatasetInfo. 
		private System.Collections.ArrayList marrFileNames ; 
		private MultiAlign.Features.clsUMCData mobjUMCData ; 
		private MultiAlign.Features.clsUMCFindingOptions mobjUMCFindingOptions ; 
		private MultiAlign.Features.clsUMCCreator mobjUMCCreator ; 
		private MultiAlign.Alignment.clsAlignmentProcessor mobjAlignmentProcessor ; 
		private MultiAlign.Alignment.clsAlignmentOptions mobjAlignmentOptions ; 
		private MultiAlign.Clustering.clsClusterProcessor mobjClusterProcessor ; 
		private MultiAlign.Clustering.clsClusterOptions mobjClusteringOptions ; 
		private MultiAlign.MassTags.clsMassTagDatabaseOptions mobjMassTagDBOptions ; 
		private MultiAlign.MassTags.clsMTDBLoader mobjMassTagDBLoader ; 
		private MultiAlign.MassTags.clsMassTagDB mobjMassTagDB ; 
		private MultiAlign.PeakMatching.clsPeakMatchingResults mobjPeakMatchingResults ; 
		private MultiAlign.PeakMatching.clsPeakMatchingProcessor mobjPeakMatcher ; 
		private bool    mblnUseMassTagDBAsBaseline ; 
		private string  mstrBaselineDataset ; 
		private bool    mblnPeakMatchedToMasstagDB ;
		private string  mstrAnalysisName = "";
        /// <summary>
        /// maintained list of alignment data.
        /// </summary>
        List<clsAlignmentData> mlist_alignmentData;
        #endregion

        #region NonSerialized Members
        [field: NonSerialized] private string mstrPathname;
		[field: NonSerialized] private System.Threading.Thread mthreadCurrentStatus ; 
		[field: NonSerialized] private PNNLControls.clsTreeNode mobj_treeRoot;
		[field: NonSerialized] private PNNLControls.clsFactorTree mobj_factorTree = new PNNLControls.clsFactorTree();
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
        /// <summary>
        /// File path where to save the analysis object when an analysis is finished.
        /// </summary>
        [field: NonSerialized] private string mstring_saveFileName;     
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

			mobjAlignmentOptions = new MultiAlign.Alignment.clsAlignmentOptions() ; 
			mobjAlignmentProcessor = new MultiAlign.Alignment.clsAlignmentProcessor() ; 
			mobjUMCCreator = new MultiAlign.Features.clsUMCCreator() ;
			mobjUMCFindingOptions = new MultiAlign.Features.clsUMCFindingOptions() ; 
			mobjUMCData = new MultiAlign.Features.clsUMCData() ; 
			mobjClusterProcessor = new MultiAlign.Clustering.clsClusterProcessor() ; 
			mobjClusteringOptions = new MultiAlign.Clustering.clsClusterOptions() ; 
			mobjMassTagDBOptions = new MultiAlign.MassTags.clsMassTagDatabaseOptions() ; 
			mobjPeakMatcher = new MultiAlign.PeakMatching.clsPeakMatchingProcessor() ; 
			mblnUseMassTagDBAsBaseline = false ; 
			menmState = enmState.IDLE ; 
			marrDatasetAlignmentFunctions = new System.Collections.ArrayList() ; 
			marrDatasetAlignmentOptions = new System.Collections.ArrayList() ; 
			mblnPeakMatchedToMasstagDB  = false ; 
			mobj_treeRoot = null;
			mstrAnalysisName = string.Empty;

            mlist_alignmentData = new List<clsAlignmentData>();
        }
        #endregion

        #region Disposing/Abort/Destruction
        ~clsMultiAlignAnalysis()
		{
			if (mthreadCurrentStatus != null)
			{
				if (mthreadCurrentStatus.IsAlive)
					mthreadCurrentStatus.Abort() ; 
				mthreadCurrentStatus = null ; 
			}

            AbortAnalysisThread();
		}
        /// <summary>
        /// Dispose method that will kill the analysis thread.
        /// </summary>
		public void Dispose()
		{
			if (mthreadCurrentStatus != null)
			{
				if (mthreadCurrentStatus.IsAlive)
					mthreadCurrentStatus.Abort() ; 
				mthreadCurrentStatus = null ; 
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
			if (mthreadCurrentStatus != null)
			{
				if (mthreadCurrentStatus.IsAlive)
					mthreadCurrentStatus.Abort() ; 
				mthreadCurrentStatus = null ; 
			}

            /// 
            /// Kill any persisting analysis objects.
            /// 
            AbortAnalysisThread();
        }
        #endregion

        #region "Loading mass tag database"
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
            Thread procThread       = new Thread(new ThreadStart(MonitorMassTagDBLoading));
            mthreadCurrentStatus    = procThread;
            procThread.Name = "Loading Mass Tag Database Thread Monitor";

            mobjMassTagDBLoader = new MultiAlign.MassTags.clsMTDBLoader(mobjMassTagDBOptions);
            procThread.Start();

            mobjMassTagDB   = mobjMassTagDBLoader.LoadMassTagDatabase();
            StatusMessage(0, mobjMassTagDBLoader.StatusMessage);
            menmState = enmState.LOADING_MASSTAGS_COMPLETE;

            
        }
        #endregion
		
		#region "Loading of UMC files"
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
				menmState = enmState.LOADING_FEATURES ; 
				Thread procThread = new  Thread(new System.Threading.ThreadStart(MonitorLoading));
				mthreadCurrentStatus = procThread ; 
				procThread.Name = "Loading Thread Monitor";
				procThread.Start() ; 
				mobjUMCCreator.UMCFindingOptions = mobjUMCFindingOptions ;

				for (int fileNum = 0 ; fileNum < FileNames.Length ; fileNum++)
				{
					mobjUMCCreator.FileName = FileNames[fileNum] ;
                    if (StatusMessage != null)
                    {
                        StatusMessage(mint_statusLevel, "Loading " + FileNames[fileNum]);
                    }

                    mint_statusLevel++;
					string extension = Path.GetExtension(FileNames[fileNum]).ToUpper(); 
					if (extension  == ".PEK")					
						mobjUMCCreator.LoadUMCs(true) ; 					
					else if (extension == ".CSV")					
						mobjUMCCreator.LoadUMCs(false) ; 					
					else					
						throw new ArgumentException("Incorrect extension for file. Please use pek or csv files as inputs.") ; 
					

					if (IsotopePeaksLoadedForFile != null)					
						IsotopePeaksLoadedForFile(FileNames[fileNum], mobjUMCCreator.GetIsotopePeaks()) ;
                                   

					mobjUMCCreator.FindUMCs() ; 
					clsUMC [] loadedUMCs =  mobjUMCCreator.GetUMCs() ;

					if (UMCsCalculatedForFile != null)					
						UMCsCalculatedForFile(FileNames[fileNum], loadedUMCs) ; 
					
					mobjUMCData.SetUMCS(FileNames[fileNum], ref loadedUMCs) ;
                    mint_statusLevel--;
				}
				menmState = enmState.DONE_LOADING_FEATURES ; 
			}
			catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
				menmState = enmState.ERROR ;
			}
		}
		#endregion 

        #region "Alignment"
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
				MultiAlign.Alignment.clsAlignmentOptions alignmentOptions = mobjAlignmentOptions ; 
				marrDatasetAlignmentOptions.Add(alignmentOptions) ;
			}
		}
		public  void  AlignDataset(int datasetIndex)
		{
			menmState                               = enmState.ALIGNING ; 
			clsAlignmentOptions alignmentOptions    =  marrDatasetAlignmentOptions[datasetIndex] as clsAlignmentOptions; 			
            mobjAlignmentProcessor.AlignmentOptions = alignmentOptions ; 

			int minScanRef = 0;
            int maxScanRef = 0 ; 

			if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
			{
				if (!mobjAlignmentProcessor.MassTagDBLoaded)
					mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjMassTagDB) ; 
			}
			else
			{
				int baselineIndex = mobjUMCData.GetDatasetIndex(alignmentOptions.AlignmentBaselineName) ; 
				if (baselineIndex == datasetIndex)
					return ; 
				mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjUMCData, baselineIndex) ; 
				mobjUMCData.GetMinMaxScan(baselineIndex, ref minScanRef, ref maxScanRef) ; 
			}

			string aligneeDataset = (string) marrFileNames[datasetIndex] ; 
			if (StatusMessage != null)
			    StatusMessage(mint_statusLevel, "Aligning " + aligneeDataset) ;
            mint_statusLevel++;

			mobjAlignmentProcessor.SetAligneeDatasetFeatures(mobjUMCData, datasetIndex) ;  
			mobjAlignmentProcessor.PerformAlignmentToMSFeatures() ; 
			clsAlignmentFunction alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction() ; 

			/// 
            /// Insert until this dataset.
            /// 
            while (marrDatasetAlignmentFunctions.Count < datasetIndex + 1)
            {
                marrDatasetAlignmentFunctions.Add(null);
                mlist_alignmentData.Add(null);
            }

			marrDatasetAlignmentFunctions[datasetIndex] = alignmentFunction ; 
			mobjAlignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref mobjUMCData) ;

            int minScanBaseline     = 0;
            int maxScanBaseline     = 0; 
			mobjUMCData.GetMinMaxScan(datasetIndex, ref minScanBaseline, ref maxScanBaseline) ; 
			float [,] heatScores    = new float[1,1]; 
			float [] xIntervals     = new float[1] ; 
			float [] yIntervals     = new float[1] ; 

			mobjAlignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals) ;

            float minMTDBNET = 0.0F;
            float maxMTDBNET = 1.0F; 

			mobjAlignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET) ;


            clsAlignmentData data = new clsAlignmentData();
            data.aligneeDataset = aligneeDataset;
            data.alignmentFunction = alignmentFunction;
            data.heatScores = heatScores;
            data.minScanBaseline = minScanBaseline;
            data.maxScanBaseline = maxScanBaseline;

			if (DatasetAligned != null)
			{
				if (alignmentOptions.IsAlignmentBaselineAMasstagDB)
				{
					DatasetAligned(alignmentFunction, aligneeDataset, ref heatScores, (float) minScanBaseline, 
						(float) maxScanBaseline, minMTDBNET, maxMTDBNET) ; 
				}
				else
				{
					DatasetAligned(alignmentFunction, aligneeDataset, ref heatScores, (float) minScanBaseline, 
						(float) maxScanBaseline, (float) minScanRef, (float) maxScanRef) ; 
				}
                
			}
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

            mlist_alignmentData[datasetIndex] = data;

            mint_statusLevel--;
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


            aligneeDataset = (string)marrFileNames[datasetIndex];            
            alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction(); 

            return true;
        }
		public  void  AlignDatasets()
		{
			menmState                           = enmState.ALIGNING ; 
			System.Threading.Thread procThread  = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorAlignments));
			mthreadCurrentStatus                = procThread ; 
			procThread.Name                     = "Alignment Thread Monitor";
			procThread.Start();

            for (int datasetNum = 0; datasetNum < marrFileNames.Count; datasetNum++)
			{                
				AlignDataset(datasetNum) ;
			}
			menmState = enmState.DONE_ALIGNING ; 
		}
		private void  AlignClustersToMassTagDB() 
		{
			try
			{
				menmState = enmState.ALIGNING ; 
				System.Threading.Thread procThread = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorAlignments));
				mthreadCurrentStatus = procThread ; 
				procThread.Name = "Alignment Thread Monitor";
				procThread.Start() ; 

				if (StatusMessage != null)
				{
					StatusMessage(0, "Aligning Clusters to Mass Tag Database") ; 
				}

				mobjAlignmentProcessor.SetReferenceDatasetFeatures(mobjMassTagDB) ; 
				mobjAlignmentProcessor.SetAligneeDatasetFeatures(mobjUMCData.mobjClusterData) ;  
				mobjAlignmentProcessor.PerformAlignmentToMassTagDatabase() ; 
				mobjAlignmentProcessor.ApplyNETMassFunctionToAligneeDatasetFeatures(ref mobjUMCData.mobjClusterData) ;  
				MultiAlign.Alignment.clsAlignmentFunction alignmentFunction = mobjAlignmentProcessor.GetAlignmentFunction() ; 

				float [,] heatScores = new float[1,1] ; 
				float [] xIntervals = new float[1] ; 
				float [] yIntervals = new float[1] ; 
				mobjAlignmentProcessor.GetAlignmentHeatMap(ref heatScores, ref xIntervals, ref yIntervals) ;

				float minClusterNET=0.0F , maxClusterNET = 1.0F ; 
				mobjUMCData.mobjClusterData.GetMinMaxNET(ref minClusterNET, ref maxClusterNET) ; 
				float minMTDBNET=0.0F , maxMTDBNET = 1.0F ; 
				mobjAlignmentProcessor.GetReferenceNETRange(ref minMTDBNET, ref maxMTDBNET) ; 

				if (DatasetAligned != null)
				{
					DatasetAligned(alignmentFunction, "Clusters", ref heatScores, minClusterNET, maxClusterNET, minMTDBNET, maxMTDBNET) ; 
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

        #region "Clustering"
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
        public void PerformClustering()
        {
            menmState = enmState.CLUSTERING;
            System.Threading.Thread procThread = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorClustering));
            mthreadCurrentStatus = procThread;
            procThread.Name = "Clustering Thread Monitor";
            procThread.Start();

            mint_statusLevel++;

            mobjClusterProcessor.ClusterOptions = mobjClusteringOptions;
            if (StatusMessage != null)
            {
                StatusMessage(0, "Performing Clustering of data points");
            }
            mobjClusterProcessor.PerformClustering(mobjUMCData);

            mint_statusLevel--;
            // lets write it out.
            menmState = enmState.DONE_CLUSTERING;
        }
        #endregion

        #region "Peak Matching"
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
        public void PerformPeakMatching()
        {
            mint_statusLevel++;
            try
            {
                // firstly, if things are not already aligned to a mass tag database then 
                // lets align them now. 
                //if (!mblnUseMassTagDBAsBaseline && mobjMassTagDBOptions.mstrDatabase != null)
                if (mobjMassTagDBOptions.mstrDatabase != null)
                {
                    AlignClustersToMassTagDB();
                }

                menmState = enmState.PEAKMATCHING;
                System.Threading.Thread procThread = new System.Threading.Thread(new System.Threading.ThreadStart(MonitorPeakMatching));
                mthreadCurrentStatus = procThread;
                procThread.Name = "Peak Matching Thread Monitor";
                procThread.Start();

                if (StatusMessage != null)
                {
                    StatusMessage(0, "Performing Peak Matching");
                }

                mobjPeakMatcher.MassTolerance = mobjClusteringOptions.MassTolerance;
                mobjPeakMatcher.NETTolerance = mobjClusteringOptions.NETTolerance;
                mobjPeakMatchingResults = mobjPeakMatcher.PerformPeakMatching(mobjUMCData.mobjClusterData, mobjMassTagDB);
                StatusMessage(0, Convert.ToString(mobjPeakMatchingResults.NumMassTagsMatched) + " masstags matched to "
                    + Convert.ToString(mobjPeakMatchingResults.NumProteinsMatched) + " proteins. Through "
                    + Convert.ToString(mobjPeakMatchingResults.NumMatches) + " matches");
                mblnPeakMatchedToMasstagDB = true;
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
            mstring_saveFileName = saveFilePath;

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
            if (MassTagDBOptions.menm_databaseType != MultiAlign.MassTags.MassTagDatabaseType.None)
            {
                listSteps.Add(stepPeakMatch);
            }
            if (massTagDBName != null && massTagDBName != "" || 
                MassTagDBOptions.menm_databaseType == MultiAlign.MassTags.MassTagDatabaseType.ACCESS)
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
                MassTagDBOptions.menm_databaseType == MultiAlign.MassTags.MassTagDatabaseType.ACCESS)
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
            mint_statusLevel    = CONST_FIRST_LEVEL;
            if (CurrentStep != null)
                CurrentStep(listSteps.IndexOf(stepCluster), stepCluster);
            PerformClustering();

            /// ////////////////////////////////////////////// 
            /// Part Five:
            ///     Perform Peak Matching
            /// ////////////////////////////////////////////// 
            mint_statusLevel = CONST_FIRST_LEVEL;
            if (MassTagDBOptions.menm_databaseType != MultiAlign.MassTags.MassTagDatabaseType.None)
            {
                if (CurrentStep != null)
                    CurrentStep(listSteps.IndexOf(stepPeakMatch), stepPeakMatch);
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
                SerializeAnalysisToFile(mstring_saveFileName);
            }
            catch(Exception ex)
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

		#region "Factors"
		/// <summary>
		/// Builds the factor tree from the internal factor and dataset 
        /// information, and builds the tree node heirarchy associated with
		/// the dataset.
		/// </summary>
		/// <returns>Factor Tree Structure</returns>
		public clsTreeNode BuildFactorTreeNode()
		{			
			if (mobj_factorTree == null)
                BuildFactorTree();

			return mobj_factorTree.BuildTree();
		}
		/// <summary>
		/// Builds the factor tree from the internal factor and dataset information.
		/// </summary>
		/// <returns>Factor tree</returns>
		public clsFactorTree BuildFactorTree()
		{			
			if (mobj_factorTree == null)
				mobj_factorTree = new clsFactorTree();

			mobj_factorTree.Data.Clear();
			mobj_factorTree.Factors.Clear();
			mobj_factorTree.ClearFactors();
			
			foreach(MultiAlign.clsDatasetInfo info in  Files)
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
						MultiAlign.clsFactorInfo factorInfo = info.Factors[i] as clsFactorInfo;
						factors.Add(factorInfo.mstrFactor, info.AssignedFactorValues[i]);						
					}										
					mobj_factorTree.AddData(datasetName, factors);		// add to dataset. 
				}
			}
			MultiAlign.clsDatasetInfo datasetInfo = Files[0] as MultiAlign.clsDatasetInfo;
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

		#region "Properties"
        /// <summary>
        /// Gets or sets the alignment data.
        /// </summary>
        public List<clsAlignmentData> AlignmentData
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
		public PNNLControls.clsTreeNode FactorTreeNode
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
		public PNNLControls.clsFactorTree FactorTree
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
				return (string []) marrFileNames.ToArray(typeof(string));
			}
			set
			{
				marrFileNames = new System.Collections.ArrayList() ; 
				marrFileNames.InsertRange(0, value) ; 
			}
		}
        /// <summary>
        /// Gets or sets the name of the baseline dataset.
        /// </summary>
		[MultiAlign.clsDataSummaryAttribute("Baseline Dataset")]
		public string BaselineDataset
		{
			get
			{
				return mstrBaselineDataset ;
			}
			set
			{
				mstrBaselineDataset = value ; 
			}
		}		
		/// <summary>
		/// Gets or sets the default alignment options.
		/// </summary>
		public MultiAlign.Alignment.clsAlignmentOptions DefaultAlignmentOptions
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
        /// Gets or sets the Alignment Options.
        /// </summary>
		public System.Collections.ArrayList AlignmentOptions
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
		public MultiAlign.MassTags.clsMassTagDatabaseOptions MassTagDBOptions
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
		[MultiAlign.clsDataSummaryAttribute("Use MTDB As Baseline")]		
		public bool UseMassTagDBAsBaseline
		{
			get
			{
				return mblnUseMassTagDBAsBaseline ; 
			}
			set
			{
				mblnUseMassTagDBAsBaseline = value ; 
			}
		}
        /// <summary>
        /// Gets or sets the UMC Finding Options.
        /// </summary>
		public MultiAlign.Features.clsUMCFindingOptions UMCFindingOptions
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
		public MultiAlign.Clustering.clsClusterOptions ClusterOptions
		{
			get
			{
				return mobjClusteringOptions ;
			}
			set
			{
				mobjClusteringOptions = value ;
			}
		}
		/// <summary>
		/// Gets the UMC Data feature options.
		/// </summary>
		public MultiAlign.Features.clsUMCData UMCData
		{
			get
			{
				return mobjUMCData ; 
			}
		}
        /// <summary>
        /// Gets the peak matching results 
        /// </summary>
		public MultiAlign.PeakMatching.clsPeakMatchingResults PeakMatchingResults
		{
			get
			{
				return mobjPeakMatchingResults ; 
			}
		}		
        /// <summary>
        /// Gets the flag whether the results were peaked matched against the Mass Tag Database.
        /// </summary>
		[MultiAlign.clsDataSummaryAttribute("Peaks Matched to MTDB")]		
		public bool PeakMatchedToMassTagDB
		{
			get
			{
				return mblnPeakMatchedToMasstagDB ; 
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
        [MultiAlign.clsDataSummaryAttribute("Analysis Name")]
        public string AnalysisName
        {
            get
            {
                return mstrAnalysisName;
            }
            set
            {
                mstrAnalysisName = value;
            }
        }
        /// <summary>
        /// Gets or sets the pathname associated with the analysis.
        /// </summary>
        public string PathName
        {
            get
            {
                return mstrPathname;
            }
            set
            {
                mstrPathname = value;
            }
        }
        /// <summary>
        /// Serializes the analysis to the path provided.
        /// </summary>
        /// <param name="fileName">Path to save the analysis to.</param>
        public void SerializeAnalysisToFile(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            if (mstrAnalysisName == string.Empty)
                mstrAnalysisName = fi.Name;
            mstrPathname = fileName;

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
                    progressStream.ProgressChanged += new PNNLControls.clsReadProgressStream.ProgressChangedEventHandler(progressStream_ProgressChanged);

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
            catch (IOException ex)
            {
                System.Windows.Forms.MessageBox.Show("MultiAlign could not read the file.");                
            }

            if (analysis != null)
            {
                if (analysis.AnalysisName == null)
                    analysis.AnalysisName = string.Empty;                
                analysis.PathName = fileName;
                if (analysis.AlignmentData == null)
                    analysis.AlignmentData = new List<clsAlignmentData>();
            }
            return analysis;
        }
        #endregion

        #region Parameter Options Reflection
        private void ReflectParameterOptions(object o, IDLTools.MetaNode node)
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
									System.Windows.Forms.MessageBox.Show("Could not save " + attr.Description + " parameter. " + ex.Message);
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
							catch(Exception ex)
							{
								System.Windows.Forms.MessageBox.Show("Could not save " + attr.Description + " parameter. " + ex.Message);
							}
						}
					}					
				}	
			}
		}
		private void LoadParameterOptions(object o, IDLTools.MetaNode node)
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
								catch(Exception ex)
								{
									System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
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
							catch(Exception ex)
							{
								System.Windows.Forms.MessageBox.Show("Could not load " + attr.Description + " parameter. " + ex.Message);
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
			IDLTools.MetaData metaData = new IDLTools.MetaData("MultiAlignWin");			
		    //ReflectParameterOptions(AlignmentOptions[0], metaData.OpenChild("AlignmentOptions"));
			ReflectParameterOptions(ClusterOptions, metaData.OpenChild("ClusterOptions"));
			ReflectParameterOptions(DefaultAlignmentOptions, metaData.OpenChild("DefaultAlignmentOptions"));
			ReflectParameterOptions(MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
			ReflectParameterOptions(UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));					
			metaData.WriteFile(filename);
		}
		/// <summary>
		/// Loads parameters from file specified in XML format
		/// </summary>
		/// <param name="filename">Parameter file to load parameters</param>
		public void LoadParametersFromFile(string filename)
		{
			IDLTools.MetaData metaData = new IDLTools.MetaData("MultiAlignWin");
			metaData.ReadFile(filename);
						
			//ReflectParameterOptions(AlignmentOptions[0], metaData.OpenChild("AlignmentOptions"));
			LoadParameterOptions(ClusterOptions, metaData.OpenChild("ClusterOptions"));
			LoadParameterOptions(DefaultAlignmentOptions, metaData.OpenChild("DefaultAlignmentOptions"));
			LoadParameterOptions(MassTagDBOptions, metaData.OpenChild("MassTagDBOptions"));
			LoadParameterOptions(UMCFindingOptions, metaData.OpenChild("UMCFindingOptions"));
        }
        #endregion        
    }
}
