using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiAlign.Data;
using MultiAlign.Data.States;
using MultiAlign.IO;
using MultiAlign.ViewModels.Analysis;
using MultiAlign.ViewModels.TreeView;
using MultiAlign.Windows.Viewers;
using MultiAlign.Workspace;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO;
using MultiAlignCustomControls.Drawing;
using System.Windows;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.ViewModels.Wizard;

namespace MultiAlign.ViewModels
{
    /// <summary>
    /// Does nothing right now...needs to replace the code behind from the main window
    /// </summary>
    public class MainViewModel: ViewModelBase
    {
        private string m_status;
        private string m_title;
        private MultiAlignWorkspace     m_workspace;
        private AnalysisSetupViewModel  m_analysisSetupViewModel;
        private ViewModels.AnalysisViewModel m_currentAnalysis;
        private Wizard.AnalysisRunningViewModel m_analysisRunningViewModel;

        public MainViewModel()
        {
            // Create the state moderation (between views)
            BuildStateModerator();
                        
            // Create a controller for an analysis...
            Controller              = new AnalysisController();
            Reporter                = new AnalysisReportGenerator();
            
            // Load the workspace...
            CurrentWorkspace        = new MultiAlignWorkspace();
            string workSpacePath    = ApplicationUtility.GetApplicationDataFolderPath("MultiAlign");
            workSpacePath           = Path.Combine(workSpacePath, Properties.Settings.Default.WorkspaceFile);
            LoadWorkspace(workSpacePath);

            // Update the titles.
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = version;

            // View Models
            GettingStartedViewModel = new GettingStartedViewModel(m_workspace, StateModerator);
            GettingStartedViewModel.NewAnalysisStarted          += new EventHandler<OpenAnalysisArgs>(GettingStartedViewModel_NewAnalysisStarted);
            GettingStartedViewModel.ExistingAnalysisSelected    += new EventHandler<OpenAnalysisArgs>(GettingStartedViewModel_ExistingAnalysisSelected);


            ShowAnalysisCommand = new BaseCommandBridge(new CommandDelegate(ShowAnalysis));
            ShowStartCommand    = new BaseCommandBridge(new CommandDelegate(ShowStart));

            AnalysisRunningViewModel = new AnalysisRunningViewModel();
            
            
            ApplicationStatusMediator.SetStatus("Ready.");
        }

        public void ShowAnalysis(object parameter)
        {
            ShowLoadedAnalysis();
        }
        public void ShowStart(object parameter)
        {
            ShowHomeScreen();
        }

        public ICommand ShowAnalysisCommand { get; set; }
        public ICommand ShowStartCommand { get; set; }


        #region Getting Started Event Handlers
        /// <summary>
        /// Loads an existing analysis file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GettingStartedViewModel_ExistingAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            LoadMultiAlignFile(e.AnalysisData);
        }
        /// <summary>
        /// Starts a new analysis 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GettingStartedViewModel_NewAnalysisStarted(object sender, OpenAnalysisArgs e)
        {
            ShowNewAnalysisSetup();
        }
        #endregion

        #region Properties
        public IAnalysisReportGenerator Reporter
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the controller 
        /// </summary>
        private AnalysisController Controller
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the current analysis.
        /// </summary>
        public AnalysisViewModel CurrentAnalysis
        {
            get
            {
                return m_currentAnalysis;
            }
            set
            {
                if (m_currentAnalysis != value)
                {
                    m_currentAnalysis = value;
                    OnPropertyChanged("CurrentAnalysis");
                }
            }
        }
        /// <summary>
        /// Gets or sets the current workspace.
        /// </summary>
        public MultiAlignWorkspace CurrentWorkspace
        {
            get
            {
                return m_workspace;
            }
            set
            {
                if (m_workspace != value)
                {
                    m_workspace = value;
                    OnPropertyChanged("CurrentWorkspace");
                }
            }
        }
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                if (m_title != value)
                {
                    m_title = value;
                    OnPropertyChanged("Title");
                }
            }
        }
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                if (m_status != value)
                {
                    m_status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        #endregion

        #region View Models
        /// <summary>
        /// Gets or sets the view model for displaying the home screen.
        /// </summary>
        public GettingStartedViewModel GettingStartedViewModel
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the view model for creating a new analysis.
        /// </summary>
        public AnalysisSetupViewModel AnalysisSetupViewModel
        {
            get
            {
                return m_analysisSetupViewModel;
            }
            set
            {
                if (m_analysisSetupViewModel != value)
                {
                    m_analysisSetupViewModel = value;
                    OnPropertyChanged("AnalysisSetupViewModel");
                }
            }
        }
        public AnalysisRunningViewModel AnalysisRunningViewModel
        {
            get
            {
                return m_analysisRunningViewModel;
            }
            set
            {
                if (m_analysisRunningViewModel != value)
                {
                    m_analysisRunningViewModel = value;
                    OnPropertyChanged("AnalysisRunningViewModel");
                }
            }
        }
        /// <summary>
        /// Gets or sets the state moderator.
        /// </summary>
        public StateModeratorViewModel StateModerator
        {
            get;
            set;
        }
        #endregion

        #region Analysis State Commands
        /// <summary>
        /// Cancels the current running analysis.
        /// </summary>
        private bool CancelAnalysis()
        {
            if (Controller != null)
            {
                Controller.CancelAnalysis();
                Controller = null;
            }
            return true;
        }
        private bool ConfirmCancel()
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(   "Performing this action will cancel the running analysis.  Do you want to cancel?", 
                                                                                       "Cancel Analysis", 
                                                                                       MessageBoxButton.YesNo);
            return (result == System.Windows.MessageBoxResult.Yes);
        }
        #endregion

        #region UI Controls
        private void LoadMultiAlignFile(RecentAnalysis analysis)
        {
            if (analysis == null)
            {
                Status = "Cannot open analysis file.";
                return;
            }
            
            string filename = System.IO.Path.Combine(analysis.Path, analysis.Name);
            if (!File.Exists(filename))
            {
                StateModerator.CurrentViewState = ViewState.HomeView;
                Status = "The analysis file does not exist";
                return;
            }

            // Show the open view
            StateModerator.CurrentViewState = ViewState.OpenView;
            System.Windows.Forms.Application.DoEvents();

            // Change the title
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = string.Format("{0} - {1}", version, analysis.Name);

            ApplicationStatusMediator.SetStatus(string.Format("Loading analysis...{0}", filename));
            CancelAnalysis();

            // Create a controller...needs to probably be refactored to not be coupled to controller...
            Controller = new AnalysisController();
            Controller.LoadExistingAnalysis(filename, Reporter);
            Controller.Config.Analysis.MetaData.AnalysisPath = analysis.Path;
            Controller.Config.Analysis.MetaData.AnalysisName = analysis.Name;


            ApplicationStatusMediator.SetStatus("Analysis Loaded.");
            AnalysisViewModel model             = new AnalysisViewModel(Controller.Config.Analysis);
            model.ClusterTree.ClustersFiltered += new EventHandler<ClustersUpdatedEventArgs>(ClusterTree_ClustersFiltered);

            //m_mainControl.DataContext = model;
            UpdateAllClustersPlot(model.ClusterTree.FilteredClusters, true);

            StateModerator.CurrentViewState = ViewState.AnalysisView;
            System.Windows.Forms.Application.DoEvents();

            CurrentAnalysis = model;
        }        
        void ClusterTree_ClustersFiltered(object sender, ClustersUpdatedEventArgs e)
        {
            UpdateAllClustersPlot(e.Clusters, false);
        }
        void UpdateAllClustersPlot(ObservableCollection<UMCClusterTreeViewModel> clusters, bool autoViewport)
        {
            List<UMCClusterLightMatched> filteredClusters = (from cluster in clusters
                                                                select cluster.Cluster).ToList();

            //m_mainControl.SetClusters(filteredClusters, autoViewport);
        }

        /// <summary>
        /// Opens an existing analysis 
        /// </summary>
        private void OpenExistingAnalysis()
        {
            
        }
        /// <summary>
        /// Shows the new analysis setup
        /// </summary>
        private void ShowNewAnalysisSetup()
        {
            string message = "";
            bool canStart  = StateModerator.CanPerformNewAnalysis(ref message);
            Status         = message;
            if (!canStart)
            {              
                return ;
            }

            ApplicationStatusMediator.SetStatus("Creating new analysis.");

            StateModerator.CurrentViewState                 = ViewState.SetupAnalysisView;
            StateModerator.CurrentAnalysisState             = AnalysisState.Setup;
            
            AnalysisConfig config                           = new AnalysisConfig();
            config.Analysis                                 = new MultiAlignAnalysis();
            config.Analysis.AnalysisType                    = AnalysisType.Full;
            config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;

            AnalysisSetupViewModel                          = new Analysis.AnalysisSetupViewModel(config);
            AnalysisSetupViewModel.CurrentStep              = AnalysisSetupStep.DatasetSelection;            
        }
        /// <summary>
        /// Displays the loaded analysis 
        /// </summary>
        private void ShowLoadedAnalysis()
        {
            string message = "";
            bool isRunning = StateModerator.IsAnalysisRunning(ref message);

            if (isRunning)
            {
                StateModerator.CurrentViewState = ViewState.RunningAnalysisView;
            }
            else
            {                
                StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
                StateModerator.CurrentViewState     = ViewState.AnalysisView;                                    
            }
        }
        /// <summary>
        /// Cancels the analysis setup
        /// </summary>
        private bool CancelAnalysisSetup()
        {
            // If we were looking at an analysis before, then go back to it.
            if (StateModerator.PreviousViewState == ViewState.AnalysisView)
            {
                StateModerator.CurrentViewState     = ViewState.AnalysisView;
                StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
            }
            else
            {
                StateModerator.CurrentViewState     = ViewState.HomeView;
                StateModerator.CurrentAnalysisState = AnalysisState.Idle;
            }
            return true;
        }
        /// <summary>
        /// Shows the home screen.
        /// </summary>
        private bool ShowHomeScreen()
        {
            StateModerator.CurrentViewState = ViewState.HomeView;
            return true;
        }
        #endregion
        
        #region Application Setup
        /// <summary>
        /// Constructs the transitions for the user interface
        /// </summary>
        private void BuildStateModerator()
        {
            StateModerator = new StateModeratorViewModel();
            StateModerator.CurrentAnalysisState     = AnalysisState.Idle;
            StateModerator.CurrentViewState         = ViewState.HomeView;
            StateModerator.PreviousAnalysisState    = AnalysisState.Idle;
            StateModerator.PreviousViewState        = ViewState.HomeView;            
        }
        /// <summary>
        /// Loads teh current workspace.
        /// </summary>
        private void LoadWorkspace(string path)
        {
            if (System.IO.File.Exists(path))
            {
                ApplicationStatusMediator.SetStatus("Loading workspace");
                MultiAlignWorkspaceReader reader = new MultiAlignWorkspaceReader();
                try
                {
                    CurrentWorkspace = reader.Read(path);
                }
                catch
                {
                    ApplicationStatusMediator.SetStatus(string.Format("Could not load the default workspace: {0}"));
                }
            }
        }
        #endregion

        #region Window And User Control Event Handlers
        void runningAnalysisControl_AnalysisComplete(object sender, System.EventArgs e)
        {
            string path             = Controller.Config.AnalysisPath;
            string name             = System.IO.Path.GetFileName(Controller.Config.AnalysisName);
            RecentAnalysis analysis = new RecentAnalysis(path, name); 
           
            StateModerator.CurrentViewState = ViewState.OpenView;
            System.Windows.Forms.Application.DoEvents();

            //m_mainControl.DataContext = new AnalysisViewModel(Controller.Config.Analysis);

            StateModerator.CurrentViewState     = ViewState.AnalysisView;
            StateModerator.CurrentAnalysisState = AnalysisState.Viewing;

            CurrentWorkspace.AddAnalysis(analysis);
        }
        void runningAnalysisControl_AnalysisCancelled(object sender, System.EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Idle;
            StateModerator.CurrentViewState = ViewState.HomeView;
        }
        void m_gettingStarted_RecentAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            LoadMultiAlignFile(e.AnalysisData);
            CurrentWorkspace.AddAnalysis(e.AnalysisData);
        }
        /// <summary>
        /// Starts a new analysis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_performAnalysisControl_AnalysisStart(object sender, System.EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Running;
            StateModerator.CurrentViewState     = ViewState.RunningAnalysisView;
            Controller.Config                   = this.AnalysisSetupViewModel.AnalysisConfiguration;            
        }
        /// <summary>
        /// Quits an analysis that is running or is being started new.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_performAnalysisControl_AnalysisQuit(object sender, System.EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Idle;
            StateModerator.CurrentViewState = ViewState.HomeView;
        }
        /// <summary>
        /// Handles when the main window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MultiAlignWorkspaceWriter writer = new MultiAlignWorkspaceWriter();
            try
            {
                string workspacePath = ApplicationUtility.GetApplicationDataFolderPath("MultiAlign");
                if (workspacePath != null)
                {
                    workspacePath = Path.Combine(workspacePath, Properties.Settings.Default.WorkspaceFile);
                    writer.Write(workspacePath, CurrentWorkspace);
                }
            }
            catch
            {
            }


            try
            {
                Controller.CancelAnalysis();
            }
            catch
            {
            }
        }
        #endregion

        

    }
}
