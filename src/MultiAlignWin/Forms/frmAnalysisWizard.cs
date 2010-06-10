/*////////////////////////////////////////////////////////////////////////
 *  File  : frmAnalysisWizard.cs
 *  Author: Ashoka Polpitya
 *          Navdeep Jaitly
 *          Brian LaMarche
 *  Date  : 9/11/2008 
 * 
 *  Pacific Northwest National Laboratory
 *  Instrument Development Laboratory
 * 
 *  Notes:
 *      Analysis wizard form for starting and running an analysis.
 * 
 *  Revisions:
 *      9-11-2008:
 *          - Removed the select data source option so data can be loaded
 *              from multiple sources.
 *          - Added this comment header block.
 *      9-12-2008:
 *          - Added a reference to the server information to 
 *              mass tag databases used.
 *      9-16-2008:
 *          - Moved the analysis methods into the clsMultiAlignAnalysisObject
 * 
 *////////////////////////////////////////////////////////////////////////	

using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Wizard.UI;
using PNNLControls;

using MultiAlignEngine;
using MultiAlignEngine.Alignment;

using PNNLProteomics.Data;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Loaders;
using PNNLProteomics.Data.Analysis;
using PNNLProteomics.SMART;

using MultiAlignWin.IO;
using MultiAlignWin.Forms.Parameters;

namespace MultiAlignWin
{
    /// <summary>
    /// Denotes what type of data is being input.
    /// </summary>
	public enum enmDataSourceType 
    { 
        DATASET_ID,
        FILTER,
        FROM_DISK 
    } ;
    /// <summary>
    /// Enumeration telling us where the options are coming from and how to run them.
    /// </summary>
	public enum enmAnalysisType
    {
        NEW,
        CREATE_PARAMETER_FILE,
        LOAD_PARAMETER_FILE
    };

    /// <summary>
    /// Form that handles the user interface for setting up analysis through the use of a wizard.
    /// </summary>
	public class frmAnalysisWizard : WizardSheet
    {
        #region Delegates
        public delegate  void UpdateProgress(int percent);
        private delegate void DelegateSetPercent(int percent);
        private delegate void DelegateSetStatus(string message);

        private delegate void DelegateSetCurrentStep(int index, string step);
        private delegate void DelegateSetSteps(List<string> step);

        #endregion

        #region Analysis Setup Step Constants 
        private const int CONST_STEP_LOAD       = 0;
        private const int CONST_STEP_FACTORS    = 1;
        private const int CONST_STEP_PARAMETERS = 2;
        private const int CONST_STEP_SAVE       = 3;
        private const int CONST_STEP_ANALYZE    = 4;
        private const int CONST_STEP_DONE       = 5;
        #endregion

        #region Members

        #region Wizard Controls
        ctlLoadDatasetWizardPage        mctl_loadDatasetPage;
        ctlDefineFactorsWizardPage      mctl_defineFactorsPage;
        ctlSelectParametersWizardPage   mctl_selectParametersPage;
        ctlSelectOutputNameWizardPage   mctl_selectOutputNamePage;
        ctlPerformAnalysisWizardPage    mctl_performAnalysisPage;
        ctlCompletedWizardPage          mctl_completedWizardPage;
        #endregion 

        private clsDMSServerInformation mobj_serverInformation;
        private string [] marrFileNames                 = null ;
        private List<clsDatasetInfo> marrDatasetInfo    = new List<clsDatasetInfo>();   	
        private enmAnalysisType         mobj_analysisType;
        private clsMultiAlignAnalysis   mobjAnalysis;
        private int     mintBaselineIndex   = -1;
        private bool    MassTagDBselected   = false;
		private string  destinationPath     = null;
        private IContainer components       = null;
        /// <summary>
        /// Flag indicating if the defaults were loaded on the first display of the parameters page.
        /// </summary>
        private bool mbool_parametersSet = false;
        private string[] marr_tempCopiedFiles;

        /// <summary>
        /// List of steps to complete.
        /// </summary>
        private List<string> mlist_wizardSteps = new List<string>();


        /// <summary>
        /// Delegate definition so we can pass a list of files to the thread to copy them for us.
        /// </summary>
        /// <param name="sourceLocations"></param>
        private delegate void DelegatedCopyFilesThreaded(string[] sourceLocations);
        private volatile int mint_numCopied;
        /// <summary>
        /// Total number of files we have reviewed for the copy.
        /// </summary>
        private volatile int mint_numReviewedForCopy;
        private volatile bool mbool_copyingFiles;
        private volatile bool mbool_finishedCopying;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor that starts a new type of analysis.
        /// </summary>
		public frmAnalysisWizard()
		{
			mobj_analysisType = enmAnalysisType.NEW;			
			InitializeComponent();			
			Init() ; 
		}
		/// <summary>
		/// Constructor that takes the type of analysis to perform.
		/// </summary>
		/// <param name="typeOfAnalysis"></param>
		public frmAnalysisWizard(enmAnalysisType typeOfAnalysis)
		{
			mobj_analysisType = typeOfAnalysis;
			InitializeComponent();		
			Init() ;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the controls and synchronizes the events for loading and moving about the wizard pages.
        /// </summary>
        private void Init()
        {
            /// 
            /// Create the control pages
            /// 
            ControlPagesInit();

            /// 
            /// Create a new analysis object and synch events.
            /// 
            mobjAnalysis = new clsMultiAlignAnalysis(new DelegateSetPercentComplete(SetPercentComplete),
                                                     new DelegateSetStatusMessage(this.SetStatusMessage), null);

            LoadAlignmentOptions();
            LoadClusterOptions();
            LoadPeakMatchingOptions();
            LoadFeatureFindingOptions();
            LoadDBOptions();
            LoadSMARTOptions();

            mobjAnalysis.AnalysisComplete                       += new clsMultiAlignAnalysis.DelegateAnalysisComplete(mobjAnalysis_AnalysisComplete);

            /// 
            /// Paremeter Setting Handling Events
            /// 
            mctl_selectParametersPage.PeakPickingParameters     += new ctlSelectParametersWizardPage.OptionsButtonClicked(PeakPickingParametersClicked);
            mctl_selectParametersPage.LoadParametersFromFile    += new ctlSelectParametersWizardPage.OptionsButtonClicked(LoadParametersFromFileClicked);
            mctl_selectParametersPage.AlignmentParameters       += new ctlSelectParametersWizardPage.OptionsButtonClicked(AlignmentParametersClicked);
            mctl_selectParametersPage.ClusteringParameters      += new ctlSelectParametersWizardPage.OptionsButtonClicked(ClusteringParametersClicked);
            mctl_selectParametersPage.PeakMatchingParameters    += new ctlSelectParametersWizardPage.OptionsButtonClicked(PeakMatchingParametersClicked);            
            mctl_selectParametersPage.SelectMassTagDatabase     += new ctlSelectParametersWizardPage.OptionsButtonClicked(SelectMassTagDatabaseClicked);
            mctl_selectParametersPage.ScoringParameters         += new ctlSelectParametersWizardPage.OptionsButtonClicked(ScoringParametersClicked);


            mobjAnalysis.IsotopePeaksLoadedForFile              += new clsMultiAlignAnalysis.DelegateIsotopePeaksLoadedForFile(IsotopePeaksLoadedForFile);
            mobjAnalysis.UMCSLoadedForFile                      += new clsMultiAlignAnalysis.DelegateUMCSLoadedForFile(mobjAnalysis_UMCSLoadedForFile);
            mobjAnalysis.DatasetAligned                         += new clsMultiAlignAnalysis.DelegateDatasetAligned(DatasetAligned);
            mobjAnalysis.ListOfSteps                            += new clsMultiAlignAnalysis.DelegateListOfSteps(mobjAnalysis_ListOfSteps);
            mobjAnalysis.CurrentStep                            += new clsMultiAlignAnalysis.DelegateCurrentStep(mobjAnalysis_CurrentStep);
            FormClosing                                         += new FormClosingEventHandler(frmAnalysisWizard_FormClosing);
        }

        /// <summary>
        /// Initializes the Wizard control pages.
        /// </summary>
        private void ControlPagesInit()
        {
            /// 
            /// Create some server information to pass to our server testers.
            /// 
            mobj_serverInformation = new clsDMSServerInformation();

            /// 
            /// Create a list of steps to perform.
            /// 
            mlist_wizardSteps.Clear();
            mlist_wizardSteps.AddRange(new string[] {"Select Data", 
                                            "Define Factors", 
                                            "Define Alignment, Cluster, Peak Matching, and MTDB Parameters",
                                            "Select Save Location",
                                            "Analyze",
                                            "Done"}
                                        );
            DisplayListOfSteps(mlist_wizardSteps);



            mctl_loadDatasetPage = new ctlLoadDatasetWizardPage();
            mctl_defineFactorsPage = new ctlDefineFactorsWizardPage();
            mctl_selectParametersPage = new ctlSelectParametersWizardPage();
            mctl_selectOutputNamePage = new ctlSelectOutputNameWizardPage();
            mctl_performAnalysisPage = new ctlPerformAnalysisWizardPage();
            mctl_completedWizardPage = new ctlCompletedWizardPage();

            mctl_loadDatasetPage.Dock = DockStyle.Fill;
            mctl_defineFactorsPage.Dock = DockStyle.Fill;
            mctl_selectParametersPage.Dock = DockStyle.Fill;
            mctl_selectOutputNamePage.Dock = DockStyle.Fill;
            mctl_performAnalysisPage.Dock = DockStyle.Fill;
            mctl_completedWizardPage.Dock = DockStyle.Fill;

            mctl_loadDatasetPage.WizardNext += new WizardPageEventHandler(mctl_loadDatasetPage_WizardNext);
            mctl_defineFactorsPage.WizardNext += new WizardPageEventHandler(MoveToParametersPage);
            mctl_defineFactorsPage.WizardBack += new WizardPageEventHandler(MoveToLoadDataSourcePage);

            /// 
            /// The user should not be able to move back from the parameter defintion page.
            /// 
            if (mobj_analysisType == enmAnalysisType.NEW)
            {
                mctl_selectParametersPage.WizardBack += new WizardPageEventHandler(MoveToDefineFactorsPage);
                mctl_selectParametersPage.WizardNext += new WizardPageEventHandler(MoveToSelectOutputPage);
            }
            else
            {
                mctl_selectParametersPage.AlignToDatabase(false);
                mctl_selectParametersPage.WizardNext += new WizardPageEventHandler(MoveToSelectOutputPage);
            }

            mctl_selectOutputNamePage.WizardBack += new WizardPageEventHandler(MoveToParametersPage);
            mctl_selectOutputNamePage.WizardNext += new WizardPageEventHandler(MoveToPerformAnalysisPage);
            mctl_performAnalysisPage.WizardBack += new WizardPageEventHandler(MoveBackToSelectOutputPage);
            mctl_performAnalysisPage.WizardNext += new WizardPageEventHandler(StopAnalysisNextPress);
            mctl_performAnalysisPage.ReadyForAnalysis += new ctlPerformAnalysisWizardPage.DelegateControlLoaded(mctl_performAnalysisPage_ReadyForAnalysis);

            if (mobj_analysisType == enmAnalysisType.NEW || mobj_analysisType == enmAnalysisType.LOAD_PARAMETER_FILE)
            {
                Pages.Add(mctl_loadDatasetPage);
                Pages.Add(mctl_defineFactorsPage);
            }

            /// 
            /// By default everyone can select the parameters.
            /// 
            Pages.Add(mctl_selectParametersPage);
            if (mobj_analysisType == enmAnalysisType.NEW ||
                mobj_analysisType == enmAnalysisType.LOAD_PARAMETER_FILE)
            {
                Pages.Add(mctl_selectOutputNamePage);
                Pages.Add(mctl_performAnalysisPage);
            }
            Pages.Add(mctl_completedWizardPage);
            SetStep(CONST_STEP_LOAD, mlist_wizardSteps[CONST_STEP_LOAD]);
        }
        #endregion

        #region Settings File and Default  Loading
        /// <summary>
        /// Loads the default feature finding options from the settings file.
        /// </summary>
        private void LoadFeatureFindingOptions()
        {
            MultiAlignEngine.Features.clsUMCFindingOptions options = new MultiAlignEngine.Features.clsUMCFindingOptions();
            options.MonoMassWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingMonoMass);
            options.AveMassWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingAverageMass);
            options.LogAbundanceWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingLogAbundance);
            options.ScanWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingScan);
            options.NETWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingNet);
            options.UseNET = Properties.Settings.Default.UserPeakPickingUseNet;
            options.FitWeight = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingFit);
            options.MaxDistance = Properties.Settings.Default.UserPeakPickingMaxDist;
            options.MinUMCLength = Convert.ToInt32(Properties.Settings.Default.UserPeakPickingMinDist);
            options.ConstraintMonoMass = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingMonoMassConstraintPPM);
            options.ConstraintAveMass = Convert.ToSingle(Properties.Settings.Default.UserPeakPickingAverageMassConstraintPPM);
            options.UMCAbundanceReportingType = MultiAlignEngine.Features.enmAbundanceReportingType.PeakMax;
            
            options.IsIsotopicFitFilterInverted = Properties.Settings.Default.UserPeakPickingInvertFitFilter;
            options.IsotopicFitFilter = Properties.Settings.Default.UserPeakPickingFitFilter;
            options.IsotopicIntensityFilter = Properties.Settings.Default.UserPeakPickingIntensityFilter;
            options.UseIsotopicFitFilter = Properties.Settings.Default.UserPeakPickingUseFitFilter;
            options.UseIsotopicIntensityFilter = Properties.Settings.Default.UserPeakPickingUseIntensity;

            if (Properties.Settings.Default.UserPeakPickingUMCReportingTypePeakArea == true)
            {
                options.UMCAbundanceReportingType = MultiAlignEngine.Features.enmAbundanceReportingType.PeakArea;
            }
            mobjAnalysis.UMCFindingOptions = options;
        }
        /// <summary>
        /// Loads the default cluster options from the settings file.
        /// </summary>
        private void LoadClusterOptions()
        {
            MultiAlignEngine.Clustering.clsClusterOptions options = new MultiAlignEngine.Clustering.clsClusterOptions();
            options.MassTolerance = Properties.Settings.Default.UserClusterOptionsMassTolerance;
            options.NETTolerance = Properties.Settings.Default.UserClusterOptionsNETTolerance;
            options.DriftTimeTolerance = Properties.Settings.Default.UserClusterOptionsDriftTimeTolerance;
            options.ClusterIntensityType = MultiAlignEngine.Clustering.enmClusterIntensityType.MAX_PER_DATASET;
            options.IgnoreCharge = Properties.Settings.Default.UserClusterOptionsIgnoreChargeStates;

            if (Properties.Settings.Default.UserClusterOptionsUseMaxInDataset == false)
                options.ClusterIntensityType = MultiAlignEngine.Clustering.enmClusterIntensityType.SUM_PER_DATASET;

            options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN;
            if (Properties.Settings.Default.UserClusterOptionsUseMeanRepresentation == false)
                options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEDIAN;

            mobjAnalysis.ClusterOptions = options;
        }

        /// <summary>
        /// Loads the default cluster options from the settings file.
        /// </summary>
        private void LoadPeakMatchingOptions()
        {
            MultiAlignEngine.PeakMatching.clsPeakMatchingOptions options = new MultiAlignEngine.PeakMatching.clsPeakMatchingOptions();
            options.MassTolerance = Properties.Settings.Default.UserPeakMatchingMassTolerance;
            options.NETTolerance = Properties.Settings.Default.UserPeakMatchingNETTolerance;
            options.DriftTimeTolerance = Properties.Settings.Default.UserPeakMatchingDriftTimeTolerance;

            mobjAnalysis.PeakMatchingOptions = options;
        }
        /// <summary>
        /// Loads the saved alignment options from the settings file.
        /// </summary>
        private void LoadAlignmentOptions()
        {
            MultiAlignEngine.Alignment.clsAlignmentOptions defOptions = new MultiAlignEngine.Alignment.clsAlignmentOptions();
            defOptions.ApplyMassRecalibration = Properties.Settings.Default.UserAlignmentOptionsUseRecalibrateMasses;
            defOptions.ContractionFactor = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsContractionFactor);
            defOptions.MassCalibrationLSQNumKnots = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsLSQNumOptions);
            defOptions.MassCalibrationLSQZScore = Properties.Settings.Default.UserAlignmentOptionsLSQOutlierZScore;
            defOptions.MassCalibrationMaxJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxJump);
            defOptions.MassCalibrationMaxZScore = Properties.Settings.Default.UserAlignmentOptionsOutlierZScore;
            defOptions.MassCalibrationNumMassDeltaBins = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumMassDeltaBins);
            defOptions.MassCalibrationNumXSlices = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumXSlices);
            defOptions.MassCalibrationUseLSQ = Properties.Settings.Default.UserAlignmentOptionsUseLSQFit;
            defOptions.MassCalibrationWindow = Properties.Settings.Default.UserAlignmentOptionsMassWindowPPM;
            defOptions.MassTolerance = Properties.Settings.Default.UserAlignmentOptionsMassTolerance;
            defOptions.MaxPromiscuity = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxPromiscuity);
            defOptions.MaxTimeJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxDistortion);
            defOptions.NETTolerance = Properties.Settings.Default.UserAlignmentOptionsNETTolerance;
            defOptions.NumTimeSections = Properties.Settings.Default.UserAlignmentOptionsNumSections;
            defOptions.NETBinSize = Properties.Settings.Default.UserAlignmentOptionsNETBinSize;
            defOptions.MassBinSize = Properties.Settings.Default.UserAlignmentOptionsMassBinSize;

            if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeHybrid == true)
                defOptions.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.HYBRID_CALIB;
            else if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeMzCoeff == true)
                defOptions.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.MZ_CALIB;
            else
                defOptions.RecalibrationType = MultiAlignEngine.Alignment.enmCalibrationType.SCAN_CALIB;
            defOptions.UsePromiscuousPoints = Properties.Settings.Default.UserAlignmentOptionsIgnorePromiscuity;

            mobjAnalysis.DefaultAlignmentOptions = defOptions;
        }
        /// <summary>
        /// Loads the saved database options from the settinsg file.
        /// </summary>
        private void LoadDBOptions()
        {
            mobjAnalysis.MassTagDBOptions.mdecimalMinPMTScore           = Convert.ToDecimal(Properties.Settings.Default.UserDBFormPMTQuality);
            mobjAnalysis.MassTagDBOptions.mfltMinXCorr                  = Convert.ToSingle(Properties.Settings.Default.UserDBFormMinXCorr);
            mobjAnalysis.MassTagDBOptions.mdblMinDiscriminant           = Properties.Settings.Default.UserDBFormMinDiscriminant;
            mobjAnalysis.MassTagDBOptions.mdblPeptideProphetVal         = Properties.Settings.Default.UserDBFormPeptideProphetProbability;
            mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath         = Properties.Settings.Default.UserDBFormLocalDatabasePath;
            mobjAnalysis.MassTagDBOptions.mintMinObservationCountFilter = Properties.Settings.Default.UserDBFormMinObservationCountFilter;

            mobjAnalysis.MassTagDBOptions.mstrExperimentFilter          = Properties.Settings.Default.UserDBFormExperimentFilter;
            mobjAnalysis.MassTagDBOptions.mstrExperimentExclusionFilter = Properties.Settings.Default.UserDBFormExperimentExclusionFilter;
        }
        /// <summary>
        /// Loads the SMART Options from the settings file.
        /// </summary>
        private void LoadSMARTOptions()
        {           
            if (mobjAnalysis.SMARTOptions == null)
                mobjAnalysis.SMARTOptions = new classSMARTOptions();
            mobjAnalysis.SMARTOptions.IsDataPaired          = Properties.Settings.Default.SMARTIsDataPaired;
            mobjAnalysis.SMARTOptions.MassTolerancePPM      = Properties.Settings.Default.SMARTMassTolerance;
            mobjAnalysis.SMARTOptions.NETTolerance          = Properties.Settings.Default.SMARTNETTolerance;
            mobjAnalysis.SMARTOptions.PairedMass            = Properties.Settings.Default.SMARTPairedMass;
            mobjAnalysis.SMARTOptions.UsePriorProbabilities = Properties.Settings.Default.SMARTUsePriorProbabilities;            
        }
        /// <summary>
        /// Loads the analysis object.
        /// </summary>
        /// <param name="analysis"></param>
        private void LoadDefaults(clsMultiAlignAnalysis analysis)
        {
            /// 
            /// Alignment Options
            /// 
            clsAlignmentOptions defOptions = new clsAlignmentOptions();
            defOptions.ApplyMassRecalibration = Properties.Settings.Default.UserAlignmentOptionsUseRecalibrateMasses;
            defOptions.ContractionFactor = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsContractionFactor);
            defOptions.MassCalibrationLSQNumKnots = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsLSQNumOptions);
            defOptions.MassCalibrationLSQZScore = Properties.Settings.Default.UserAlignmentOptionsLSQOutlierZScore;
            defOptions.MassCalibrationMaxJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxJump);
            defOptions.MassCalibrationMaxZScore = Properties.Settings.Default.UserAlignmentOptionsOutlierZScore;
            defOptions.MassCalibrationNumMassDeltaBins = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumMassDeltaBins);
            defOptions.MassCalibrationNumXSlices = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsNumXSlices);
            defOptions.MassCalibrationUseLSQ = Properties.Settings.Default.UserAlignmentOptionsUseLSQFit;
            defOptions.MassCalibrationWindow = Properties.Settings.Default.UserAlignmentOptionsMassWindowPPM;
            defOptions.MassTolerance = Properties.Settings.Default.UserAlignmentOptionsMassTolerance;
            defOptions.MaxPromiscuity = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxPromiscuity);
            defOptions.MaxTimeJump = Convert.ToInt16(Properties.Settings.Default.UserAlignmentOptionsMaxDistortion);
            defOptions.NETTolerance = Properties.Settings.Default.UserAlignmentOptionsNETTolerance;
            defOptions.NumTimeSections = Properties.Settings.Default.UserAlignmentOptionsNumSections;

            if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeHybrid == true)
                defOptions.RecalibrationType = enmCalibrationType.HYBRID_CALIB;
            else if (Properties.Settings.Default.UserAlignmentOptionsCalibrationTypeMzCoeff == true)
                defOptions.RecalibrationType = enmCalibrationType.MZ_CALIB;
            else
                defOptions.RecalibrationType = enmCalibrationType.SCAN_CALIB;
            defOptions.UsePromiscuousPoints = Properties.Settings.Default.UserAlignmentOptionsIgnorePromiscuity;

            for (int i = 0; i < mobjAnalysis.AlignmentOptions.Count; i++)
            {
                clsAlignmentOptions options = mobjAnalysis.AlignmentOptions[i] as clsAlignmentOptions;
                mobjAnalysis.AlignmentOptions[i] = defOptions;
            }

            /// 
            /// Cluster Options
            /// 

            mbool_parametersSet = true;
        }
        #endregion

        #region Analysis Event Handlers 
        /// <summary>
        /// Sets the display for the current step.  Uses thread safe call if necessary.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="step"></param>
        void mobjAnalysis_CurrentStep(int index, string step)
        {
            if (InvokeRequired == true)
            {
                Invoke(new DelegateSetCurrentStep(mctl_performAnalysisPage.SetStep), 
                        new object[]{ index, step });
            }
            else
            {
                mctl_performAnalysisPage.SetStep(index, step);
            }
        }
        /// <summary>
        /// Displays the list of steps on the control perform steps wizard. 
        /// Uses thread safe call if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="steps"></param>
        void mobjAnalysis_ListOfSteps(object sender, List<string> steps)
        {

            if (InvokeRequired == true)
            {
                Invoke(new DelegateSetSteps(mctl_performAnalysisPage.DisplayListOfSteps),
                                            new object [] {steps});
            }
            else
            {
                mctl_performAnalysisPage.DisplayListOfSteps(steps);
            }
        }
        void AnalysisComplete()
        {
            mctl_performAnalysisPage.NextButtonEnabled  = true;
            SetActivePage(Pages.IndexOf(mctl_completedWizardPage));
            SetStep(CONST_STEP_DONE, mlist_wizardSteps[CONST_STEP_DONE]);            
        }
        void mobjAnalysis_AnalysisComplete(object sender)
        {
            if (InvokeRequired == true)
            {
                Invoke(new MethodInvoker(AnalysisComplete));
            }
            else
            {
                AnalysisComplete();
            }
        }
        #endregion
      
        #region Windows Generated Code
        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
            /// 
            /// Kill the dataset loading if it is currently loading datasets
            /// 
            try
            {
                mctl_loadDatasetPage.Dispose();
            }
            catch
            {

            }
            /// 
            /// Kill the analysis thread if it exists.
            /// 
            mobjAnalysis.Abort();
            mobjAnalysis = null;

			base.Dispose( disposing );
		}
		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // frmAnalysisWizard
            // 
           // this.AcceptButton = this.nextButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(687, 551);
            this.Name = "frmAnalysisWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MultiAlign Analysis Wizard";
            this.ResumeLayout(false);

		}
		#endregion
        #endregion

        #region Form Event Handlers
        /// <summary>
        /// Handles when the user tries to close the form while performing a dataset analysis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frmAnalysisWizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            /// 
            /// Only allow the user to cancel closing if the close reason is by clicking on the close button.
            /// 
            if (e.CloseReason == CloseReason.UserClosing)
            {

                /// 
                /// Make sure the user wants to stop the analysis
                /// 
                if (mobjAnalysis != null && mobjAnalysis.Processing == true)
                {
                    bool stop = StopAnalysis(null, null);
                    if (stop == false)
                    {
                        /// 
                        /// Let the analysis go on...
                        /// 
                        e.Cancel = true;
                        return;
                    }
                    else
                    {
                        /// 
                        /// Cancel the analysis
                        /// 
                        e.Cancel = false;
                        return;
                    }
                }
                else
                {
                    /// 
                    /// Make sure the user does in fact want to close and stop the process at hand.
                    /// 
                    CancelEventArgs cancelArgs = new CancelEventArgs(false);
                    OnQueryCancel(cancelArgs);

                    if (cancelArgs.Cancel == true)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false;
                    }
                }
            }
        }
        #endregion

        #region Page Handling
        /// <summary>
        /// Handlers when the program is told to start running.
        /// </summary>
        void mctl_performAnalysisPage_ReadyForAnalysis()
        {
            bool started = StartAnalysis(); //sender);            
            if (started == false)
            {
                /// 
                /// If the analysis was started. then we dont want the 
                /// user to cancel anything.  They must first stop the 
                /// anaylsis. Then continue!                
                ///                     
                mctl_performAnalysisPage.CancelButtonEnabled = false;
            }
        }             
        /// <summary>
        /// Moves the wizard to the select factors page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mctl_loadDatasetPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            List<clsDatasetInfo> datasets = mctl_loadDatasetPage.Datasets;
            mobj_serverInformation  = mctl_loadDatasetPage.ServerInformation;
            if (datasets == null)
            {
                /// 
                /// No datasets were found.
                /// 
                MessageBox.Show("No datasets were loaded.  Cannot proceed with analysis.");
            }
            else
            {
                mctl_defineFactorsPage.DatasetInfo  = datasets;
                e.NewPage                           = mctl_defineFactorsPage.Name;
                SetStep(CONST_STEP_FACTORS, mlist_wizardSteps[CONST_STEP_FACTORS]);
            }
        }
        /// <summary>
        /// Moves the wizard to loading of the datasets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToLoadDataSourcePage(object sender, WizardPageEventArgs e)
        {
           e.NewPage                                = mctl_loadDatasetPage.Name;
           mctl_loadDatasetPage.ExtraButtonVisible = false;
           SetStep(CONST_STEP_LOAD, mlist_wizardSteps[CONST_STEP_LOAD]);
        }        
        /// <summary>
        /// Moves the wizard to the parameter loading page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToParametersPage(object sender, WizardPageEventArgs e)
        {
            string[] datasetNames                   = DataSetNames;
			bool usePredefinedFeaturesOnly			= true;

			foreach (string dataset in datasetNames)
			{
				if (!dataset.Contains("_LCMSFeatures"))
				{
					// At least 1 non-Predefined-Features file
					usePredefinedFeaturesOnly = false;
					break;
				}
			}

			mctl_selectParametersPage.UsePredefinedFeaturesOnly = usePredefinedFeaturesOnly;
            mctl_selectParametersPage.DataSetNames				= datasetNames; 
            e.NewPage											= mctl_selectParametersPage.Name;
            mctl_selectParametersPage.NextButtonText			= "Next >";

            /// 
            /// Set the parameters
            /// 
            if (mbool_parametersSet == false)
            {
                LoadDefaults(mobjAnalysis);
            }

            SetStep(CONST_STEP_PARAMETERS, mlist_wizardSteps[CONST_STEP_PARAMETERS]);
        }
        /// <summary>
        /// Moves the wizard to the factor definition page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToDefineFactorsPage(object sender, WizardPageEventArgs e)
		{
			string[] Aliases = FileAliases;
			if (Aliases == null || Aliases.Length == 0)
			{
				MessageBox.Show("No files found, or the Aliases not set","Oops!", MessageBoxButtons.OK,MessageBoxIcon.Warning) ;
                e.NewPage = mctl_loadDatasetPage.Name;
                SetStep(CONST_STEP_LOAD, mlist_wizardSteps[CONST_STEP_LOAD]);		
			}
			else
			{
				//marrDatasetInfo = mctl_selectJobIdsPage.DatasetInfo ;			
				e.NewPage                             = mctl_defineFactorsPage.Name ;
				mctl_defineFactorsPage.DatasetInfo    = marrDatasetInfo ;
                //mctl_defineFactorsPage.dataSource   = datasource ;
                SetStep(CONST_STEP_FACTORS, mlist_wizardSteps[CONST_STEP_FACTORS]);
			}
        }        
        /// <summary>
        /// Moves the wizard pages to selecting the output location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MoveToSelectOutputPage(object sender, WizardPageEventArgs e)
		{
			string []aliases = mctl_selectParametersPage.FileAliases ;
			// check to see if the baseline has already been selected, e.g.: this is after a pressing 
			// of back button when it's selected. If not selected, get from the form
			if (mintBaselineIndex == -1) 
				mintBaselineIndex = mctl_selectParametersPage.SelectedFileIndex;
			// Still not selected, show a messagebox
			if (mintBaselineIndex == -1 && !mctl_selectParametersPage.UseMassTagDBAsBaseline)
			{
				MessageBox.Show("Select a basline first.", "Baseline ?", MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
                e.NewPage = mctl_selectParametersPage.Name;
                SetStep(CONST_STEP_PARAMETERS, mlist_wizardSteps[CONST_STEP_PARAMETERS]);
			}
            else if (aliases.Length == 1 && !MassTagDBselected)
            {
                MessageBox.Show("Mass Tag DB not loaded.", "Load Mass Tag DB", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                e.NewPage = mctl_selectParametersPage.Name;
                SetStep(CONST_STEP_PARAMETERS, mlist_wizardSteps[CONST_STEP_PARAMETERS]);
            }
            else
            {
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, mlist_wizardSteps[CONST_STEP_SAVE]);
            }            
		}

        /// <summary>
        /// Moves the page to performining the analysis page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToPerformAnalysisPage(object sender, WizardPageEventArgs e)
        {
            string outputFilename = mctl_selectOutputNamePage.ProjectOutputFileName;
            destinationPath       = mctl_selectOutputNamePage.DestinationFolder;
            if (destinationPath == null)
            {
                MessageBox.Show("Destination folder doesn't exist.", "Folder problem...!");
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, mlist_wizardSteps[CONST_STEP_SAVE]);
            }
            else if (outputFilename == null)
            {
                MessageBox.Show("The output file name was incorrect!");
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, mlist_wizardSteps[CONST_STEP_SAVE]);
            }
            else if (System.IO.File.Exists(Path.Combine(destinationPath, outputFilename)) == true)
            {
                DialogResult result = MessageBox.Show("The output file already exists.  Do you want to overwrite it?", "Confirm File Overwrite", MessageBoxButtons.YesNo);
                /// 
                /// Cancel due to overwrite
                /// 
                if (result == DialogResult.No)
                {
                    // Go back!
                    e.NewPage = mctl_selectOutputNamePage.Name;
                    SetStep(CONST_STEP_SAVE, mlist_wizardSteps[CONST_STEP_SAVE]);
                }
                else
                {
                    // Continue
                    
                    e.NewPage = mctl_performAnalysisPage.Name;
                    SetStep(CONST_STEP_ANALYZE, mlist_wizardSteps[CONST_STEP_ANALYZE]);
                    mctl_performAnalysisPage.NextButtonText = "Stop";

                }
            }
            else
            {
                /// 
                /// Display to start the analysis.
                ///                 
                e.NewPage = mctl_performAnalysisPage.Name;
                SetStep(CONST_STEP_ANALYZE, mlist_wizardSteps[CONST_STEP_ANALYZE]);
                mctl_performAnalysisPage.NextButtonText = "Stop";
            }
        }       
        /// <summary>
        /// Moves the page back to the select output page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MoveBackToSelectOutputPage(object sender, WizardPageEventArgs e)
		{
			mctl_performAnalysisPage.ExtraButtonVisible = false;
			mctl_performAnalysisPage.NextButtonEnabled  = true ;
			mctl_selectOutputNamePage.DestinationFolder = destinationPath ;
            mctl_selectOutputNamePage.NextButtonText = "Re-start";

            SetStep(CONST_STEP_SAVE, mlist_wizardSteps[CONST_STEP_SAVE]);
        }
        /// <summary>
        /// Perform Analysis Next.  The analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void MoveToWizardCompletedPage(object sender, WizardPageEventArgs e)
        {
            mctl_completedWizardPage.BackButtonEnabled  = false;
            e.NewPage                                   = mctl_completedWizardPage.Name;
            SetStep(CONST_STEP_DONE, mlist_wizardSteps[CONST_STEP_DONE]);
        }
        #endregion
        
        #region  Status Updates
        private void ThreadedSetPercentComplete(int percent)
        {
            mctl_performAnalysisPage.SetProgressBar(percent); 
        }

		public  void SetPercentComplete(int percentComplete)
		{
            if (InvokeRequired == true)
                BeginInvoke(new DelegateSetPercent(ThreadedSetPercentComplete), percentComplete);
            else
                ThreadedSetPercentComplete(percentComplete);
		}

        /// <summary>
        /// Sets the status message to the appropiate listeners.
        /// </summary>
        /// <param name="statusLevel"></param>
        /// <param name="message"></param>
		public  void SetStatusMessage(int statusLevel, string message)
        {
            if (InvokeRequired == true)
                BeginInvoke(new DelegateSetStatusMessage(mctl_performAnalysisPage.AddStatusMessage), statusLevel, message);
            else
                mctl_performAnalysisPage.AddStatusMessage(statusLevel, message);
            
            /// 
            /// Log the data by first constructing the path to the analysis output log.
            /// 
            try
            {
                string path = Path.GetDirectoryName(mobjAnalysis.PathName);
                path        = Path.Combine(path, mobjAnalysis.AnalysisName + ".log");
                classAnalysisLogWriter.WriteMessage(path, statusLevel, message);
            }
            catch
            {
            }
        }
		#endregion		 

        #region Parameter Event Handlers
        /// <summary>
        /// Handles changing the mass tag database name.
        /// </summary>
        /// <param name="newMTDB"></param>
        void mctl_selectParametersPage_SetMassTagDabase(string newMTDB)
        {
            if (string.IsNullOrEmpty(newMTDB) == true)
            {
                if (mobjAnalysis.MassTagDBOptions != null)
                {
                    if (mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
                    {
                        mctl_selectParametersPage.MassTagDBName = newMTDB;
                        MassTagDBselected = true;
                    }
                    else if (mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
                    {
                        mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath = newMTDB;
                        MassTagDBselected = true;
                    }
                }
            }
            else
            {
                MassTagDBselected = false;
            }
        }
		private void PeakPickingParametersClicked()
		{
			frmFeatureFindingParameters peakPickingOptionsForm = new frmFeatureFindingParameters() ;
			peakPickingOptionsForm.UMCFindingOptions = mobjAnalysis.UMCFindingOptions ;
			if (peakPickingOptionsForm.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.UMCFindingOptions = peakPickingOptionsForm.UMCFindingOptions ;
			}
		}
		private void AlignmentParametersClicked()
		{
			frmMSAlignmentParameters alignmentOptionsForm = new frmMSAlignmentParameters() ; 
			alignmentOptionsForm.AlignmentOptions = mobjAnalysis.DefaultAlignmentOptions ; 
			if (alignmentOptionsForm.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.DefaultAlignmentOptions = alignmentOptionsForm.AlignmentOptions ; 
			}
		}
		private void ClusteringParametersClicked()
		{
			frmClusterParameters clusterParametersForm = new frmClusterParameters() ;
			clusterParametersForm.ClusterOptions = mobjAnalysis.ClusterOptions ;
			if (clusterParametersForm.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.ClusterOptions = clusterParametersForm.ClusterOptions ; 
			}
		}
        private void PeakMatchingParametersClicked()
        {
            frmPeakMatchingParameters peakMatchingParametersForm = new frmPeakMatchingParameters();
            peakMatchingParametersForm.PeakMatchingOptions = mobjAnalysis.PeakMatchingOptions;
            if (peakMatchingParametersForm.ShowDialog() == DialogResult.OK)
            {
                mobjAnalysis.PeakMatchingOptions = peakMatchingParametersForm.PeakMatchingOptions;
            }
        }
		private void SelectMassTagDatabaseClicked()
		{
			frmDBName dbForm = new frmDBName(mobj_serverInformation.ConnectionExists); 
			dbForm.Owner = this;
			dbForm.MassTagDatabaseOptions = mobjAnalysis.MassTagDBOptions ; 

			if (dbForm.ShowDialog() == DialogResult.OK)
			{
				mobjAnalysis.MassTagDBOptions = dbForm.MassTagDatabaseOptions ; 
				if (mobjAnalysis.MassTagDBOptions.mstrDatabase != null &&
					mobjAnalysis.MassTagDBOptions.mstrDatabase != "" &&
					mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL
					)
				{
					mctl_selectParametersPage.MassTagDBName = mobjAnalysis.MassTagDBOptions.mstrDatabase ;					
					MassTagDBselected = true ;
				}
				else if(mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath != null &&
					mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath != "" &&
					mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
				{					
					mctl_selectParametersPage.MassTagDBName = mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath;
					MassTagDBselected = true;
				}
			}
        }
        /// <summary>
        /// Displays a form to set the scoring parameters window.
        /// </summary>
        void ScoringParametersClicked()
        {
            formPeptideIDScoring scoreForm  = new formPeptideIDScoring();
            scoreForm.Owner                 = this;
            scoreForm.Icon                  = Icon;
            scoreForm.Options               = mobjAnalysis.SMARTOptions;

            if (scoreForm.ShowDialog() == DialogResult.OK)
            {
                
                /// 
                /// Get new options
                /// 
                mobjAnalysis.SMARTOptions = scoreForm.Options;
            }            
        }
        private void LoadParametersFromFileClicked()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load Parameter File";

            if (DialogResult.OK == dialog.ShowDialog())
            {
                mobjAnalysis.LoadParametersFromFile(dialog.FileName);
            }
        }
        void mobjAnalysis_UMCSLoadedForFile(string fileName, MultiAlignEngine.Features.clsUMC[] umcs)
        {
            mctl_performAnalysisPage.DisplayUMCS(fileName, umcs);
        }
        private void IsotopePeaksLoadedForFile(string fileName, MultiAlignEngine.Features.clsIsotopePeak[] isotopePeaks)
        {
            mctl_performAnalysisPage.DisplayIsotopePeaks(fileName, isotopePeaks);
        }
        private void DatasetAligned(clsAlignmentFunction alignmentFnc, 
                                    string fileName,
                                    ref float[,] mScores,
                                    float minX,
                                    float maxX,
                                    float minY,
                                    float maxY,
                                    int   part)
        {
            mctl_performAnalysisPage.SetAlignmentHeatMap(alignmentFnc,
                                                        fileName,
                                                        mScores, 
                                                        minX,
                                                        maxX, 
                                                        minY, 
                                                        maxY, 
                                                        part);
        }
        #endregion

        #region Perform Analysis functions
        /// <summary>
        /// Handle when the perform analysis next button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PerformAnalysisNextPressed(object sender, WizardPageEventArgs e)
        {
            if (mobjAnalysis == null || mobjAnalysis.Processing == false)
            {

            }
            else
            {                
                bool stopped = StopAnalysis(sender, e);                
            }
        }


        private void StopAnalysisNextPress(object sender, WizardPageEventArgs e)
        {

            StopAnalysis(sender, e);
        }
        /// <summary>
        /// Stops the analysis from running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool StopAnalysis(object sender, WizardPageEventArgs e)
        {   
            /// 
            /// Make sure the user wants to cancel the analysis.
            /// 
            DialogResult result = MessageBox.Show("Do you want to stop the analysis?", "Stop Analysis?", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                if (e != null)
                    e.NewPage = mctl_performAnalysisPage.Name;

                return false;
            }

            /// 
            /// Tell the loading thread if it exists to stop copying files.
            /// 
            mbool_copyingFiles = false;

            /// 
            /// Show stop analysis.
            /// 
            try
            {                
                mobjAnalysis.Abort();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            
            /// 
            /// Also, now let them go back or cancel.
            /// 
            mctl_performAnalysisPage.BackButtonEnabled   = true;
            mctl_performAnalysisPage.CancelButtonEnabled = true;

            if (e != null)
            {
                mctl_selectOutputNamePage.NextButtonText = "Re-start";
                e.NewPage = mctl_selectOutputNamePage.Name;
            }
            mctl_performAnalysisPage.ResetAnalysisReady();

            return true;
        }

        /// <summary>
        /// Extra button is the Start button now. Crux of the analysis is here!		
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool StartAnalysis()
        {
            //e.Cancel = true;
            //e.NewPage = mctl_performAnalysisPage.Name;

            SetStep(CONST_STEP_ANALYZE, mlist_wizardSteps[CONST_STEP_ANALYZE]);

            /// 
            /// Disable the buttons from the user hitting anything.
            /// 
            mctl_performAnalysisPage.NextButtonText      = "Stop";
            mctl_performAnalysisPage.BackButtonEnabled   = false;
            mctl_performAnalysisPage.CancelButtonEnabled = false;

            /// 
            /// Barf! but we want this to show the current page now!
            /// 
            Application.DoEvents();

            string[] sourcePaths    = null;
            bool copySuccess        = false;
            
            mobjAnalysis.UseMassTagDBAsBaseline     = mctl_selectParametersPage.UseMassTagDBAsBaseline;
            mobjAnalysis.UseSMART                   = mctl_selectParametersPage.UseSMART;
            string massTagDBName                    = mobjAnalysis.MassTagDBOptions.mstrDatabase;
            bool alignmentDatabaseInvalid           = false;

            /// 
            /// Check to make sure that the database selected / provided is valid.  
            /// 
            if (mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
                alignmentDatabaseInvalid = (massTagDBName == null || massTagDBName == "");
            else if (mobjAnalysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
                alignmentDatabaseInvalid = (File.Exists(mobjAnalysis.MassTagDBOptions.mstr_databaseFilePath) == false);

            if (mobjAnalysis.UseMassTagDBAsBaseline && alignmentDatabaseInvalid)
            {
                MessageBox.Show("Please select a mass tag database as you are going to use one as a baseline for alignment.");
                return false;
            }

            if (!mobjAnalysis.UseMassTagDBAsBaseline && mintBaselineIndex == -1)
            {
                MessageBox.Show("Please select either a mass tag database or a dataset as a baseline for alignment.");
                return false;
            }

            sourcePaths = FileLocations;
                        
            copySuccess = CopyFilesHandler(sourcePaths);

            /// 
            /// Make sure we get all the files locally first.
            /// 
            if (copySuccess)
            {
                if (mintBaselineIndex > -1)
                {
                    mobjAnalysis.BaselineDataset = marrFileNames[mintBaselineIndex];
                }
                else
                {
                    mobjAnalysis.BaselineDataset = null;
                }
                mobjAnalysis.FileNames              = marrFileNames;

                //TODO: Change the Files in the analysis object to a strong typed list.  
                //  See if serialization of the files object will affect how data is serialized and deserialized.


                ArrayList list = new ArrayList();
                foreach (clsDatasetInfo info in marrDatasetInfo)
                {
                    list.Add(info);
                }

                mobjAnalysis.Files = list;               
                string outputPath = mctl_selectOutputNamePage.ProjectOutputFileName;
                string logPath    = Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileNameWithoutExtension(outputPath) + ".log");
                classAnalysisLogWriter.WriteHeader(logPath, "MultiAlign Analysis " + Path.GetFileNameWithoutExtension(outputPath) + " " + DateTime.Now);
                mobjAnalysis.StartAnalysis(outputPath);                
                return true;
            }
            else
            {                
                MessageBox.Show("Could not start the analysis.  One of the dataset files was unavailable.");
                return false;
            }
        }              
		#endregion 
        
        #region Copying of UMC Files
        /// <summary>
        /// Threaded copy function.
        /// </summary>
        /// <param name="o"></param>
        private void CopyFiles(object o)
        {
            string[] sourceLocations = (string[])o;

            string sourcePathAndFileName = null;
            string filePath = null;
            int numFiles = sourceLocations.Length;
            string destination;

            marr_tempCopiedFiles = new string[sourceLocations.Length];

            /// 
            /// Copy the files to local disk.
            /// 
            for (int i = 0; i < numFiles && mbool_copyingFiles == true; i++)
            {

                sourcePathAndFileName = sourceLocations[i];

                int index = sourceLocations[i].LastIndexOf("\\");
                filePath = sourcePathAndFileName.Substring(index + 1);

                //TODO: Fix the desintation path as a parameter...this is not thread safe!!!
                destination = Path.Combine(destinationPath, filePath);

                FileInfo fsource = new FileInfo(sourcePathAndFileName);
                FileInfo fdest = new FileInfo(destination);

                if (!System.IO.File.Exists(destination) || (fsource.Length != fdest.Length))
                {
                    try
                    {
                        System.IO.File.Copy(sourcePathAndFileName, destination, true);
                        /// 
                        /// A thread safe way to atomically increment the counter of the number of files copied
                        ///              
                        marr_tempCopiedFiles[i] = destination;
                        mint_numCopied++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "File.Copy");
                    }
                }
                else if (System.IO.File.Exists(destination))
                {
                    /// 
                    /// A thread safe way to atomically increment the counter of the number of files copied
                    ///              
                    mint_numCopied++;
                    marr_tempCopiedFiles[i] = destination;
                }
                mint_numReviewedForCopy = i;
            }
            /// 
            /// Tell the listening thread we are done via a flag.
            /// 
            mbool_finishedCopying = true;
        }
		private bool CopyFilesHandler(string [] sourceLocations)
		{

            mctl_performAnalysisPage.ClearListBox();
            mctl_performAnalysisPage.AddStatusMessage(0, "Copying files to working folder...");
            mctl_performAnalysisPage.Visible = true;
            mctl_performAnalysisPage.SetProgressBar(0);
			            
			/// 
            /// Make sure the user has selected files.
            /// 
			if (sourceLocations.Length == 0)
			{
				MessageBox.Show("No files selected/found");
                return false;
			}
			else
			{
                /// 
                /// Set the count to zero.
                /// 
                mint_numCopied          = 0;
                mint_numReviewedForCopy = 0;
                mbool_finishedCopying   = false;
                mbool_copyingFiles      = true;
                marr_tempCopiedFiles    = new string[sourceLocations.Length];
                int total               = sourceLocations.Length;


                string[] localFileCache = new string[sourceLocations.Length];
                string lastFileCopied   = Path.GetFileName(sourceLocations[0]);
                mctl_performAnalysisPage.AddStatusMessage(1, "Copying " + lastFileCopied);                
                sourceLocations.CopyTo(localFileCache, 0);

                /// 
                /// Register the thread
                ///                                 
                ParameterizedThreadStart start = new ParameterizedThreadStart(CopyFiles);
                Thread thread = new Thread(start);
                thread.Start(sourceLocations);
                
                /// 
                /// Eh! the poor man's way of waiting for the thread to finish!!!
                /// 
                while (mbool_copyingFiles == true && mbool_finishedCopying == false)
                {                    
                    Thread.Sleep(10);
                    /// 
                    /// Update the user interface.
                    /// 
                    int copiedPercent  = Convert.ToInt32(100.0 * (Convert.ToDouble(mint_numCopied) / Convert.ToDouble(total)));                    
                    mctl_performAnalysisPage.SetProgressBar(copiedPercent);
                    string currentCopy = Path.GetFileName(localFileCache[mint_numReviewedForCopy]);
                    if (currentCopy != lastFileCopied)
                    {
                        lastFileCopied = currentCopy;
                        mctl_performAnalysisPage.AddStatusMessage(1, "Copying " + lastFileCopied);  
                    }
                    Application.DoEvents();

                    
                }

                /// 
                /// Someone cancelled the analysis....so kill the copy thread!
                /// 
                if (mbool_copyingFiles == true)
                {
                    Thread.Sleep(100);
                    try
                    {
                        thread.Abort();
                    }
                    catch (ThreadAbortException ex)
                    {
						System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                    catch
                    {
                    }
                }
                
			}

            /// 
            /// If we copied enough of the files.
            /// 
			if (mint_numCopied == sourceLocations.Length)
			{
                marrFileNames = new string[mint_numCopied];
                marr_tempCopiedFiles.CopyTo(marrFileNames, 0);				
                return true;
			}
			return false;
		}
		#endregion 
	
        #region Properties
        public string [] FileAliases
		{
			get
			{              
                marrDatasetInfo = mctl_loadDatasetPage.Datasets;

				string[] fileAliases = new string[marrDatasetInfo.Count] ;
				for (int i = 0 ; i < marrDatasetInfo.Count ; i++)
				{
					fileAliases[i] = ((MultiAlignEngine.clsDatasetInfo)marrDatasetInfo[i]).mstrAlias ;
				}
				return fileAliases ;
			}
		}
		public string [] DataSetNames
		{
			get
			{

                marrDatasetInfo = mctl_loadDatasetPage.Datasets;

				string[] datasetNames = new string[marrDatasetInfo.Count] ;
				for (int i = 0 ; i < marrDatasetInfo.Count ; i++)
				{
					datasetNames[i] = ((MultiAlignEngine.clsDatasetInfo)marrDatasetInfo[i]).mstrDatasetName ;
				}
				return datasetNames ;
			}
		}
		public string [] FileLocations
		{
			get
			{
                marrDatasetInfo = mctl_loadDatasetPage.Datasets;

				string[] fileLocations = new string[marrDatasetInfo.Count] ;
				for (int i = 0 ; i < marrDatasetInfo.Count ; i++)
				{
					fileLocations[i] = ((MultiAlignEngine.clsDatasetInfo)marrDatasetInfo[i]).mstrLocalPath ;
				}
				return fileLocations ;
			}
		}
		public clsMultiAlignAnalysis MultiAlignAnalysis
		{
			get
			{
				return mobjAnalysis ; 
			}
        }
        #endregion
    }
}

