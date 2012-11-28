using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using Manassa.Data;
using Manassa.IO;
using Manassa.Workspace;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCustomControls.Drawing;
using MultiAlignCore.IO;

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

            // Controllers etc for running analysis.
            AnalysisState                   = ApplicationAnalysisState.Idle;
            CurrentWorkspace                = new ManassaWorkspace();                       
            m_analysisLoadDialog            = new System.Windows.Forms.OpenFileDialog();
            Controller                      = new AnalysisController();
            Reporter                        = new AnalysisReportGenerator();        
    
            m_performAnalysisControl.AnalysisQuit  += new System.EventHandler(m_performAnalysisControl_AnalysisQuit);
            m_performAnalysisControl.AnalysisStart += new System.EventHandler(m_performAnalysisControl_AnalysisStart);
            m_gettingStarted.RecentAnalysisSelected += new System.EventHandler<Windows.OpenAnalysisArgs>(m_gettingStarted_RecentAnalysisSelected);
            // Bind the status to the status mediators.
            Binding binding = new Binding("Status");
            binding.Source  = ApplicationStatusMediator.Mediator;
            SetBinding(StatusProperty, binding);

            // Update the titles.
            string version  = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title           = version;

            Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);

            LoadWorkspace(Properties.Settings.Default.WorkspaceFile);

            ApplicationStatusMediator.SetStatus("Ready.");
        }

        void m_gettingStarted_RecentAnalysisSelected(object sender, Windows.OpenAnalysisArgs e)
        {
            LoadMultiAlignFile(e.AnalysisData);
            CurrentWorkspace.AddAnalysis(e.AnalysisData);
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

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ManassaWorkspaceWriter writer = new ManassaWorkspaceWriter();
            writer.Write(Properties.Settings.Default.WorkspaceFile, CurrentWorkspace);

        }

        #region Dependency Properties
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
        
        public ApplicationAnalysisState AnalysisState
        {
            get { return (ApplicationAnalysisState)GetValue(AnalysisStateProperty); }
            set { SetValue(AnalysisStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisStateProperty =
            DependencyProperty.Register("AnalysisState", typeof(ApplicationAnalysisState), typeof(MainWindow));

        public ApplicationAnalysisState LastApplicationState
        {
            get { return (ApplicationAnalysisState)GetValue(LastApplicationStateProperty); }
            set { SetValue(LastApplicationStateProperty, value); }
        }

        public AnalysisController Controller
        {
            get { return (AnalysisController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(AnalysisController), typeof(MainWindow));

        

        // Using a DependencyProperty as the backing store for LastApplicationState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastApplicationStateProperty =
            DependencyProperty.Register("LastApplicationState", typeof(ApplicationAnalysisState), typeof(MainWindow));
        #endregion

        #region Loading and Running Analysis 
        /// <summary>
        /// Starts a new analysis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_performAnalysisControl_AnalysisStart(object sender, System.EventArgs e)
        {
            AnalysisState = ApplicationAnalysisState.RunningAnalysis;
           // runningAnalysisControl.CurrentState = AnalysisState;
        }
        /// <summary>
        /// Quits an analysis that is running or is being started new.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_performAnalysisControl_AnalysisQuit(object sender, System.EventArgs e)
        {
            switch (AnalysisState)
            {
                case ApplicationAnalysisState.Idle:
                    break;
                case ApplicationAnalysisState.RunningAnalysis:                    
                    StopCurrentAnalysis();
                    break;
                case ApplicationAnalysisState.SetupAnalysis:
                    break;
                case ApplicationAnalysisState.Opened:
                    break;
                default:
                    break;
            }

            switch (LastApplicationState)
            {
                case ApplicationAnalysisState.Idle:
                    AnalysisState = ApplicationAnalysisState.Idle;
                    break;
                case ApplicationAnalysisState.RunningAnalysis:
                    AnalysisState = ApplicationAnalysisState.Idle;
                    break;
                case ApplicationAnalysisState.SetupAnalysis:
                    AnalysisState = ApplicationAnalysisState.Idle;
                    break;
                case ApplicationAnalysisState.Opened:
                    AnalysisState = LastApplicationState;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Clean up the application state.
        /// </summary>
        private void CleanupMultiAlignController()
        {
            if (Controller != null)
            {
                Controller = null;
            }
        }
        /// <summary>
        /// Clean up the application state.
        /// </summary>
        private void StopCurrentAnalysis()
        {
            //TODO: Do stuff!
            CleanupMultiAlignController();
        }
        private void LoadMultiAlignFile(RecentAnalysis analysis)
        {
            LoadMultiAlignFile(analysis.Path);

            string version = MultiAlignCore.ApplicationUtility.GetEntryAssemblyData();
            Title = string.Format("{0} - {1}", version, analysis.Name);
        }
        private void LoadMultiAlignFile(string filename)
        {
            ApplicationStatusMediator.SetStatus(string.Format("Loading analysis...{0}", filename));
            // Cleanup old ties if necessary.
            CleanupMultiAlignController();
            
            // Create a new controller
            Controller = new AnalysisController();
            Controller.LoadExistingAnalysis(filename, Reporter);
            Controller.Config.Analysis.MetaData.AnalysisPath = filename;

            m_mainControl.IsEnabled = true;

            ApplicationStatusMediator.SetStatus("Analysis loaded.  Creating plots.");
            m_mainControl.Analysis  = Controller.Config.Analysis;
            AnalysisState           = ApplicationAnalysisState.Opened;
            LastApplicationState    = AnalysisState;
        }
        #endregion

        #region Event Handlers
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenExistingAnalysis();
        }
        private void OpenExistingAnalysis()
        {
            System.Windows.Forms.DialogResult result = m_analysisLoadDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                RecentAnalysis newAnalysis = new RecentAnalysis();
                newAnalysis.Name = System.IO.Path.GetFileNameWithoutExtension(m_analysisLoadDialog.FileName);
                newAnalysis.Path = m_analysisLoadDialog.FileName;

                LoadMultiAlignFile(newAnalysis);


                CurrentWorkspace.AddAnalysis(newAnalysis);
            }
        }
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewAnalysis();
        }
        private void StartNewAnalysis()
        {
            ApplicationStatusMediator.SetStatus("Creating new analysis.");

            AnalysisConfig config                           = new AnalysisConfig();
            config.Analysis                                 = new MultiAlignAnalysis();
            config.Analysis.AnalysisType                    = AnalysisType.Full;
            config.Analysis.Options.AlignmentOptions.IsAlignmentBaselineAMasstagDB = false;
            m_performAnalysisControl.AnalysisConfiguration  = config;
            m_performAnalysisControl.CurrentStep            = AnalysisSetupStep.DatasetSelection;
            AnalysisState                                   = ApplicationAnalysisState.SetupAnalysis;                         
        }        
        #endregion

        private void ShowGettingStarted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            AnalysisState = ApplicationAnalysisState.Idle;
        }

        private void New_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            StartNewAnalysis();
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenExistingAnalysis();
        }

        private void CurrentAnalysis(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (m_mainControl.Analysis != null)
            {
                AnalysisState = ApplicationAnalysisState.Opened;
            }
        }

        private void MoveToLastApplicationState(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            AnalysisState = LastApplicationState;
        }
    }
}
