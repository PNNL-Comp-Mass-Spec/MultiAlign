﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Manassa.Data;
using Manassa.IO;
using Manassa.Workspace;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO;
using System;

namespace Manassa
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

            CurrentWorkspace                           = new ManassaWorkspace();                       
            m_analysisLoadDialog                       = new System.Windows.Forms.OpenFileDialog();
            Controller                                 = new AnalysisController();
            Reporter                                   = new AnalysisReportGenerator();

            RunningAnalysisControl.AnalysisCancelled  += new System.EventHandler(runningAnalysisControl_AnalysisCancelled);
            RunningAnalysisControl.AnalysisComplete   += new System.EventHandler(runningAnalysisControl_AnalysisComplete);
            PerformAnalysisControl.AnalysisQuit     += new System.EventHandler(m_performAnalysisControl_AnalysisQuit);
            PerformAnalysisControl.AnalysisStart    += new System.EventHandler(m_performAnalysisControl_AnalysisStart);
            GettingStartedControl.RecentAnalysisSelected += new System.EventHandler<Windows.OpenAnalysisArgs>(m_gettingStarted_RecentAnalysisSelected);

            // Bind the status to the status mediators.
            Binding binding = new Binding("Status");
            binding.Source  = ApplicationStatusMediator.Mediator;
            binding.Mode    = BindingMode.TwoWay;
            SetBinding(StatusProperty, binding);

            // Update the titles.
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = version;

            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            LoadWorkspace(Properties.Settings.Default.WorkspaceFile);
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
                ManassaWorkspaceReader reader = new ManassaWorkspaceReader();
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
        public ManassaWorkspace CurrentWorkspace
        {
            get { return (ManassaWorkspace)GetValue(CurrentWorkSpaceProperty); }
            set { SetValue(CurrentWorkSpaceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CurrentWorkSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentWorkSpaceProperty =
            DependencyProperty.Register("CurrentWorkspace", typeof(ManassaWorkspace), typeof(MainWindow));
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
            LoadMultiAlignFile(analysis.Path);

            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = string.Format("{0} - {1}", version, analysis.Name);
        }
        private void LoadMultiAlignFile(string filename)
        {
            ApplicationStatusMediator.SetStatus(string.Format("Loading analysis...{0}", filename));                        
            CancelAnalysis();

            Controller                                       = new AnalysisController();
            Controller.LoadExistingAnalysis(filename, Reporter);
            Controller.Config.Analysis.MetaData.AnalysisPath = filename;

            ApplicationStatusMediator.SetStatus("Analysis loaded.  Creating plots.");
            m_mainControl.Analysis = Controller.Config.Analysis;
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
                newAnalysis.Name            = System.IO.Path.GetFileNameWithoutExtension(m_analysisLoadDialog.FileName);
                newAnalysis.Path            = m_analysisLoadDialog.FileName;
                LoadMultiAlignFile(newAnalysis);


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
                if (m_mainControl.Analysis != null)
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
            analysis.Name = Controller.Config.AnalysisName;

            m_mainControl.Analysis = Controller.Config.Analysis;

            StateModerator.CurrentViewState     = ViewState.AnalysisView;
            StateModerator.CurrentAnalysisState = AnalysisState.Viewing;
                            
            CurrentWorkspace.AddAnalysis(analysis);   
        }
        void runningAnalysisControl_AnalysisCancelled(object sender, System.EventArgs e)
        {
            StateModerator.CurrentAnalysisState = AnalysisState.Idle;
            StateModerator.CurrentViewState     = ViewState.HomeView;            
        }
        void m_gettingStarted_RecentAnalysisSelected(object sender, Windows.OpenAnalysisArgs e)
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
            ManassaWorkspaceWriter writer = new ManassaWorkspaceWriter();
            writer.Write(Properties.Settings.Default.WorkspaceFile, CurrentWorkspace);

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
        #endregion        
    }
}