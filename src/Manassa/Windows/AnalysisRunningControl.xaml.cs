using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Manassa.Data;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using System.IO;

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

            GalleryImages = new ObservableCollection<UserControl>();
            
        }

        #region Logging Handlers 
        /// <summary>
        /// Stops the routing of the logger messages for the UI.
        /// </summary>
        private void DerouteMessages()
        {
            Logger.Status -= Logger_Status;
        }
        /// <summary>
        /// Starts the routing of the logger messages for the UI.
        /// </summary>
        private void RouteMessages()
        {
            Logger.Status += new EventHandler<MultiAlignCore.IO.StatusEventArgs>(Logger_Status);         
        }
        /// <summary>
        /// Updates the current messages windows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Logger_Status(object sender, MultiAlignCore.IO.StatusEventArgs e)
        {            
            Action workAction = delegate
            {                
                CurrentStatusMessage = e.Message;
                Messages.Add(e.Message);
                messagesBox.ScrollIntoView(e.Message);                
            };

            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);            
        }
        #endregion

        public void Start()
        {
            // Set the messages
            Messages.Clear();
            CurrentStatusMessage = "Starting Analysis.";

            // route the logger messages
            RouteMessages();

            IsAnalysisRunning = true;

            Reporter.Config = AnalysisConfiguration;

            Controller.AnalysisComplete += new EventHandler(Controller_AnalysisComplete);
            Controller.AnalysisError    += new EventHandler(Controller_AnalysisError);
            Controller.AnalysisCancelled += new EventHandler(Controller_AnalysisCancelled);

            // Start the analysis.
            Controller.StartMultiAlignGUI(AnalysisConfiguration, this);                        
        }


        private void AnalysisEnded(string reason)
        {
            Action workAction = delegate
            {
                IsAnalysisRunning = false;
                ApplicationStatusMediator.SetStatus(reason);

                Controller.AnalysisComplete -= Controller_AnalysisComplete;
                Controller.AnalysisError -= Controller_AnalysisError;
                Controller.AnalysisCancelled -= Controller_AnalysisCancelled;
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        void Controller_AnalysisCancelled(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis was cancelled.");
        }
        void Controller_AnalysisError(object sender, EventArgs e)
        {
            AnalysisEnded("There was an error with the analysis.");            
        }
        void Controller_AnalysisComplete(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis is complete.");    
        }



        public ObservableCollection<UserControl> GalleryImages
        {
            get { return (ObservableCollection<UserControl>)GetValue(GalleryImagesProperty); }
            set { SetValue(GalleryImagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GalleryImages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GalleryImagesProperty =
            DependencyProperty.Register("GalleryImages",
                        typeof(ObservableCollection<UserControl>), 
                        typeof(AnalysisRunningControl));

        public bool IsAnalysisRunning
        {
            get { return (bool)GetValue(IsAnalysisRunningProperty); }
            set { SetValue(IsAnalysisRunningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAnalysisRunning.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAnalysisRunningProperty =
            DependencyProperty.Register("IsAnalysisRunning", typeof(bool), typeof(AnalysisRunningControl), new UIPropertyMetadata(false));
        
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

                                                    if (x.IsAnalysisRunning)
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
        private void BuildAlignmentPlotView(FeaturesAlignedEventArgs e)
        {
            AlignmentPlotView view = new AlignmentPlotView();
            view.AlignmentData = e;
            GalleryImages.Add(view);
        }
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            Action workAction = delegate
            {
                BuildAlignmentPlotView(e);
                Reporter.CreateAlignmentPlots(e);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            Action workAction = delegate
            {
                Reporter.CreateBaselinePlots(e);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {
            Action workAction = delegate
            {
                Reporter.CreateMassTagPlot(e);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);        
        }
        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            Action workAction = delegate
            {
                Reporter.CreatePeakMatchedPlots(e);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        public void CreatePlotReport()
        {
            Action workAction = delegate
            {
                Reporter.CreatePlotReport();
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        public string PlotPath
        {
            get
            {
                string plotPath = "";
                Action workAtion = delegate
                {
                    plotPath = Reporter.PlotPath;
                };
               
                Dispatcher.Invoke(workAtion, DispatcherPriority.Normal);                    
                return plotPath;
            }
            set
            {
                Action workAction = delegate
                {
                    Reporter.PlotPath = value;
                };
                Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
            }
        }        
        public void CreateClusterPlots(List<PNNLOmics.Data.Features.UMCClusterLight> clusters)
        {
            Action workAction = delegate
            {                
                Reporter.CreateClusterPlots(clusters);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal);
        }
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Action workAction = delegate
            {
                Reporter.CreateChargePlots(chargeMap);
            };
            Dispatcher.Invoke(workAction, DispatcherPriority.Normal); 
        }
        #endregion
    }
}
