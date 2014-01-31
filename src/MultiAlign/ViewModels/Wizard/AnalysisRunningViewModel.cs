using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using MultiAlign.Data;
using MultiAlign.Windows;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCustomControls.Drawing;
using PNNLOmics.Data.Features;
using System.Windows.Input;
using MultiAlign.Commands;

namespace MultiAlign.ViewModels.Wizard
{
    /// <summary>
    /// View model for running an analysis.
    /// </summary>
    public class AnalysisRunningViewModel : ViewModelBase, IAnalysisReportGenerator
    {
        /// <summary>
        /// The analysis is completed.
        /// </summary>
        public event EventHandler<AnalysisStatusArgs> AnalysisComplete;
        public event EventHandler<AnalysisStatusArgs> AnalysisCancelled;

        private AnalysisController          m_controller;
        private IAnalysisReportGenerator    m_reporter;
        private AnalysisConfig              m_configuration;
        private string                      m_currentStatus;
        private bool                        m_isAnalysisRunning;

        public AnalysisRunningViewModel()
        {            
            Messages        = new ObservableCollection<string>();
            GalleryImages   = new ObservableCollection<UserControl>();
            Reporter        = new AnalysisReportGenerator();

            CancelAnalysis = new BaseCommandBridge(new CommandDelegate(CancelAnalysisDelegate));
            RouteMessages();
        }

        private void CancelAnalysisDelegate(object parameter)
        {
            Controller.CancelAnalysis();
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
                Messages.Insert(0, e.Message);   
           
            };

            ThreadSafeDispatcher.Invoke(workAction);
        }
        #endregion

        public void Start(AnalysisConfig config)
        {
            // Set the messages
            Messages.Clear();
            GalleryImages.Clear();

            CurrentStatusMessage             = "Starting Analysis.";            
            IsAnalysisRunning                = true;
            Reporter.Config                  = config;
            m_configuration                  = config;
            Controller                       = new AnalysisController();            
            Controller.AnalysisComplete     += new EventHandler(Controller_AnalysisComplete);
            Controller.AnalysisError        += new EventHandler(Controller_AnalysisError);
            Controller.AnalysisCancelled    += new EventHandler(Controller_AnalysisCancelled);

            // Start the analysis.
            Controller.StartMultiAlignGUI(config, this);                        
        }

        public ICommand CancelAnalysis { get; set; }

        #region Properties
        /// <summary>
        /// Gets the images associated with a dataset.
        /// </summary>
        public ObservableCollection<UserControl> GalleryImages
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets whether an analysis is running.
        /// </summary>
        public bool IsAnalysisRunning
        {
            get
            {
                return m_isAnalysisRunning;
            }
            set
            {
                if (m_isAnalysisRunning != value)
                {
                    m_isAnalysisRunning = value;
                    OnPropertyChanged("IsAnalysisRunning");
                }
            }
        }
        public AnalysisController Controller
        {
            get
            {
                return m_controller;
            }
            set
            {
                if (m_controller != value)
                {
                    m_controller = value;
                    OnPropertyChanged("Controller");
                }
            }
        }
        public IAnalysisReportGenerator Reporter
        {
            get
            {
                return m_reporter;
            }
            set
            {
                if (m_reporter != value)
                {
                    m_reporter = value;
                    OnPropertyChanged("Reporter");
                }
            }
        }
        public AnalysisConfig AnalysisConfiguration
        {
            get
            {
                return m_configuration;
            }
            set
            {
                if (m_configuration != value)
                {
                    m_configuration = value;
                    OnPropertyChanged("AnalysisConfiguration");
                }
            }
        }
        public string CurrentStatusMessage
        {
            get
            {
                return m_currentStatus;
            }
            set
            {
                if (m_currentStatus != value)
                {
                    m_currentStatus = value;
                    OnPropertyChanged("CurrentStatusMessage");
                }
            }
        }
        public ObservableCollection<string> Messages
        {
            get;
            private set;
        }   
        #endregion

        #region Analysis Controller Event Handlers
        private void AnalysisEnded(string reason, bool isCancelled, bool isComplete)
        {
            Action workAction = delegate
            {
                IsAnalysisRunning = false;
                ApplicationStatusMediator.SetStatus(reason);

                Controller.AnalysisComplete  -= Controller_AnalysisComplete;
                Controller.AnalysisError     -= Controller_AnalysisError;
                Controller.AnalysisCancelled -= Controller_AnalysisCancelled;

                if (isComplete)
                {
                    if (AnalysisComplete != null)
                    {
                        AnalysisComplete(this, new AnalysisStatusArgs(m_configuration));
                    }
                    return;
                }
                if (!isCancelled)
                {
                    if (AnalysisComplete != null)
                    {
                        AnalysisComplete(this, new AnalysisStatusArgs(m_configuration));
                    }
                }
                else
                {
                    if (AnalysisCancelled != null)
                    {
                        AnalysisCancelled(this, new AnalysisStatusArgs(m_configuration));
                    }
                }
            };
            ThreadSafeDispatcher.Invoke(workAction);
            
        }
        void Controller_AnalysisCancelled(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis was cancelled.", true, false);
        }
        void Controller_AnalysisError(object sender, EventArgs e)
        {
            AnalysisEnded("There was an error with the analysis.", true, false);            
        }
        void Controller_AnalysisComplete(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis is complete.", false, true);
        }
        #endregion

        #region Building Plots
        /// <summary>
        /// Builds the alignment plot views.
        /// </summary>
        /// <param name="e"></param>
        private void BuildAlignmentPlotView(FeaturesAlignedEventArgs e)
        {
            AlignmentPlotView view = new AlignmentPlotView();
            view.AlignmentData = e;
            GalleryImages.Insert(0, view);

            if (GalleryImages.Count > 10)
            {
                UserControl control = GalleryImages[0];
                GalleryImages.RemoveAt(0);
            }
        }
        private void BuildMassTagPlots(MassTagsLoadedEventArgs e)
        {            
            FeaturePlotView view = new FeaturePlotView();
            view.MassTagsData    = e;
            GalleryImages.Insert(0, view);            
            if (GalleryImages.Count > 10)
            {
                GalleryImages.RemoveAt(0);
            }           
        }
        /// <summary>
        /// Builds the alignment plot views.
        /// </summary>
        /// <param name="e"></param>
        private void BuildBaselineView(BaselineFeaturesLoadedEventArgs e)
        {            
            // We dont care about the dataset
            if (e.DatasetInformation != null)
            {
                FeaturePlotView view = new FeaturePlotView();
                view.BaselineData = e;
                GalleryImages.Insert(0, view);
                //GalleryScroll.ScrollToEnd();
                if (GalleryImages.Count > 10)
                {
                    GalleryImages.RemoveAt(0);
                }
            }            
        }
        /// <summary>
        /// Builds the alignment plot views.
        /// </summary>
        /// <param name="e"></param>
        private void BuildClusterPlots(List<UMCClusterLight> clusters)
        {
            ClustersPlotView view = new ClustersPlotView();
            view.Clusters = clusters;

            GalleryImages.Insert(0, view);
            //GalleryScroll.ScrollToEnd();
            if (GalleryImages.Count > 10)
            {
                GalleryImages.RemoveAt(0);
            }
        }
        #endregion

        #region IAnalysisReportGenerator Members        
        public string PlotPath
        {
            get
            {
                string plotPath =  Reporter.PlotPath;                
                return plotPath;
            }
            set
            {               
                Reporter.PlotPath = value;                
           }
        }        
        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            Action workAction = delegate
            {
                BuildAlignmentPlotView(e);
                Reporter.CreateAlignmentPlots(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            Action workAction = delegate
            {
                BuildBaselineView(e);
                Reporter.CreateBaselinePlots(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {
            Action workAction = delegate
            {
                BuildMassTagPlots(e);
                Reporter.CreateMassTagPlot(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            Action workAction = delegate
            {
                Reporter.CreatePeakMatchedPlots(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreateClusterPlots(FeaturesClusteredEventArgs clusters)
        {
            Action workAction = delegate
            {
                BuildClusterPlots(clusters.Clusters);
                Reporter.CreateClusterPlots(clusters);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Action workAction = delegate
            {
                Reporter.CreateChargePlots(chargeMap);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        public void CreatePlotReport()
        {
            Action workAction = delegate
            {
                Reporter.CreatePlotReport();
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }
        #endregion

        #region IAnalysisReportGenerator Members
        public AnalysisConfig Config
        {
            get
            {
                return m_configuration;
            }
            set
            {
                AnalysisConfiguration = value;
            }
        }
        #endregion
    }
}
