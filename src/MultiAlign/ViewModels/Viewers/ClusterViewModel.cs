using MultiAlign.Commands;
using MultiAlign.IO;
using MultiAlign.ViewModels.Features;
using MultiAlign.ViewModels.TreeView;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Extensions;
using PNNLControls;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Chromatograms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using PNNLOmics.Extensions;
using System.Windows.Forms.Integration;
using System.Windows.Input;


namespace MultiAlign.ViewModels.Viewers
{
    /// <summary>
    /// View model for the main cluste control viewer
    /// </summary>
    public class ClusterDetailViewModel: ViewModelBase
    {
        private ChargeStateViewModel    m_charge;
        private WindowsFormsHost        m_parentSpectraHost;
        private WindowsFormsHost        m_clusterChartHost;
        private WindowsFormsHost        m_driftChartHost;
        private UMCTreeViewModel        m_selectedFeature;
        private UMCClusterTreeViewModel m_clusterViewModel;

        private System.Windows.Forms.SaveFileDialog  m_featureImageSaveDialog;
        private UMCClusterLight m_mainCluster;
        private bool m_adjustingFeaturePlots = false;
        private Dictionary<ctlChartBase, List<ChartSynchData>> m_chartSynchMap;
        private System.Windows.Forms.SaveFileDialog m_saveFileDialog;

        private int m_numberOfIsotopes;
        private int m_minScan;
        private int m_maxScan;
        private UMCClusterLight m_cluster;
        private Dictionary<int, List<MSFeatureLight>> m_scanMaps;

        SICChart m_sicChart;
        SingleUMCChart m_msFeaturePlot;
        SingleClusterChart m_clusterChart;
        SingleClusterChart m_driftChart;
        MsFeatureChart m_parentSpectra;
        private bool m_usesDriftTime;
        private WindowsFormsHost m_msFeaturePlotHost;
        private WindowsFormsHost m_xicChartHost;


        public ClusterDetailViewModel()
        {
            m_numberOfIsotopes  = 4;
            m_scanMaps          = new Dictionary<int, List<MSFeatureLight>>();
            Charges             = new ObservableCollection<ChargeStateViewModel>();
            m_chartSynchMap     = new Dictionary<ctlChartBase, List<ChartSynchData>>();

            FeatureFindingTolerances                = new FeatureTolerances();
            FeatureFindingTolerances.Mass           = 10;
            FeatureFindingTolerances.DriftTime      = 3;
            FeatureFindingTolerances.RetentionTime  = 50;
            ClusterTolerances                       = new FeatureTolerances();
            ClusterTolerances.Mass                  = 10;
            ClusterTolerances.DriftTime             = 3;
            ClusterTolerances.RetentionTime         = .03;

            m_ppmError                      = 10;

            m_sicChart                      = new SICChart();
            m_sicChart.Title                = "XIC";                        
            m_sicChart.TitleVisible         = false;
            m_sicChart.LegendVisible        = false;
            m_xicChartHost = new WindowsFormsHost() { Child = m_sicChart };

            m_parentSpectra                 = new MsFeatureChart();
            m_parentSpectra.Title           = "MS";
            m_parentSpectra.XAxisLabel      = "m/z";
            m_parentSpectra.YAxisLabel      = "Intensity";
            m_parentSpectra.TitleVisible    = false;
            m_parentSpectraHost = new WindowsFormsHost() { Child = m_parentSpectra };

            m_msFeaturePlot                 = new SingleUMCChart();
            m_msFeaturePlot.Title           = "";
            m_msFeaturePlot.XAxisShortHand  = "scans";
            m_msFeaturePlot.YAxisShortHand  = "ppm";
            m_msFeaturePlot.LegendVisible   = false;
            m_msFeaturePlotHost             = new WindowsFormsHost() { Child = m_msFeaturePlot };

            m_clusterChart                  = new SingleClusterChart();
            m_clusterChart.TitleVisible     = false;
            m_clusterChart.XAxisShortHand   = "NET";
            m_clusterChart.YAxisShortHand   = "ppm";
            m_clusterChart.LegendVisible    = false;
            m_clusterChartHost              = new WindowsFormsHost() { Child = m_clusterChart };

            m_driftChart                    = new SingleClusterChart();
            m_driftChart.TitleVisible       = false;
            m_driftChart.XAxisShortHand     = "ms";
            m_driftChart.YAxisShortHand     = "ppm";
            m_driftChart.XAxisLabel         = "Drift Time (ms)";
            m_driftChart.LegendVisible      = false;
            m_driftChartHost                = new WindowsFormsHost() { Child = m_driftChart };

            Action x = delegate() 
            {
                UMCLight feature    = SelectedFeature.Feature;
                UMCLight newFeature = LoadExtractedIonChromatrogram(feature, m_ppmError);                
                m_sicChart.SetFeature(newFeature);
                m_sicChart.AutoViewPort();                
            };
            CreateXic = new BaseCommandBridge (x, null);

            m_sicChart.ViewPortChanged      += new ViewPortChangedHandler(m_sicChart_ViewPortChanged);
            m_sicChart.ChartPointPressed    += new EventHandler<SelectedPointEventArgs>(m_sicChart_ChartPointPressed);     
            m_msFeaturePlot.ViewPortChanged += new ViewPortChangedHandler(m_msFeaturePlot_ViewPortChanged);
            m_clusterChart.ViewPortChanged  += new ViewPortChangedHandler(m_clusterChart_ViewPortChanged);
            m_driftChart.ViewPortChanged    += new ViewPortChangedHandler(m_driftChart_ViewPortChanged);

            m_featureImageSaveDialog        = new System.Windows.Forms.SaveFileDialog();
            m_saveFileDialog                = new System.Windows.Forms.SaveFileDialog();
            m_saveFileDialog.Filter         = "DTA (*.dta)|*.dta|MGF (*.mgf)|*.mgf";
            Features                        = new ObservableCollection<UMCTreeViewModel>();
        }

        #region Windows Form Base
        public WindowsFormsHost DriftChartHost
        {
            get
            {
                return m_driftChartHost;
            }
        }
        public WindowsFormsHost ClusterChartHost
        {
            get
            {
                return m_clusterChartHost;
            }
        }
        public WindowsFormsHost ParentSpectraChart
        {
            get
            {
                return m_parentSpectraHost;
            }
        }
        public WindowsFormsHost MSFeaturePlot
        {
            get
            {
                return m_msFeaturePlotHost;
            }
        }
        public WindowsFormsHost XicChart
        {
            get
            {
                return m_xicChartHost;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the current cluster.
        /// </summary>
        public UMCClusterLight Cluster
        {
            get
            {
                return m_cluster;
            }
            set
            {
                
                m_cluster = value;
            }
        }
        public UMCTreeViewModel SelectedFeature
        {
            get
            {
                return m_selectedFeature;
            }
            set
            {
                if (m_selectedFeature != value)
                {
                    m_selectedFeature = value;
                    if (value != null)
                        SetFeature(value.Feature);

                    OnPropertyChanged("SelectedFeature");
                }
            }
        }
        #region Properties
        public string SelectedFeatureName
        {
            get
            {
                return m_selectedFeatureName;
            }
            set
            {
                if (m_selectedFeatureName != value)
                {
                    m_selectedFeatureName = value;
                    OnPropertyChanged("SelectedFeatureName");
                }
            }
        }
        public bool UsesDriftTime
        {
            get
            {
                return m_usesDriftTime;
            }
            set
            {
                if (m_usesDriftTime != value)
                {
                    m_usesDriftTime = value;
                    OnPropertyChanged("UsesDriftTime");
                }
            }
        }
        /// <summary>
        /// Gets or sets the number of isotopes to display.
        /// </summary>
        public int NumberOfIsotopes
        {
            get
            {
                return m_numberOfIsotopes;
            }
            set
            {
                if (m_numberOfIsotopes != value)
                {
                    m_numberOfIsotopes = value;
                    OnPropertyChanged("NumberOfIsotopes");
                }
            }
        }
        #endregion

        #region Data auto-properties
        /// <summary>
        /// Cluster tolerances
        /// </summary>
        public FeatureTolerances ClusterTolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Feature finding tolerances
        /// </summary>
        public FeatureTolerances FeatureFindingTolerances
        {
            get;
            set;
        }
        #endregion   

        public RectangleF Viewport { get; set; }

        public MSSpectra SelectedSpectrum { get; set; }


        private void SetFeature(UMCLight feature)
        {
            if (feature == null)
                return;

            m_adjustingFeaturePlots = true;
            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(feature.GroupID);

            if (info != null)
            {
                SelectedFeatureName = info.DatasetName;
            }

            m_msFeaturePlot.MainFeature = feature;
            //UMCLight newFeature = LoadExtractedIonChromatrogram(feature, m_ppmError);
            m_sicChart.SetFeature(feature);
            m_sicChart.AutoViewPort();
            m_msFeaturePlot.UpdateCharts(true);

            m_sicChart.AdjustViewPortWithTolerances(FeatureFindingTolerances, false);
            m_msFeaturePlot.AdjustViewPortWithTolerances(FeatureFindingTolerances, false);
            m_adjustingFeaturePlots = false;


            UpdateCharges(feature);

            if (feature.MSFeatures.Count > 0)
            {
                feature.MSFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                {
                    return x.Scan.CompareTo(y.Scan);
                });
                MSFeatureLight msFeature = feature.MSFeatures[0];
                LoadSpectrum(msFeature);
            }
        }

        private void SynchViewports(ctlChartBase sender, ChartSynchType synchType, params ctlChartBase[] targets)
        {
            sender.ViewPortChanged += new ViewPortChangedHandler(sender_ViewPortChanged);
            List<ChartSynchData> synchs = new List<ChartSynchData>();
            foreach (ctlChartBase target in targets)
            {
                ChartSynchData data = new ChartSynchData();
                data.SynchType = synchType;
                data.Target = target;
                synchs.Add(data);
            }
            if (!m_chartSynchMap.ContainsKey(sender))
            {
                m_chartSynchMap.Add(sender, new List<ChartSynchData>());
            }
            m_chartSynchMap[sender].AddRange(synchs);
        }

        #region Chart Event Handlers
        void m_sicChart_ChartPointPressed(object sender, SelectedPointEventArgs e)
        {
            // Figure out the closest spectra to view.            
            if (e != null && e.SelectedPoint != null)
            {
                float x = e.SelectedPoint.X;

                if (SelectedFeature != null)
                {
                    int point = Convert.ToInt32(x);
                    MSFeatureLight closest = null;
                    int closestDist = int.MaxValue;
                    foreach (MSFeatureLight feature in SelectedFeature.Feature.MSFeatures)
                    {
                        int diff = Math.Abs(feature.Scan - point);
                        if (closest == null || diff < closestDist && (SelectedCharge.ChargeState == feature.ChargeState))
                        {
                            closest = feature;
                            closestDist = diff;
                        }
                    }
                    if (closest != null)
                    {
                        LoadSpectrum(closest);
                    }
                }
            }
        }
        void sender_ViewPortChanged(ctlChartBase chart, ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;
            m_adjustingFeaturePlots = true;

            if (!m_chartSynchMap.ContainsKey(chart))
                return;

            foreach (ChartSynchData data in m_chartSynchMap[chart])
            {
                RectangleF senderView = args.ViewPort;
                ctlChartBase target = data.Target;
                RectangleF targetView = target.ViewPort;
                switch (data.SynchType)
                {
                    case ChartSynchType.XAxis:
                        target.ViewPort = new RectangleF(senderView.X,
                                                     targetView.Y,
                                                     senderView.Width,
                                                     targetView.Height);
                        break;
                    case ChartSynchType.YAxis:
                        target.ViewPort = new RectangleF(targetView.X,
                                                     senderView.Y,
                                                     targetView.Width,
                                                     senderView.Height);
                        break;
                    case ChartSynchType.YToXAxis:
                        target.ViewPort = new RectangleF(senderView.Y,
                                                     targetView.Y,
                                                     senderView.Height,
                                                     targetView.Height);
                        break;
                    case ChartSynchType.XToYAxis:
                        target.ViewPort = new RectangleF(targetView.X,
                                                     senderView.X,
                                                     targetView.Width,
                                                     senderView.Width);
                        break;
                    case ChartSynchType.Both:
                        target.ViewPort = new RectangleF(senderView.X,
                                                     senderView.Y,
                                                     senderView.Width,
                                                     senderView.Height);
                        break;
                    default:
                        break;
                }
            }
            m_adjustingFeaturePlots = false;
        }
        #region Viewport synching
        void m_driftChart_ViewPortChanged(ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;


            m_adjustingFeaturePlots = true;
            RectangleF otherView = args.ViewPort;
            RectangleF newViewport = new RectangleF(m_clusterChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_clusterChart.ViewPort.Width,
                                                            otherView.Height);
            m_clusterChart.ViewPort = newViewport;
            m_adjustingFeaturePlots = false;

            Viewport = m_clusterChart.ViewPort;
        }
        void m_clusterChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {

            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots = true;
            RectangleF otherView = args.ViewPort;

            RectangleF newViewport = new RectangleF(m_driftChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_driftChart.ViewPort.Width,
                                                            otherView.Height);
            m_driftChart.ViewPort = newViewport;
            m_adjustingFeaturePlots = false;

            Viewport = m_clusterChart.ViewPort;
        }
        void m_msFeaturePlot_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots = true;
            RectangleF otherView = args.ViewPort;
            RectangleF newViewport = new RectangleF(otherView.X,
                                                            m_sicChart.ViewPort.Y,
                                                            otherView.Width,
                                                            m_sicChart.ViewPort.Height);
            m_sicChart.ViewPort = newViewport;
            m_adjustingFeaturePlots = false;
        }
        void m_sicChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots = true;
            RectangleF otherView = args.ViewPort;
            RectangleF newViewport = new RectangleF(otherView.X,
                                                            m_msFeaturePlot.ViewPort.Y,
                                                            otherView.Width,
                                                            m_msFeaturePlot.ViewPort.Height);
            m_msFeaturePlot.ViewPort = newViewport;
            m_adjustingFeaturePlots = false;
        }
        #endregion
        #endregion

        private string m_selectedFeatureName;
        private double m_ppmError;

                
        /// <summary>
        /// Grabs the data from the provider cache and shoves into UI where needed.  This is done
        /// here like this to prevent holding large amounts of data in memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCluster(UMCClusterLightMatched cluster)
        {
            m_adjustingFeaturePlots = true;

            // Grab the data from the cache            
            UpdatePlotsWithClusterData(cluster);
            m_adjustingFeaturePlots = false;           
        }
        private void UpdateCharges(UMCLight feature)
        {
            Charges.Clear();
            m_scanMaps = feature.CreateChargeMap();

            foreach (int charge in m_scanMaps.Keys)
            {
                double mz           = 0;
                int  minScan        = int.MaxValue;
                int  maxScan        = int.MinValue;
                long maxIntensity   = 0;

                foreach (MSFeatureLight msFeature in m_scanMaps[charge])
                {
                    minScan = Math.Min(minScan, msFeature.Scan);
                    maxScan = Math.Max(maxScan, msFeature.Scan);
                    if (maxIntensity < msFeature.Abundance)
                    {
                        maxIntensity = msFeature.Abundance;
                        mz           = msFeature.Mz;
                    }
                }

                Charges.Add(new ChargeStateViewModel(charge, mz, minScan, maxScan));
            }
            Charges.OrderBy(x => x);
            if (Charges.Count > 0)
            {                
                SelectedCharge = Charges[0];
            }
        }

        #region View Model 
        public ChargeStateViewModel SelectedCharge
        {
            get
            {
                return m_charge;
            }
            set
            {
                if (m_charge != value)
                {
                    m_charge = value;
                    OnPropertyChanged("SelectedCharge");

                    if (value == null)
                        return;

                    if (m_scanMaps.ContainsKey(value.ChargeState))
                    {
                        int minScan = int.MaxValue;
                        int maxScan = int.MinValue;
                        foreach (MSFeatureLight msFeature in m_scanMaps[value.ChargeState])
                        {
                            minScan = Math.Min(minScan, msFeature.Scan);
                            maxScan = Math.Max(maxScan, msFeature.Scan);
                        }

                        if (m_scanMaps[value.ChargeState].Count > 0)
                        {
                            MinimumScan = minScan;
                            MaximumScan = maxScan;
                        }
                    }
                }
            }
        }
        public ObservableCollection<ChargeStateViewModel> Charges
        { 
            get;
            private set; 
        }
        public ICommand CreateXic { get; set; }
        public double PpmError
        {
            get
            {
                return m_ppmError;
            }
            set
            {
                if (m_ppmError != value)
                {
                    m_ppmError = value;
                    OnPropertyChanged("PpmError");
                }
            }
        }
        public ObservableCollection<UMCTreeViewModel> Features 
        { 
            get;
            private set; 
        }
        public int MinimumScan
        {
            get
            {
                return m_minScan;
            }
            set
            {
                if (value != m_minScan)
                {
                    m_minScan = value;
                    OnPropertyChanged("MinimumScan");
                }
            }
        }
        public int MaximumScan
        {
            get
            {
                return m_maxScan;
            }
            set
            {
                if (value != m_maxScan)
                {
                    m_maxScan = value;
                    OnPropertyChanged("MaximumScan");
                }
            }
        }
        #endregion

        #region Spectrum Loading
        private UMCLight LoadExtractedIonChromatrogram(UMCLight feature, double massError)
        {           
            var tempFeature = new UMCLight(feature);

             DatasetInformation info = SingletonDataProviders.GetDatasetInformation(feature.GroupID);
             if (info != null && info.Raw != null && info.RawPath != null)
             {
                 using (ISpectraProvider provider = RawLoaderFactory.CreateFileReader(info.RawPath))
                 {
                     if (provider != null)
                     {
                         provider.AddDataFile(info.RawPath, 0);
            
                         var creator = new XicCreator();
                         var xics    = creator.CreateXic(feature, m_ppmError, provider);
                         foreach (var charge in xics.Keys)
                         {
                             foreach (var msfeature in xics[charge])
                             {
                                 tempFeature.AddChildFeature(msfeature);
                             }
                         }
                     }
                 }
             }

             return tempFeature;
        }
        
        private void LoadSpectrum(MSFeatureLight msFeature)
        {
            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(msFeature.GroupID);
            if (info != null && info.Raw != null && info.RawPath != null)
            {
                Dictionary<int, List<MSFeatureLight>> features = msFeature.ParentFeature.CreateChargeMap();

                int N = features[msFeature.ChargeState].Count;
                if (!features.ContainsKey(msFeature.ChargeState) || N < 1)
                    return;

                double sum = 0;
                double min = double.MaxValue;
                double max = double.MinValue;
                double maxIntensity = 0;

                foreach (MSFeatureLight chargeFeature in features[msFeature.ChargeState])
                {
                    min = Math.Min(min, chargeFeature.Mz);
                    max = Math.Max(max, chargeFeature.Mz);
                    maxIntensity = Math.Max(chargeFeature.Abundance, maxIntensity);
                    sum += chargeFeature.Mz;
                }
                maxIntensity += (maxIntensity * .1);

                double averageMz = sum / Convert.ToDouble(N);
                double spacing = (1.0 / Convert.ToDouble(msFeature.ChargeState));
                double adder = spacing * m_numberOfIsotopes;
                double lowMz = averageMz - spacing;
                double highMz = averageMz + adder;

                List<XYData> spectrum = ParentSpectraFinder.GetParentSpectrum(info.RawPath,
                                                                                msFeature.Scan,
                                                                                lowMz,
                                                                                highMz);
                m_parentSpectra.Title = string.Format("Scan {0} Charge {1} Dataset {2}",
                                                        msFeature.Scan,
                                                        msFeature.ChargeState,
                                                        msFeature.GroupID
                                                        );
                m_parentSpectra.SetFeature(msFeature, spectrum);
                m_parentSpectra.AutoViewPort();
            }
        }
        #endregion

        /// <summary>
        /// Dependency Property
        /// </summary>
        public UMCClusterTreeViewModel SelectedCluster
        {
            get
            {
                return m_clusterViewModel;
            }
            set
            {
                if (m_clusterViewModel != value)
                {
                    m_clusterViewModel = value;
                    SetCluster(value.Cluster);
                    OnPropertyChanged("SelectedCluster");
                }
            }
        }
        
        #region Plot updating.
        /// <summary>
        /// Updates the plots with data stored in the cache.  
        /// </summary>
        /// <param name="providers"></param>
        /// <param name="cluster"></param>
        private void UpdatePlotsWithClusterData(UMCClusterLightMatched matchedCluster)
        {
            // Clear the charta
            m_clusterChart.ClearData();
            m_driftChart.ClearData();

            UMCClusterLight cluster = matchedCluster.Cluster;

            // Then start populating the charts with data.
            m_mainCluster               = cluster;
            m_clusterChart.MainCluster  = matchedCluster;
            m_driftChart.MainCluster    = matchedCluster;

            // Then we find all the nearby clusters
            double massPpm  = ClusterTolerances.Mass;
            double net      = ClusterTolerances.RetentionTime;
            double minMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, massPpm);
            double maxMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, -massPpm);
            double minNet   = cluster.RetentionTime - net;
            double maxNet   = cluster.RetentionTime + net;


            List<UMCClusterLight> otherClusters
                = SingletonDataProviders.Providers.ClusterCache.FindNearby(minMass, maxMass, minNet, maxNet);

            // Remove self from the list
            int index = otherClusters.FindIndex(delegate(UMCClusterLight x)
            {
                return x.ID == cluster.ID;
            });

            if (index > -1)
            {
                otherClusters.RemoveAt(index);
            }


            // Then find the matching clusters and map them back to previously matched (to mass tag data)
            List<UMCClusterLightMatched> otherClusterMatches = new List<UMCClusterLightMatched>();
            otherClusters.ForEach(x => otherClusterMatches.Add(FeatureCacheManager<UMCClusterLightMatched>.FindById(x.ID)));

            foreach (UMCClusterLightMatched matchedOtherCluster in otherClusterMatches)
            {
                matchedOtherCluster.Cluster.Features.Clear();
                matchedOtherCluster.Cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, false, false);
            }

            m_clusterChart.AddAdditionalClusters(otherClusterMatches);
            m_driftChart.AddAdditionalClusters(otherClusterMatches);
            m_clusterChart.UpdateCharts(true);
            m_driftChart.UpdateCharts(true);

            m_clusterChart.AdjustViewPortWithTolerances(ClusterTolerances, false);
            m_driftChart.AdjustViewPortWithTolerances(ClusterTolerances, true);
            Viewport = m_clusterChart.ViewPort;

            // Map out the MS/MS spectra.
            List<MSFeatureMsMs> msmsFeatures = new List<MSFeatureMsMs>();
            foreach (UMCLight feature in cluster.Features)
            {
                foreach (MSFeatureLight msFeature in feature.MSFeatures)
                {
                    foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                    {
                        MSFeatureMsMs map = new MSFeatureMsMs();
                        map.FeatureID = msFeature.ID;
                        map.FeatureGroupID = msFeature.GroupID;
                        map.Mz = msFeature.Mz;
                        map.PrecursorMZ = spectrum.PrecursorMZ;
                        map.FeatureScan = msFeature.Scan;
                        map.MsMsScan = spectrum.Scan;
                        map.MassMonoisotopicAligned = msFeature.MassMonoisotopicAligned;
                        map.ChargeState = msFeature.ChargeState;
                        map.Sequence = "";
                        map.PeptideSequence = "";
                        msmsFeatures.Add(map);
                    }
                }
            }

            List<KeyValuePair<float, float>> distances = new List<KeyValuePair<float, float>>();
            List<UMCLight> features = new List<UMCLight>();
            features.AddRange(matchedCluster.Cluster.Features);

            Features.Clear();
            features.ForEach(x  => Features.Add(new UMCTreeViewModel(x)));
            SelectedFeature     =  Features[0];
        }
        #endregion        
    
        /// <summary>
        /// Gets or sets the PPM error 
        /// </summary>
        public double XicPpmError { get; set; }
    }
}
