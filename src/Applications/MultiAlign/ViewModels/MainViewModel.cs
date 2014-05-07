using System;
using System.IO;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.Data.States;
using MultiAlign.ViewModels.Viewers;
using MultiAlign.ViewModels.Wizard;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using PNNLOmics.Annotations;

namespace MultiAlign.ViewModels
{
    /// <summary>
    /// Does nothing right now...needs to replace the code behind from the main window
    /// </summary>
    public sealed class MainViewModel: ViewModelBase
    {
        private string m_status;
        private string m_title;        
        private AnalysisViewModel           m_currentAnalysis;
        private AnalysisSetupViewModel m_analysisSetupViewModel;
        private AnalysisRunningViewModel m_analysisRunningViewModel;

        public MainViewModel()
        {
            // Create the state moderation (between views)
            BuildStateModerator();

            MainDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            MainDataName      = "analysis";



            // Titles and Status
            var version  = ApplicationUtility.GetEntryAssemblyData();
            Title           = version;
            
            // Command Setup
            ShowAnalysisCommand = new BaseCommand(ShowAnalysis, BaseCommand.AlwaysPass);
            ShowStartCommand    = new BaseCommand(ShowStart, BaseCommand.AlwaysPass);

            // View Models
            
            var workSpacePath    = ApplicationUtility.GetApplicationDataFolderPath("MultiAlign");
            workSpacePath           = Path.Combine(workSpacePath, Properties.Settings.Default.WorkspaceFile);
            GettingStartedViewModel = new GettingStartedViewModel(workSpacePath, StateModerator);

            GettingStartedViewModel.NewAnalysisStarted += GettingStartedViewModel_NewAnalysisStarted;
            GettingStartedViewModel.ExistingAnalysisSelected += GettingStartedViewModel_ExistingAnalysisSelected;

            AnalysisRunningViewModel = new AnalysisRunningViewModel();            
            AnalysisRunningViewModel.AnalysisCancelled += AnalysisRunningViewModel_AnalysisCancelled;
            AnalysisRunningViewModel.AnalysisComplete  += AnalysisRunningViewModel_AnalysisComplete;

            LoadingAnalysisViewModel = new AnalysisLoadingViewModel();
            LoadingAnalysisViewModel.AnalysisLoaded += LoadingAnalysisViewModel_AnalysisLoaded;

            ApplicationStatusMediator.SetStatus("Ready.");
            
        }


        #region Command Delegate Method Handlers

        private void ShowAnalysis()
        {
            ShowLoadedAnalysis();
        }

        private void ShowStart()
        {
            ShowHomeScreen();
        }
        #endregion

        #region Commands
        /// <summary>
        /// Gets the showing of an analysis 
        /// </summary>        
        [UsedImplicitly]  
        public ICommand ShowAnalysisCommand { get; private set; }
        /// <summary>
        /// Gets the showing of a new analysis
        /// </summary>        
        [UsedImplicitly]  
        public ICommand ShowStartCommand { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current analysis.
        /// </summary>
        [UsedImplicitly]  
        public AnalysisViewModel CurrentAnalysis
        {
            get
            {
                return m_currentAnalysis;
            }
            set
            {
                if (m_currentAnalysis == value) return;
                m_currentAnalysis = value;
                OnPropertyChanged("CurrentAnalysis");
            }
        }
        /// <summary>
        /// Gets or sets the title of the window
        /// </summary>
        [UsedImplicitly]  
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                if (m_title == value) return;
                m_title = value;
                OnPropertyChanged("Title");
            }
        }
        /// <summary>
        /// Gets or sets the status 
        /// </summary>
        [UsedImplicitly]
        public string Status
        {
            get
            {
                return m_status;
            }
            set
            {
                if (m_status == value) return;
                m_status = value;
                OnPropertyChanged("Status");
            }
        }
        #endregion

        #region View Models
        /// <summary>
        /// Gets or sets the view model for displaying the home screen.
        /// </summary>

        [UsedImplicitly]
        public GettingStartedViewModel GettingStartedViewModel
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the view model for creating a new analysis.
        /// </summary>
        [UsedImplicitly]  
        public AnalysisSetupViewModel AnalysisSetupViewModel
        {
            get
            {
                return m_analysisSetupViewModel;
            }
            set
            {
                if (m_analysisSetupViewModel == value) return;
                m_analysisSetupViewModel = value;
                OnPropertyChanged("AnalysisSetupViewModel");
            }
        }
        /// <summary>
        /// Gets or sets the analysis running view model
        /// </summary>
        [UsedImplicitly]  
        public AnalysisRunningViewModel AnalysisRunningViewModel
        {
            get
            {
                return m_analysisRunningViewModel;
            }
            set
            {
                if (m_analysisRunningViewModel == value) return;
                m_analysisRunningViewModel = value;
                OnPropertyChanged("AnalysisRunningViewModel");
            }
        }
        /// <summary>
        /// Gets or sets the state moderator.
        /// </summary>
        [UsedImplicitly]  
        public StateModeratorViewModel StateModerator
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or set the view model for loading an analysis.
        /// </summary>
        [UsedImplicitly]  
        public AnalysisLoadingViewModel LoadingAnalysisViewModel { get; set; }
        #endregion


        #region Loading      

        /// <summary>
        /// Loads a recent analysis
        /// </summary>
        /// <param name="recentAnalysis"></param>
        private void LoadAnalysis(RecentAnalysis recentAnalysis)
        {
            string message;
            if (StateModerator.IsAnalysisRunning(out message))
            {
                Status = "Cannot open a new analysis while one is running.";
                return;
            }

            var filename = Path.Combine(recentAnalysis.Path, recentAnalysis.Name);
            if (!File.Exists(filename))
            {
                StateModerator.CurrentViewState = ViewState.HomeView;
                Status = "The analysis file does not exist";
                return;
            }

            // Show the open view
            StateModerator.CurrentViewState = ViewState.OpenView;
            LoadingAnalysisViewModel.LoadAnalysis(recentAnalysis);

        }
        /// <summary>
        /// Handles when an analysis has been loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoadingAnalysisViewModel_AnalysisLoaded(object sender, AnalysisStatusArgs e)
        {
            DisplayAnalysis(e.Analysis);
        }
        private void DisplayAnalysis(MultiAlignAnalysis analysis)
        {
            // Change the title
            var version = ApplicationUtility.GetEntryAssemblyData();
            Title = string.Format("{0} - {1}", version, analysis.MetaData.AnalysisName);

            var model                               = new AnalysisViewModel(analysis);            
            CurrentAnalysis                         = model;
            StateModerator.CurrentViewState         = ViewState.AnalysisView;
            var recent                              = new RecentAnalysis(   analysis.MetaData.AnalysisPath,
                                                                            analysis.MetaData.AnalysisName);
            GettingStartedViewModel.CurrentWorkspace.AddAnalysis(recent);
        }
        #endregion

        #region Display

        /// <summary>
        /// Shows the new analysis setup
        /// </summary>
        private void ShowNewAnalysisSetup()
        {
            string message;
            var canStart    = StateModerator.CanPerformNewAnalysis(out message);
            Status          = message;
            if (!canStart)
            {              
                return ;
            }

            ApplicationStatusMediator.SetStatus("Creating new analysis.");

            StateModerator.CurrentViewState                 = ViewState.SetupAnalysisView;
            StateModerator.CurrentAnalysisState             = AnalysisState.Setup;
            
            var config                           = new AnalysisConfig
            {
                Analysis = new MultiAlignAnalysis(),
                AnalysisPath = MainDataDirectory,
                AnalysisName = MainDataName
            };
            config.Analysis.AnalysisType                    = AnalysisType.Full;
            config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;

            AnalysisSetupViewModel                = new AnalysisSetupViewModel(config);
            AnalysisSetupViewModel.AnalysisQuit  += AnalysisSetupViewModel_AnalysisQuit;
            AnalysisSetupViewModel.AnalysisStart += AnalysisSetupViewModel_AnalysisStart;
            AnalysisSetupViewModel.CurrentStep    = AnalysisSetupStep.DatasetSelection;
        }
        /// <summary>
        /// Displays the loaded analysis 
        /// </summary>
        private void ShowLoadedAnalysis()
        {
            string message;
            var isRunning = StateModerator.IsAnalysisRunning(out message);

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
        /// Shows the home screen.
        /// </summary>
        private void ShowHomeScreen()
        {
            StateModerator.CurrentViewState = ViewState.HomeView;
        }

        /// <summary>
        /// Cancels the analysis setup
        /// </summary>
        private void CancelAnalysisSetup()
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
        }

        #endregion

        #region Settings
        [UsedImplicitly]
        public string MainDataDirectory { get; set; }
        [UsedImplicitly]  
        public string MainDataName { get; set; }
        #endregion

        #region Application State
        /// <summary>
        /// Constructs the transitions for the user interface
        /// </summary>
        private void BuildStateModerator()
        {
            StateModerator = new StateModeratorViewModel
            {
                CurrentAnalysisState = AnalysisState.Idle,
                CurrentViewState = ViewState.HomeView,
                PreviousAnalysisState = AnalysisState.Idle,
                PreviousViewState = ViewState.HomeView
            };
        }
        #endregion
        
        #region Getting Started View Model Event Handlers
        /// <summary>
        /// Loads an existing analysis file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GettingStartedViewModel_ExistingAnalysisSelected(object sender, OpenAnalysisArgs e)
        {
            LoadAnalysis(e.AnalysisData);
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

        #region Analysis Running View Model Events
        /// <summary>
        /// Adds the finished analysis back into the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnalysisRunningViewModel_AnalysisComplete(object sender, AnalysisStatusArgs e)
        {
            var path                         = e.Configuration.AnalysisPath;
            var name                         = Path.GetFileName(e.Configuration.AnalysisName);
            var recent               = new RecentAnalysis(path, name);
            StateModerator.CurrentViewState     = ViewState.AnalysisView;
            StateModerator.CurrentAnalysisState = AnalysisState.Viewing;

            GettingStartedViewModel.AddAnalysis(recent);            
            DisplayAnalysis(e.Configuration.Analysis);
        }
        void AnalysisRunningViewModel_AnalysisCancelled(object sender, EventArgs e)
        {
            StateModerator.CurrentViewState     = ViewState.HomeView;
            StateModerator.CurrentAnalysisState = AnalysisState.Idle;
        }
        #endregion

        #region Aanlysis Setup View Model Events
        /// <summary>
        /// Starts the analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnalysisSetupViewModel_AnalysisStart(object sender, EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Running;
            StateModerator.CurrentViewState     = ViewState.RunningAnalysisView;            
            AnalysisRunningViewModel.Start(AnalysisSetupViewModel.AnalysisConfiguration);
        }
        /// <summary>
        /// Cancels the analysis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AnalysisSetupViewModel_AnalysisQuit(object sender, EventArgs e)
        {
            CancelAnalysisSetup();
        }
        #endregion        
    }
}
