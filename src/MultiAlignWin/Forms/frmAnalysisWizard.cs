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

using PNNLProteomics.IO;
using PNNLProteomics.Data;
using PNNLProteomics.EventModel;
using PNNLProteomics.Data.Analysis;
using PNNLProteomics.SMART;

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
        DefineFactorsWizardPage      mctl_defineFactorsPage;
        ctlSelectParametersWizardPage   mctl_selectParametersPage;
        ctlSelectOutputNameWizardPage   mctl_selectOutputNamePage;
        ctlPerformAnalysisWizardPage    mctl_performAnalysisPage;
        ctlCompletedWizardPage          mctl_completedWizardPage;
        #endregion 

        private clsDMSServerInformation     m_serverInformation;        
        private List<DatasetInformation>    m_datasetInfoList = new List<DatasetInformation>();   	
        private enmAnalysisType             m_analysisType;
        private MultiAlignAnalysis          m_analysis;
        private int     m_baselineIndex         = -1;
        private bool    m_isMassTagDBselected   = false;
		private string  m_destinationPath       = null;
        private IContainer components           = null;
        /// <summary>
        /// Flag indicating if the defaults were loaded on the first display of the parameters page.
        /// </summary>
        private bool m_areParametersSet = false;        
        /// <summary>
        /// List of steps to complete.
        /// </summary>
        private List<string> m_wizardSteps = new List<string>();
        /// <summary>
        /// Delegate definition so we can pass a list of files to the thread to copy them for us.
        /// </summary>
        /// <param name="sourceLocations"></param>
        private delegate void DelegatedCopyFilesThreaded(string[] sourceLocations);
        /// <summary>
        /// Count of the number of files that have been copied.
        /// </summary>
        private volatile int m_numCopied;
        /// <summary>
        /// Total number of files we have reviewed for the copy.
        /// </summary>
        private volatile int m_numReviewedForCopy;
        /// <summary>
        /// Flag indicating whether files are finished copying.
        /// </summary>
        private volatile bool m_isCopyingFiles;
        /// <summary>
        /// Flag indicating whether copying has finished or not.
        /// </summary>
        private volatile bool m_isFinishedCopying;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor that starts a new type of analysis.
        /// </summary>
		public frmAnalysisWizard()
		{
			m_analysisType = enmAnalysisType.NEW;			
			InitializeComponent();			
			Init() ; 
		}
		/// <summary>
		/// Constructor that takes the type of analysis to perform.
		/// </summary>
		/// <param name="typeOfAnalysis"></param>
		public frmAnalysisWizard(enmAnalysisType typeOfAnalysis)
		{
			m_analysisType = typeOfAnalysis;
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
            m_analysis = new MultiAlignAnalysis(new DelegateSetPercentComplete(SetPercentComplete),
                                                     new DelegateSetStatusMessage(this.SetStatusMessage), null);

            LoadAlignmentOptions();
            LoadClusterOptions();
            LoadPeakMatchingOptions();
            LoadFeatureFindingOptions();
            LoadDBOptions();
            LoadSMARTOptions();

            m_analysis.AnalysisComplete                       += new MultiAlignAnalysis.DelegateAnalysisComplete(mobjAnalysis_AnalysisComplete);

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


            m_analysis.IsotopePeaksLoadedForFile              += new MultiAlignAnalysis.DelegateIsotopePeaksLoadedForFile(IsotopePeaksLoadedForFile);
            m_analysis.UMCSLoadedForFile                      += new MultiAlignAnalysis.DelegateUMCSLoadedForFile(mobjAnalysis_UMCSLoadedForFile);
            m_analysis.DatasetAligned                         += new MultiAlignAnalysis.DelegateDatasetAligned(DatasetAligned);
            m_analysis.ListOfSteps                            += new MultiAlignAnalysis.DelegateListOfSteps(mobjAnalysis_ListOfSteps);
            m_analysis.CurrentStep                            += new MultiAlignAnalysis.DelegateCurrentStep(mobjAnalysis_CurrentStep);
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
            m_serverInformation = new clsDMSServerInformation();

            /// 
            /// Create a list of steps to perform.
            /// 
            m_wizardSteps.Clear();
            m_wizardSteps.AddRange(new string[] {"Select Data", 
                                            "Define Factors", 
                                            "Define Alignment, Cluster, Peak Matching, and MTDB Parameters",
                                            "Select Save Location",
                                            "Analyze",
                                            "Done"}
                                        );
            DisplayListOfSteps(m_wizardSteps);



            mctl_loadDatasetPage = new ctlLoadDatasetWizardPage();
            mctl_defineFactorsPage = new DefineFactorsWizardPage();
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
            if (m_analysisType == enmAnalysisType.NEW)
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

            if (m_analysisType == enmAnalysisType.NEW || m_analysisType == enmAnalysisType.LOAD_PARAMETER_FILE)
            {
                Pages.Add(mctl_loadDatasetPage);
                Pages.Add(mctl_defineFactorsPage);
            }

            /// 
            /// By default everyone can select the parameters.
            /// 
            Pages.Add(mctl_selectParametersPage);
            if (m_analysisType == enmAnalysisType.NEW ||
                m_analysisType == enmAnalysisType.LOAD_PARAMETER_FILE)
            {
                Pages.Add(mctl_selectOutputNamePage);
                Pages.Add(mctl_performAnalysisPage);
            }
            Pages.Add(mctl_completedWizardPage);
            SetStep(CONST_STEP_LOAD, m_wizardSteps[CONST_STEP_LOAD]);
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
            m_analysis.UMCFindingOptions = options;
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
            options.AlignClusters = Properties.Settings.Default.UserClusterOptionsAlignToDatabase;

            if (Properties.Settings.Default.UserClusterOptionsUseMaxInDataset == false)
                options.ClusterIntensityType = MultiAlignEngine.Clustering.enmClusterIntensityType.SUM_PER_DATASET;

            options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEAN;
            if (Properties.Settings.Default.UserClusterOptionsUseMeanRepresentation == false)
                options.ClusterRepresentativeType = MultiAlignEngine.Clustering.enmClusterRepresentativeType.MEDIAN;

            m_analysis.ClusterOptions = options;
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

            m_analysis.PeakMatchingOptions = options;
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

            m_analysis.DefaultAlignmentOptions = defOptions;
        }
        /// <summary>
        /// Loads the saved database options from the settinsg file.
        /// </summary>
        private void LoadDBOptions()
        {
            m_analysis.MassTagDBOptions.mdecimalMinPMTScore           = Convert.ToDecimal(Properties.Settings.Default.UserDBFormPMTQuality);
            m_analysis.MassTagDBOptions.mfltMinXCorr                  = Convert.ToSingle(Properties.Settings.Default.UserDBFormMinXCorr);
            m_analysis.MassTagDBOptions.mdblMinDiscriminant           = Properties.Settings.Default.UserDBFormMinDiscriminant;
            m_analysis.MassTagDBOptions.mdblPeptideProphetVal         = Properties.Settings.Default.UserDBFormPeptideProphetProbability;
            m_analysis.MassTagDBOptions.mstr_databaseFilePath         = Properties.Settings.Default.UserDBFormLocalDatabasePath;
            m_analysis.MassTagDBOptions.mintMinObservationCountFilter = Properties.Settings.Default.UserDBFormMinObservationCountFilter;

            m_analysis.MassTagDBOptions.mstrExperimentFilter          = Properties.Settings.Default.UserDBFormExperimentFilter;
            m_analysis.MassTagDBOptions.mstrExperimentExclusionFilter = Properties.Settings.Default.UserDBFormExperimentExclusionFilter;
        }
        /// <summary>
        /// Loads the SMART Options from the settings file.
        /// </summary>
        private void LoadSMARTOptions()
        {           
            if (m_analysis.SMARTOptions == null)
                m_analysis.SMARTOptions = new classSMARTOptions();
            m_analysis.SMARTOptions.IsDataPaired = Properties.Settings.Default.STACIsDataPaired;
            m_analysis.SMARTOptions.MassTolerancePPM = Properties.Settings.Default.STACMassTolerance;
            m_analysis.SMARTOptions.NETTolerance = Properties.Settings.Default.STACNETTolerance;
            m_analysis.SMARTOptions.PairedMass = Properties.Settings.Default.STACPairedMass;
            m_analysis.SMARTOptions.UsePriorProbabilities = Properties.Settings.Default.STACUsePriorProbabilities;            
        }
        /// <summary>
        /// Loads the analysis object.
        /// </summary>
        /// <param name="analysis"></param>
        private void LoadDefaults(MultiAlignAnalysis analysis)
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

            for (int i = 0; i < m_analysis.AlignmentOptions.Count; i++)
            {
                clsAlignmentOptions options = m_analysis.AlignmentOptions[i] as clsAlignmentOptions;
                m_analysis.AlignmentOptions[i] = defOptions;
            }

            /// 
            /// Cluster Options
            /// 

            m_areParametersSet = true;
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
        /// <summary>
        /// 
        /// </summary>
        void AnalysisComplete()
        {
            try
            {
                /// 
                /// Get the path name and make a directory for the analysis.
                /// 
                //mstring_analysisName = System.IO.Path.GetFileNameWithoutExtension(BaselineDataset);

                /// 
                /// Update the path name
                /// 
                AnalysisBinaryWriter writer = new AnalysisBinaryWriter();
                writer.WriteAnalysis(Path.Combine(m_analysis.PathName, m_analysis.AnalysisName + ".mln"), m_analysis);                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not serialize the analysis file.  "  + ex.Message);
            }

            mctl_performAnalysisPage.NextButtonEnabled  = true;
            SetActivePage(Pages.IndexOf(mctl_completedWizardPage));
            SetStep(CONST_STEP_DONE, m_wizardSteps[CONST_STEP_DONE]);            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
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
            m_analysis.Abort();
            m_analysis = null;

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
            //this.AcceptButton = this.mnextButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(603, 498);
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
                if (m_analysis != null && m_analysis.Processing == true)
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
            List<DatasetInformation> datasets = mctl_loadDatasetPage.Datasets;
            m_serverInformation  = mctl_loadDatasetPage.ServerInformation;
            if (datasets == null)
            {
                /// 
                /// No datasets were found.
                /// 
                MessageBox.Show("No datasets were loaded.  Cannot proceed with analysis.");
            }
            else
            {
                m_datasetInfoList                   = datasets;
                mctl_defineFactorsPage.DatasetInfo  = datasets;
                e.NewPage                           = mctl_defineFactorsPage.Name;
                SetStep(CONST_STEP_FACTORS, m_wizardSteps[CONST_STEP_FACTORS]);
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
           SetStep(CONST_STEP_LOAD, m_wizardSteps[CONST_STEP_LOAD]);
        }        
        /// <summary>
        /// Moves the wizard to the parameter loading page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToParametersPage(object sender, WizardPageEventArgs e)
        {
            List<string> names = new List<string>();
            foreach (DatasetInformation info in m_datasetInfoList)
            {
                names.Add(info.DatasetName);
            }
            string[] datasetNames = new string[names.Count];
            names.CopyTo(datasetNames);

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
            if (m_areParametersSet == false)
            {
                LoadDefaults(m_analysis);
            }

            SetStep(CONST_STEP_PARAMETERS, m_wizardSteps[CONST_STEP_PARAMETERS]);
        }
        /// <summary>
        /// Moves the wizard to the factor definition page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveToDefineFactorsPage(object sender, WizardPageEventArgs e)
		{
			
			if (m_datasetInfoList.Count <= 0) 
			{
				MessageBox.Show("No files selected.","Oops!", MessageBoxButtons.OK,MessageBoxIcon.Warning) ;
                e.NewPage = mctl_loadDatasetPage.Name;
                SetStep(CONST_STEP_LOAD, m_wizardSteps[CONST_STEP_LOAD]);		
			}
			else
			{				
				e.NewPage                             = mctl_defineFactorsPage.Name ;
				mctl_defineFactorsPage.DatasetInfo    = m_datasetInfoList ;
                SetStep(CONST_STEP_FACTORS, m_wizardSteps[CONST_STEP_FACTORS]);
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
			if (m_baselineIndex == -1) 
				m_baselineIndex = mctl_selectParametersPage.SelectedFileIndex;
			// Still not selected, show a messagebox
			if (m_baselineIndex == -1 && !mctl_selectParametersPage.UseMassTagDBAsBaseline)
			{
				MessageBox.Show("Select a basline first.", "Baseline ?", MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
                e.NewPage = mctl_selectParametersPage.Name;
                SetStep(CONST_STEP_PARAMETERS, m_wizardSteps[CONST_STEP_PARAMETERS]);
			}
            else if (aliases.Length == 1 && !m_isMassTagDBselected)
            {
                MessageBox.Show("Mass Tag DB not loaded.", "Load Mass Tag DB", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                e.NewPage = mctl_selectParametersPage.Name;
                SetStep(CONST_STEP_PARAMETERS, m_wizardSteps[CONST_STEP_PARAMETERS]);
            }
            else
            {
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, m_wizardSteps[CONST_STEP_SAVE]);
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
            m_destinationPath       = mctl_selectOutputNamePage.DestinationFolder;
            if (m_destinationPath == null)
            {
                MessageBox.Show("Destination folder doesn't exist.", "Folder problem...!");
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, m_wizardSteps[CONST_STEP_SAVE]);
            }
            else if (outputFilename == null)
            {
                MessageBox.Show("The output file name was incorrect!");
                e.NewPage = mctl_selectOutputNamePage.Name;
                SetStep(CONST_STEP_SAVE, m_wizardSteps[CONST_STEP_SAVE]);
            }
            else if (System.IO.File.Exists(Path.Combine(m_destinationPath, outputFilename)) == true)
            {
                DialogResult result = MessageBox.Show("The output file already exists.  Do you want to overwrite it?", "Confirm File Overwrite", MessageBoxButtons.YesNo);
                /// 
                /// Cancel due to overwrite
                /// 
                if (result == DialogResult.No)
                {
                    // Go back!
                    e.NewPage = mctl_selectOutputNamePage.Name;
                    SetStep(CONST_STEP_SAVE, m_wizardSteps[CONST_STEP_SAVE]);
                }
                else
                {
                    // Continue
                    
                    e.NewPage = mctl_performAnalysisPage.Name;
                    SetStep(CONST_STEP_ANALYZE, m_wizardSteps[CONST_STEP_ANALYZE]);
                    mctl_performAnalysisPage.NextButtonText = "Stop";

                }
            }
            else
            {
                /// 
                /// Display to start the analysis.
                ///                 
                e.NewPage = mctl_performAnalysisPage.Name;
                SetStep(CONST_STEP_ANALYZE, m_wizardSteps[CONST_STEP_ANALYZE]);
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
			mctl_selectOutputNamePage.DestinationFolder = m_destinationPath ;
            mctl_selectOutputNamePage.NextButtonText = "Re-start";

            SetStep(CONST_STEP_SAVE, m_wizardSteps[CONST_STEP_SAVE]);
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
            SetStep(CONST_STEP_DONE, m_wizardSteps[CONST_STEP_DONE]);
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
                string analysisPath  = Path.Combine(Path.GetDirectoryName(m_analysis.PathName), m_analysis.AnalysisName);
                analysisPath         = Path.Combine(analysisPath, m_analysis.AnalysisName + ".log");
                AnalysisLogWriter.WriteMessage(analysisPath, statusLevel, message);
            }
            catch
            {
                //PASS
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
                if (m_analysis.MassTagDBOptions != null)
                {
                    if (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
                    {
                        mctl_selectParametersPage.MassTagDBName = newMTDB;
                        m_isMassTagDBselected = true;
                    }
                    else if (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
                    {
                        m_analysis.MassTagDBOptions.mstr_databaseFilePath = newMTDB;
                        m_isMassTagDBselected = true;
                    }
                }
            }
            else
            {
                m_isMassTagDBselected = false;
            }
        }
		private void PeakPickingParametersClicked()
		{
			frmFeatureFindingParameters peakPickingOptionsForm = new frmFeatureFindingParameters() ;
			peakPickingOptionsForm.UMCFindingOptions = m_analysis.UMCFindingOptions ;
			if (peakPickingOptionsForm.ShowDialog() == DialogResult.OK)
			{
				m_analysis.UMCFindingOptions = peakPickingOptionsForm.UMCFindingOptions ;
			}
		}
		private void AlignmentParametersClicked()
		{
			frmMSAlignmentParameters alignmentOptionsForm = new frmMSAlignmentParameters() ; 
			alignmentOptionsForm.AlignmentOptions = m_analysis.DefaultAlignmentOptions ; 
			if (alignmentOptionsForm.ShowDialog() == DialogResult.OK)
			{
				m_analysis.DefaultAlignmentOptions = alignmentOptionsForm.AlignmentOptions ; 
			}
		}
		private void ClusteringParametersClicked()
		{
			frmClusterParameters clusterParametersForm = new frmClusterParameters() ;
			clusterParametersForm.ClusterOptions = m_analysis.ClusterOptions ;
			if (clusterParametersForm.ShowDialog() == DialogResult.OK)
			{
				m_analysis.ClusterOptions = clusterParametersForm.ClusterOptions ; 
			}
		}
        private void PeakMatchingParametersClicked()
        {
            frmPeakMatchingParameters peakMatchingParametersForm = new frmPeakMatchingParameters();
            peakMatchingParametersForm.PeakMatchingOptions = m_analysis.PeakMatchingOptions;
            if (peakMatchingParametersForm.ShowDialog() == DialogResult.OK)
            {
                m_analysis.PeakMatchingOptions = peakMatchingParametersForm.PeakMatchingOptions;
            }
        }
		private void SelectMassTagDatabaseClicked()
		{
			frmDBName dbForm = new frmDBName(m_serverInformation.ConnectionExists); 
			dbForm.Owner = this;
			dbForm.MassTagDatabaseOptions = m_analysis.MassTagDBOptions ; 

			if (dbForm.ShowDialog() == DialogResult.OK)
			{
				m_analysis.MassTagDBOptions = dbForm.MassTagDatabaseOptions ; 
				if (m_analysis.MassTagDBOptions.mstrDatabase != null &&
					m_analysis.MassTagDBOptions.mstrDatabase != "" &&
					m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL
					)
				{
					mctl_selectParametersPage.MassTagDBName = m_analysis.MassTagDBOptions.mstrDatabase ;					
					m_isMassTagDBselected = true ;
				}
				else if(m_analysis.MassTagDBOptions.mstr_databaseFilePath != null &&
					m_analysis.MassTagDBOptions.mstr_databaseFilePath != "" &&
					m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
				{					
					mctl_selectParametersPage.MassTagDBName = m_analysis.MassTagDBOptions.mstr_databaseFilePath;
					m_isMassTagDBselected = true;
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
            scoreForm.Options               = m_analysis.SMARTOptions;

            if (scoreForm.ShowDialog() == DialogResult.OK)
            {
                
                /// 
                /// Get new options
                /// 
                m_analysis.SMARTOptions = scoreForm.Options;
            }            
        }
        private void LoadParametersFromFileClicked()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load Parameter File";

            if (DialogResult.OK == dialog.ShowDialog())
            {
                m_analysis.LoadParametersFromFile(dialog.FileName);
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
            if (m_analysis == null || m_analysis.Processing == false)
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
            m_isCopyingFiles = false;

            /// 
            /// Show stop analysis.
            /// 
            try
            {                
                m_analysis.Abort();
            }
            catch (ThreadAbortException abortException)
            {
                //pass
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error stopping the analysis. " + ex.Message);
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
            SetStep(CONST_STEP_ANALYZE, m_wizardSteps[CONST_STEP_ANALYZE]);

            // Disable the buttons from the user hitting anything.
            mctl_performAnalysisPage.NextButtonText      = "Stop";
            mctl_performAnalysisPage.BackButtonEnabled   = false;
            mctl_performAnalysisPage.CancelButtonEnabled = false;
            
            m_analysis.UseMassTagDBAsBaseline       = mctl_selectParametersPage.UseMassTagDBAsBaseline;
            m_analysis.UseSMART                     = mctl_selectParametersPage.UseSMART;
            string massTagDBName                    = m_analysis.MassTagDBOptions.mstrDatabase;
            bool alignmentDatabaseInvalid           = false;

            /// 
            /// Check to make sure that the database selected / provided is valid.  
            /// 
            if (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.SQL)
                alignmentDatabaseInvalid = (massTagDBName == null || massTagDBName == "");
            else if (m_analysis.MassTagDBOptions.menm_databaseType == MultiAlignEngine.MassTags.MassTagDatabaseType.ACCESS)
                alignmentDatabaseInvalid = (File.Exists(m_analysis.MassTagDBOptions.mstr_databaseFilePath) == false);

            if (m_analysis.UseMassTagDBAsBaseline && alignmentDatabaseInvalid)
            {
                MessageBox.Show("Please select a mass tag database as you are going to use one as a baseline for alignment.");
                return false;
            }

            if (!m_analysis.UseMassTagDBAsBaseline && m_baselineIndex == -1)
            {
                MessageBox.Show("Please select either a mass tag database or a dataset as a baseline for alignment.");
                return false;
            }

            // Copy all of the data files to the local drive.
            DatasetDownloader downloader = new DatasetDownloader();
            downloader.CopyingComplete  += new EventHandler(downloader_CopyingComplete);
            downloader.FileCopied       += new EventHandler<FileCopyEventArgs>(downloader_FileCopied);
            downloader.CopyFiles(m_datasetInfoList, m_destinationPath);

            return true;
        }            
        /// <summary>
        /// Displays the error message to the user when something bad happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mobjAnalysis_AnalysisException(object sender, ExceptionArgs e)
        {
            MessageBox.Show("Could not complete the analysis. " + e.Exception.Message);
        }              
		#endregion 
        
        #region Download Message Handler
        /// <summary>
        /// Marshals even to user interface thread for copy messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        delegate void DelegateDisplayCopiedData(FileCopyEventArgs e);
        void downloader_FileCopied(object sender, FileCopyEventArgs e)
        {
            if (!InvokeRequired)
            {
                DisplayFileCopiedData(e);
            }
            else
            {
                Invoke(new DelegateDisplayCopiedData (DisplayFileCopiedData), new object[] { e });
            }
        }
        /// <summary>
        /// Updates the display with the file path that was copied.
        /// </summary>
        /// <param name="args"></param>
        private void DisplayFileCopiedData(FileCopyEventArgs args)
        {
            string pathName = Path.GetFileNameWithoutExtension(args.DestinationPath);
            mctl_performAnalysisPage.AddStatusMessage(0, "Copied " + pathName);    
        }
        /// <summary>
        /// Triggers the analysis to continue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloader_CopyingComplete(object sender, EventArgs e)
        {
            if (!InvokeRequired)
            {
                ContinueAnalysis();
            }
            else
            {
                BeginInvoke(new MethodInvoker(ContinueAnalysis));
            }
        }
        /// <summary>
        /// Continues with the rest of the analysis.
        /// </summary>
        private void ContinueAnalysis()
        {
            m_analysis.Datasets.Clear();
            m_analysis.Datasets.AddRange(m_datasetInfoList);

            string outputPath    = mctl_selectOutputNamePage.ProjectOutputFileName;
            string dataPath      = Path.GetDirectoryName(outputPath);
            string analysisName  = Path.GetFileNameWithoutExtension(outputPath); 
            string analysisPath  = Path.Combine(dataPath, analysisName);            
            string logPath       = Path.Combine(analysisPath, analysisName + ".log");            
            string parameterPath = Path.Combine(analysisPath, analysisName + "_parameters.xml");

            if (!Directory.Exists(analysisPath))
            {
                Directory.CreateDirectory(analysisPath);
            }
            
            if (m_baselineIndex > -1)
            {
                m_analysis.BaselineDataset = m_analysis.Datasets[m_baselineIndex].mstrLocalPath;
            }
            else
            {
                // Make sure we have something to align to.
                if (!m_analysis.UseMassTagDBAsBaseline)
                    throw new Exception("The mass tag database (MTDB) was not selected.");

                m_analysis.BaselineDataset = null;
            }

            m_analysis.PathName       = analysisPath;
            m_analysis.AnalysisName   = analysisName;
            m_analysis.SaveParametersToFile(parameterPath);
            AnalysisLogWriter.WriteHeader(logPath, "MultiAlign Analysis " + analysisName + " " + DateTime.Now);

            m_analysis.AnalysisException += new EventHandler<ExceptionArgs>(mobjAnalysis_AnalysisException);
            m_analysis.StartAnalysis();
        }
        #endregion
        
        #region Copying of UMC Files
        /// <summary>
        /// Threaded copy function.
        /// </summary>
        /// <param name="o"></param>
        private void CopyFiles(object o)
        {
            List<DatasetInformation> datasets = o as List<DatasetInformation>;

            string sourcePathAndFileName = null;
            string filePath = null;
            int numFiles = datasets.Count;
            string destination;

            
            /// 
            /// Copy the files to local disk.
            /// 
            for (int i = 0; i < numFiles && m_isCopyingFiles == true; i++)
            {

                sourcePathAndFileName = datasets[i].ArchivePath;

                //int index = sourceLocations[i].LastIndexOf("\\");
                //filePath = sourcePathAndFileName.Substring(index + 1);
                filePath = System.IO.Path.GetFileName(sourcePathAndFileName);


                //TODO: Fix the desintation path as a parameter...this is not thread safe!!!
                destination = Path.Combine(m_destinationPath, filePath);

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
                        m_numCopied++;
                        datasets[i].mstrLocalPath = destination;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "File.Copy");
                    }
                }
                else if (System.IO.File.Exists(destination))
                {
                    datasets[i].mstrLocalPath = destination;
                    m_numCopied++;                    
                }
                m_numReviewedForCopy = i;
            }
            /// 
            /// Tell the listening thread we are done via a flag.
            /// 
            m_isFinishedCopying = true;
        }
		private bool CopyFilesHandler()
		{

            mctl_performAnalysisPage.ClearListBox();
            mctl_performAnalysisPage.AddStatusMessage(0, "Copying files to working folder...");
            mctl_performAnalysisPage.Visible = true;
            mctl_performAnalysisPage.SetProgressBar(0);

            int total = m_datasetInfoList.Count;
   
			/// 
            /// Make sure the user has selected files.
            /// 
			if (total <= 0)
			{
				MessageBox.Show("No files selected/found");
                return false;
			}
			else
			{
                /// 
                /// Set the count to zero.
                /// 
                m_numCopied          = 0;
                m_numReviewedForCopy = 0;
                m_isFinishedCopying   = false;
                m_isCopyingFiles      = true;                


                string lastFileCopied = m_datasetInfoList[0].DatasetName;
                List<string> names    = new List<string>();                
                foreach (DatasetInformation info in m_datasetInfoList)
                {
                    names.Add(info.DatasetName);
                }
                
                /// 
                /// Register the thread
                ///                                 
                ParameterizedThreadStart start = new ParameterizedThreadStart(CopyFiles);
                Thread thread = new Thread(start);
                thread.Start(this.m_datasetInfoList);
                
                /// 
                /// Eh! the poor man's way of waiting for the thread to finish!!!
                /// 
                while (m_isCopyingFiles == true && m_isFinishedCopying == false)
                {                    
                    Thread.Sleep(10);
                    /// 
                    /// Update the user interface.
                    /// 
                    int copiedPercent  = Convert.ToInt32(100.0 * (Convert.ToDouble(m_numCopied) / Convert.ToDouble(total)));                    
                    mctl_performAnalysisPage.SetProgressBar(copiedPercent);

                    string currentCopy = names[m_numReviewedForCopy];
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
                if (m_isCopyingFiles == true)
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
			if (m_numCopied == total)
			{                                
                return true;
			}
			return false;
		}
		#endregion 
	
        #region Properties
		public string [] FileLocations
		{
			get
			{
                m_datasetInfoList = mctl_loadDatasetPage.Datasets;

				string[] fileLocations = new string[m_datasetInfoList.Count] ;
				for (int i = 0 ; i < m_datasetInfoList.Count ; i++)
				{
					fileLocations[i] = m_datasetInfoList[i].mstrLocalPath ;
				}
				return fileLocations ;
			}
		}
		public MultiAlignAnalysis MultiAlignAnalysis
		{
			get
			{
				return m_analysis ; 
			}
        }
        #endregion
    }
}

