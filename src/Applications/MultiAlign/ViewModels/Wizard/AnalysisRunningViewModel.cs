using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using MultiAlign.Commands;
using MultiAlign.Data;
using MultiAlign.Windows.Plots;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Reports;
using PNNLOmics.Data.Features;

namespace MultiAlign.ViewModels.Wizard
{
    /// <summary>
    ///     View model for running an analysis.
    /// </summary>
    public class AnalysisRunningViewModel : ViewModelBase, IAnalysisReportGenerator
    {
        private AnalysisConfig m_configuration;
        private AnalysisController m_controller;
        private bool m_isAnalysisRunning;
        private IAnalysisReportGenerator m_reporter;

        public AnalysisRunningViewModel()
        {
            Messages = new ObservableCollection<StatusEventArgs>();
            GalleryImages = new ObservableCollection<UserControl>();
            Reporter = new AnalysisReportGenerator();
            AnalysisNodes = new ObservableCollection<AnalysisGraphNodeViewModel>();
            CancelAnalysis = new BaseCommand(CancelAnalysisDelegate, BaseCommand.AlwaysPass);
            RouteMessages();
        }

        public ICommand CancelAnalysis { get; set; }

        #region Properties

        public ObservableCollection<AnalysisGraphNodeViewModel> AnalysisNodes { get; set; }

        /// <summary>
        ///     Gets the images associated with a dataset.
        /// </summary>
        public ObservableCollection<UserControl> GalleryImages { get; private set; }

        /// <summary>
        ///     Gets or sets whether an analysis is running.
        /// </summary>
        public bool IsAnalysisRunning
        {
            get { return m_isAnalysisRunning; }
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
            get { return m_controller; }
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
            get { return m_reporter; }
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
            get { return m_configuration; }
            set
            {
                if (m_configuration == value)
                    return;

                m_configuration = value;
                OnPropertyChanged("AnalysisConfiguration");
            }
        }

        public ObservableCollection<StatusEventArgs> Messages { get; private set; }

        #endregion

        #region Analysis Controller Event Handlers

        private void AnalysisEnded(string reason, bool isCancelled, bool isComplete)
        {
            Action workAction = delegate
            {
                IsAnalysisRunning = false;
                ApplicationStatusMediator.SetStatus(reason);

                Controller.AnalysisComplete -= Controller_AnalysisComplete;
                Controller.AnalysisError -= Controller_AnalysisError;
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

        private void ControllerOnAnalysisStarted(object sender, AnalysisGraphEventArgs analysisGraphEventArgs)
        {
            Action workAction = () =>
            {
                AnalysisNodes.Clear();
                analysisGraphEventArgs.AnalysisGraph.Nodes.ForEach(
                    x => AnalysisNodes.Add(new AnalysisGraphNodeViewModel(x)));
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }

        private void Controller_AnalysisCancelled(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis was cancelled.", true, false);
        }

        private void Controller_AnalysisError(object sender, EventArgs e)
        {
            AnalysisEnded("There was an error with the analysis.", true, false);
        }

        private void Controller_AnalysisComplete(object sender, EventArgs e)
        {
            AnalysisEnded("The analysis is complete.", false, true);
        }

        #endregion

        #region Building Plots

        /// <summary>
        ///     Builds the alignment plot views.
        /// </summary>
        /// <param name="e"></param>
        private void BuildAlignmentPlotView(FeaturesAlignedEventArgs e)
        {
            var view = new AlignmentPlotView {AlignmentData = e};
            GalleryImages.Insert(0, view);

            if (GalleryImages.Count > 10)
            {
                GalleryImages.RemoveAt(0);
            }
        }

        private void BuildMassTagPlots(MassTagsLoadedEventArgs e)
        {
            var view = new FeaturePlotView {MassTagsData = e};
            GalleryImages.Insert(0, view);
            if (GalleryImages.Count > 10)
            {
                GalleryImages.RemoveAt(9);
            }
        }

        /// <summary>
        ///     Builds the alignment plot views.
        /// </summary>
        /// <param name="e"></param>
        private void BuildBaselineView(BaselineFeaturesLoadedEventArgs e)
        {
            // We dont care about the dataset
            if (e.DatasetInformation != null)
            {
                var view = new FeaturePlotView {BaselineData = e};
                GalleryImages.Insert(0, view);
                if (GalleryImages.Count > 10)
                {
                    GalleryImages.RemoveAt(0);
                }
            }
        }

        /// <summary>
        ///     Builds the alignment plot views.
        /// </summary>
        private void BuildClusterPlots(List<UMCClusterLight> clusters)
        {
            var view = new ClustersPlotView {Clusters = clusters};

            GalleryImages.Insert(0, view);
            if (GalleryImages.Count > 10)
            {
                GalleryImages.RemoveAt(0);
            }
        }

        #endregion

        #region IAnalysisReportGenerator Members        

        public string PlotPath
        {
            get { return Reporter.PlotPath; }
            set { Reporter.PlotPath = value; }
        }

        public void CreateAlignmentPlots(FeaturesAlignedEventArgs e)
        {
            Action workAction = () =>
            {
                BuildAlignmentPlotView(e);
                Reporter.CreateAlignmentPlots(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e)
        {
            Action workAction = () =>
            {
                BuildBaselineView(e);
                Reporter.CreateBaselinePlots(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreateMassTagPlot(MassTagsLoadedEventArgs e)
        {
            Action workAction = () =>
            {
                BuildMassTagPlots(e);
                Reporter.CreateMassTagPlot(e);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e)
        {
            Action workAction = () => Reporter.CreatePeakMatchedPlots(e);
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreateClusterPlots(FeaturesClusteredEventArgs clusters)
        {
            Action workAction = () =>
            {
                BuildClusterPlots(clusters.Clusters);
                Reporter.CreateClusterPlots(clusters);
            };
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreateChargePlots(Dictionary<int, int> chargeMap)
        {
            Action workAction = () => Reporter.CreateChargePlots(chargeMap);
            ThreadSafeDispatcher.Invoke(workAction);
        }

        public void CreatePlotReport()
        {
            Action workAction = () => Reporter.CreatePlotReport();
            ThreadSafeDispatcher.Invoke(workAction);
        }

        #endregion

        #region IAnalysisReportGenerator Members

        public AnalysisConfig Config
        {
            get { return m_configuration; }
            set { AnalysisConfiguration = value; }
        }

        #endregion

        /// <summary>
        ///     The analysis is completed.
        /// </summary>
        public event EventHandler<AnalysisStatusArgs> AnalysisComplete;

        public event EventHandler<AnalysisStatusArgs> AnalysisCancelled;

        private void CancelAnalysisDelegate()
        {
            Controller.CancelAnalysis();
        }

        public void Start(AnalysisConfig config)
        {
            // Set the messages
            Messages.Clear();
            GalleryImages.Clear();

            IsAnalysisRunning = true;
            Reporter.Config = config;
            m_configuration = config;
            Controller = new AnalysisController();
            Controller.AnalysisComplete += Controller_AnalysisComplete;
            Controller.AnalysisError += Controller_AnalysisError;
            Controller.AnalysisCancelled += Controller_AnalysisCancelled;
            Controller.AnalysisStarted += ControllerOnAnalysisStarted;

            // Start the analysis.
            Controller.StartMultiAlignGui(config, this);
        }

        #region Logging Handlers

        /// <summary>
        ///     Starts the routing of the logger messages for the UI.
        /// </summary>
        private void RouteMessages()
        {
            Logger.Status += Logger_Status;
        }

        /// <summary>
        ///     Updates the current messages windows.
        /// </summary>
        private void Logger_Status(object sender, StatusEventArgs e)
        {
            Action workAction = () => Messages.Insert(0, e);

            ThreadSafeDispatcher.Invoke(workAction);
        }

        #endregion
    }
}