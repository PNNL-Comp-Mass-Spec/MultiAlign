using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using MultiAlign.Data;
using MultiAlign.IO;
using MultiAlign.Workspace;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO;
using System;
using System.IO;
using MultiAlignCore;
using MultiAlign.Data.States;
using MultiAlign.Windows.Viewers;
using MultiAlign.ViewModels;

namespace MultiAlign.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.OpenFileDialog m_analysisLoadDialog;        
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            BuildStateModerator();

            CurrentWorkspace                           = new MultiAlignWorkspace();                       
            m_analysisLoadDialog                       = new System.Windows.Forms.OpenFileDialog();
            Controller                                 = new AnalysisController();
            Reporter                                   = new AnalysisReportGenerator();

            RunningAnalysisControl.AnalysisCancelled  += new System.EventHandler(runningAnalysisControl_AnalysisCancelled);
            RunningAnalysisControl.AnalysisComplete   += new System.EventHandler(runningAnalysisControl_AnalysisComplete);
            PerformAnalysisControl.AnalysisQuit     += new System.EventHandler(m_performAnalysisControl_AnalysisQuit);
            PerformAnalysisControl.AnalysisStart    += new System.EventHandler(m_performAnalysisControl_AnalysisStart);
            GettingStartedControl.RecentAnalysisSelected += new System.EventHandler<OpenAnalysisArgs>(m_gettingStarted_RecentAnalysisSelected);

            // Bind the status to the status mediators.
            Binding binding = new Binding("Status");
            binding.Source  = ApplicationStatusMediator.Mediator;
            binding.Mode    = BindingMode.TwoWay; 
            SetBinding(StatusProperty, binding);

            // Update the titles.
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = version;

            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);


            string workSpacePath    = ApplicationUtility.GetApplicationDataFolderPath("MultiAlign");
            workSpacePath           = Path.Combine(workSpacePath, Properties.Settings.Default.WorkspaceFile);
            LoadWorkspace(workSpacePath);

            ApplicationStatusMediator.SetStatus("Ready.");
        }

        #region Application Setup
        /// <summary>
        /// Constructs the transitions for the user interface
        /// </summary>
        private void BuildStateModerator()
        {
            StateModerator = new StateModerator();

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

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the state transition moderator so we can control UI business logic.
        /// </summary>
        public StateModerator StateModerator
        {
            get { return (StateModerator)GetValue(StateModeratorProperty); }
            set { SetValue(StateModeratorProperty, value); }
        }
        // Using a DependencyProperty as the backing store for StateModerator.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateModeratorProperty =
            DependencyProperty.Register("StateModerator", typeof(StateModerator), typeof(MainWindow));
        public IAnalysisReportGenerator Reporter
        {
            get { return (IAnalysisReportGenerator)GetValue(ReporterProperty); }
            set { SetValue(ReporterProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Reporter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReporterProperty =
            DependencyProperty.Register("Reporter", typeof(IAnalysisReportGenerator), typeof(MainWindow));        
        /// <summary>
        /// Gets or sets the current work space item
        /// </summary>        
        public MultiAlignWorkspace CurrentWorkspace
        {
            get { return (MultiAlignWorkspace)GetValue(CurrentWorkSpaceProperty); }
            set { SetValue(CurrentWorkSpaceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CurrentWorkSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentWorkSpaceProperty =
            DependencyProperty.Register("CurrentWorkspace", typeof(MultiAlignWorkspace), typeof(MainWindow));
        /// <summary>
        /// Gets or sets the status message to display.
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(MainWindow));                
        public AnalysisController Controller
        {
            get { return (AnalysisController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(AnalysisController), typeof(MainWindow));        
        #endregion

        #region Analysis State Commands
        /// <summary>
        /// Cancesl the current running analysis.
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
            MessageBoxResult result = MessageBox.Show("Performing this action will cancel the running analysis.  Do you want to cancel?", "Cancel Analysis", MessageBoxButton.YesNo);
            return (result == MessageBoxResult.Yes);            
        }
        private void LoadMultiAlignFile(RecentAnalysis analysis)
        {
            if (analysis == null)
            {
                Status = "Cannot open analysis file.";
                return;
            }

            StateModerator.CurrentViewState = ViewState.OpenView; 
            System.Windows.Forms.Application.DoEvents();
            
            
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = string.Format("{0} - {1}", version, analysis.Name);
            string filename = System.IO.Path.Combine(analysis.Path, analysis.Name);

            if (!File.Exists(filename))
            {
                Status = "The analysis file does not exist";
                return;
            }

            ApplicationStatusMediator.SetStatus(string.Format("Loading analysis...{0}", filename));                        
            CancelAnalysis();

            Controller                                       = new AnalysisController();
            Controller.LoadExistingAnalysis(filename, Reporter);
            Controller.Config.Analysis.MetaData.AnalysisPath = analysis.Path;
            Controller.Config.Analysis.MetaData.AnalysisName = analysis.Name;

            ApplicationStatusMediator.SetStatus(".");
            AnalysisViewModel model     = new AnalysisViewModel(Controller.Config.Analysis);
            m_mainControl.DataContext   = model;


            StateModerator.CurrentViewState = ViewState.AnalysisView;
            System.Windows.Forms.Application.DoEvents();
        }
        /// <summary>
        /// Opens an existing analysis 
        /// </summary>
        private void OpenExistingAnalysis()
        {
            string message      = "";
            bool canStart       = StateModerator.CanOpenAnalysis(ref message);
            Status              = message;
            if (!canStart)
            {
                // Display a message saying whether we want to cancel or not.                
                return;
            }

            System.Windows.Forms.DialogResult result = m_analysisLoadDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                StateModerator.CurrentViewState     = ViewState.OpenView;
                StateModerator.CurrentAnalysisState = AnalysisState.Loading;
                
                RecentAnalysis newAnalysis  = new RecentAnalysis();
                newAnalysis.Name            = System.IO.Path.GetFileName(m_analysisLoadDialog.FileName);
                newAnalysis.Path            = System.IO.Path.GetDirectoryName(m_analysisLoadDialog.FileName);
                LoadMultiAlignFile(newAnalysis);

                newAnalysis.Name            = System.IO.Path.GetFileName(m_analysisLoadDialog.FileName);

                StateModerator.CurrentViewState     = ViewState.AnalysisView;
                StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
                CurrentWorkspace.AddAnalysis(newAnalysis);                                
            }
        }
        private void ShowNewAnalysisSetup()
        {
            string message = "";
            bool canStart  = StateModerator.CanPerformNewAnalysis(ref message);
            Status         = message;
            if (!canStart)
            {
                // Display a message saying whether we want to cancel or not.                
                return ;
            }

            ApplicationStatusMediator.SetStatus("Creating new analysis.");

            StateModerator.CurrentViewState                 = ViewState.SetupAnalysisView;
            StateModerator.CurrentAnalysisState             = AnalysisState.Setup;
            
            AnalysisConfig config                           = new AnalysisConfig();
            config.Analysis                                 = new MultiAlignAnalysis();
            config.Analysis.AnalysisType                    = AnalysisType.Full;
            config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;

            PerformAnalysisControl.AnalysisConfiguration    = config;
            PerformAnalysisControl.CurrentStep              = AnalysisSetupStep.DatasetSelection;            
        }
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
                //if (m_mainControl.Analysis != null)
                {
                    StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
                    StateModerator.CurrentViewState     = ViewState.AnalysisView;                    
                }
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

        #region Window And User Control Event Handlers
        void runningAnalysisControl_AnalysisComplete(object sender, System.EventArgs e)
        {
            RecentAnalysis analysis = new RecentAnalysis();
            analysis.Path = Controller.Config.AnalysisPath;
            analysis.Name = System.IO.Path.GetFileName(Controller.Config.AnalysisName);
            StateModerator.CurrentViewState = ViewState.OpenView;
            System.Windows.Forms.Application.DoEvents();
           
            m_mainControl.DataContext = new AnalysisViewModel(Controller.Config.Analysis);

            StateModerator.CurrentViewState     = ViewState.AnalysisView;
            StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
            
                
            CurrentWorkspace.AddAnalysis(analysis);   
        }
        void runningAnalysisControl_AnalysisCancelled(object sender, System.EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Idle;
            StateModerator.CurrentViewState     = ViewState.HomeView;            
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
            Controller.Config                   = PerformAnalysisControl.AnalysisConfiguration;
            
            RunningAnalysisControl.Controller   = Controller;
            RunningAnalysisControl.Start(PerformAnalysisControl.AnalysisConfiguration);
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
        
        #region UI Command Bindings
        private void ShowGettingStarted_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ShowHomeScreen();
        }
        private void CurrentAnalysis_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ShowLoadedAnalysis();
        }
        private void NewAnalysis_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ShowNewAnalysisSetup();
        }
        private void OpenExisting_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenExistingAnalysis();
        }
        private void OpenRecent_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            LoadMultiAlignFile(e.Parameter as RecentAnalysis );
        }
        #endregion        
    }
}
