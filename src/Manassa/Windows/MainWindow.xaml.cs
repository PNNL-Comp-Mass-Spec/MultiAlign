using System.Collections.ObjectModel;
using System.Windows;
using Manassa.Data;
using Manassa.Windows;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCustomControls.Drawing;
using System.Windows.Data;

namespace Manassa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.OpenFileDialog m_analysisLoadDialog;
        /// <summary>
        /// Object that controls the setup of an analysis and processing.
        /// </summary>
        private AnalysisController      m_controller;
        private AnalysisReportGenerator m_reporter;

        public MainWindow()
        {
            InitializeComponent();
            DataContext                     = this;
            AnalysisState                   = ApplicationAnalysisState.Idle;            
            RecentAnalysisObjects           = new ObservableCollection<RecentAnalysis>();
            m_recentStackPanel.DataContext  = RecentAnalysisObjects;
            m_analysisLoadDialog            = new System.Windows.Forms.OpenFileDialog();
            m_controller                    = new AnalysisController();
            m_reporter                      = new AnalysisReportGenerator();
            
            m_performAnalysisControl.AnalysisQuit += new System.EventHandler(m_performAnalysisControl_AnalysisQuit);

            Binding binding = new Binding("Status");
            binding.Source  = ApplicationStatusMediator.Mediator;
            SetBinding(StatusProperty, binding);            
        }

        void m_performAnalysisControl_AnalysisQuit(object sender, System.EventArgs e)
        {
            if (LastApplicationState == ApplicationAnalysisState.Opened)
                AnalysisState = LastApplicationState;
            else
                AnalysisState = ApplicationAnalysisState.Idle;
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(MainWindow));

        private void CleanupMultiAlignController()
        {
            if (m_controller != null)
            {
                m_controller = null;
            }
        }
        private void LoadMultiAlignFile(string filename)
        {
            // Cleanup old ties if necessary.
            CleanupMultiAlignController();
            
            // Create a new controller
            m_controller = new AnalysisController();                                    
            m_controller.LoadExistingAnalysis(filename, m_reporter);
            m_controller.Config.Analysis.MetaData.AnalysisPath = filename;

            m_mainControl.IsEnabled = true;
            m_mainControl.Analysis  = m_controller.Config.Analysis;            
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult result = m_analysisLoadDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadMultiAlignFile(m_analysisLoadDialog.FileName);

                AnalysisState        = ApplicationAnalysisState.Opened;
                LastApplicationState = AnalysisState;

                RecentAnalysis newAnalysis  = new RecentAnalysis();
                newAnalysis.Name            = System.IO.Path.GetFileNameWithoutExtension(m_analysisLoadDialog.FileName);
                newAnalysis.Path            = m_analysisLoadDialog.FileName;

                if (RecentAnalysisObjects.Count > 10)
                {
                    RecentAnalysisObjects.RemoveAt(0);
                }
                RecentAnalysisObjects.Add(newAnalysis);
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
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

        public ObservableCollection<RecentAnalysis> RecentAnalysisObjects
        {
            get { return (ObservableCollection<RecentAnalysis>)GetValue(RecentAnalysisProperty); }
            set { SetValue(RecentAnalysisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RecentAnalysis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecentAnalysisProperty =
            DependencyProperty.Register("RecentAnalysisObjects", typeof(ObservableCollection<RecentAnalysis>), typeof(MainWindow));


        public ApplicationAnalysisState  AnalysisState
        {
            get { return (ApplicationAnalysisState )GetValue(AnalysisStateProperty); }
            set { SetValue(AnalysisStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisStateProperty =
            DependencyProperty.Register("AnalysisState", typeof(ApplicationAnalysisState ), typeof(MainWindow));



        public ApplicationAnalysisState LastApplicationState
        {
            get { return (ApplicationAnalysisState)GetValue(LastApplicationStateProperty); }
            set { SetValue(LastApplicationStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastApplicationState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastApplicationStateProperty =
            DependencyProperty.Register("LastApplicationState", typeof(ApplicationAnalysisState), typeof(MainWindow));


    }

    public enum ApplicationAnalysisState
    {
        Idle,
        RunningAnalysis,
        SetupAnalysis,
        Opened
    }
}
