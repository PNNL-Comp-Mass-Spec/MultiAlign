using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MultiAlignCore.Data;
using Manassa.Data;
using System.Collections.ObjectModel;
using MultiAlignCore.Algorithms;
using MultiAlignCore.IO;

namespace Manassa.Windows
{
    /// <summary>
    /// ViewModel for running an analysis, display results, etc.
    /// </summary>
    public partial class AnalysisRunningControl : UserControl, IAnalysisReportGenerator 
    {        
        public AnalysisRunningControl()
        {
            InitializeComponent();
            CurrentState        = ApplicationAnalysisState.Idle;
            DataContext         = this;
            Messages = new ObservableCollection<string>();
            
        }

        private void DerouteMessages()
        {
            Logger.Status -= Logger_Status;
        }
        private void RouteMessages()
        {
            Logger.Status += new EventHandler<MultiAlignCore.IO.StatusEventArgs>(Logger_Status);         
        }

        void Logger_Status(object sender, MultiAlignCore.IO.StatusEventArgs e)
        {
            CurrentStatusMessage = e.Message;
            Messages.Add(e.Message);
        }
        public void Start()
        {
            Messages.Clear();
            CurrentStatusMessage = "Starting Analysis.";
            RouteMessages();
            Controller.StartMultiAlign(AnalysisConfiguration, this);            
        }

        public AnalysisController Controller
        {
            get { return (AnalysisController)GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(AnalysisController), typeof(AnalysisRunningControl));

        public IAnalysisReportGenerator Reporter
        {
            get { return (IAnalysisReportGenerator)GetValue(StaticReporterProperty); }
            set { SetValue(StaticReporterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StaticReporter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StaticReporterProperty =
            DependencyProperty.Register("Reporter", typeof(IAnalysisReportGenerator), typeof(AnalysisRunningControl)); 
        
        public AnalysisConfig AnalysisConfiguration
        {
            get { return (AnalysisConfig)GetValue(AnalysisConfigurationProperty); }
            set { SetValue(AnalysisConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnalysisConfiguration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnalysisConfigurationProperty =
            DependencyProperty.Register("AnalysisConfiguration", typeof(AnalysisConfig), typeof(AnalysisRunningControl));

        public string CurrentStatusMessage
        {
            get { return (string)GetValue(CurrentStatusProperty); }
            set { SetValue(CurrentStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentStatusProperty =
            DependencyProperty.Register("CurrentStatusMessage", typeof(string), typeof(AnalysisRunningControl), new UIPropertyMetadata("Ready"));


        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>)GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof(ObservableCollection<string>), typeof(AnalysisRunningControl));


        /// <summary>
        /// Determines the current state of the analysis.
        /// </summary>
        public ApplicationAnalysisState CurrentState
        {
            get { return (ApplicationAnalysisState)GetValue(CurrentStateProperty); }
            set { SetValue(CurrentStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentStateProperty =
            DependencyProperty.Register("CurrentState", 
                                        typeof(ApplicationAnalysisState), 
                                        typeof(AnalysisRunningControl),            
                                        new PropertyMetadata(
                                                delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
                                                {
                                                    var x = sender as AnalysisRunningControl;
                                                    if (x == null)
                                                        return;

                                                    if (x.CurrentState == ApplicationAnalysisState.RunningAnalysis)
                                                    {
                                                        x.Start();
                                                    }
                                                })
                                            );

        #region IAnalysisReportGenerator Members
        public AnalysisConfig Config
        {
            get
            {
                return AnalysisConfiguration;
            }
            set
            {
                //DO nothing?
            }
        }
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            Reporter.CreateAlignmentPlots(e);
        }
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            Reporter.CreateBaselinePlots(e);
        }
        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {
            Reporter.CreateMassTagPlot(e);            
        }
        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            Reporter.CreatePeakMatchedPlots(e);
        }
        public void CreatePlotReport()
        {
            Reporter.CreatePlotReport();
        }
        public string PlotPath
        {
            get
            {
                return Reporter.PlotPath;
            }
            set
            {
                Reporter.PlotPath = value;
            }
        }
        public void SaveImage(System.Drawing.Image image, string name)
        {
            Reporter.SaveImage(image, name);
        }
        public void CreateClusterPlots(List<PNNLOmics.Data.Features.UMCClusterLight> clusters)
        {
            Reporter.CreateClusterPlots(clusters);
        }
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Reporter.CreateChargePlots(chargeMap);   
        }
        #endregion
    }
}
