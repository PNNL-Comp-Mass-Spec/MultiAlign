using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Extensions;
using PNNLControls;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmicsIO.IO;
using System.IO;
using System;
using System.Linq;
using MultiAlign.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MultiAlign.Windows.Viewers.Clusters
{


    /// <summary>
    /// Interaction logic for ClusterControl.xaml
    /// </summary>
    public partial class ClusterControl : UserControl, INotifyPropertyChanged
    {
        private System.Windows.Forms.SaveFileDialog  m_featureImageSaveDialog;
        private UMCClusterLight m_mainCluster;
        private bool m_adjustingFeaturePlots = false;
        private Dictionary<ctlChartBase, List<ChartSynchData>> m_chartSynchMap;
        private System.Windows.Forms.SaveFileDialog m_saveFileDialog;

        private int m_numberOfIsotopes;
        private int m_minScan;
        private int m_maxScan;

        private Dictionary<int, List<MSFeatureLight>> m_scanMaps;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClusterControl()
        {
            InitializeComponent();

            m_numberOfIsotopes  = 4;
            m_scanMaps          = new Dictionary<int, List<MSFeatureLight>>();
            Charges             = new ObservableCollection<int>();
            m_chartSynchMap     = new Dictionary<ctlChartBase, List<ChartSynchData>>();

            FeatureFindingTolerances                = new FeatureTolerances();
            FeatureFindingTolerances.Mass           = 10;
            FeatureFindingTolerances.DriftTime      = 3;
            FeatureFindingTolerances.RetentionTime  = 50;


            ClusterTolerances               = new FeatureTolerances();
            ClusterTolerances.Mass          = 10;
            ClusterTolerances.DriftTime     = 3;
            ClusterTolerances.RetentionTime = .03;

            m_featureImageSaveDialog        = new System.Windows.Forms.SaveFileDialog();
            
            
            m_sicChart.ViewPortChanged      += new PNNLControls.ViewPortChangedHandler(m_sicChart_ViewPortChanged);
            m_sicChart.ChartPointPressed    += new System.EventHandler<SelectedPointEventArgs>(m_sicChart_ChartPointPressed);     

            m_msFeaturePlot.ViewPortChanged += new PNNLControls.ViewPortChangedHandler(m_msFeaturePlot_ViewPortChanged);

            m_clusterChart.ViewPortChanged  += new PNNLControls.ViewPortChangedHandler(m_clusterChart_ViewPortChanged);
            m_driftChart.ViewPortChanged    += new PNNLControls.ViewPortChangedHandler(m_driftChart_ViewPortChanged);
                        
            m_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            m_saveFileDialog.Filter = "DTA (*.dta)|*.dta|MGF (*.mgf)|*.mgf";

            Features = new ObservableCollection<UMCLight>();
        }

        

        void m_sicChart_ChartPointPressed(object sender, SelectedPointEventArgs e)
        {
            // Figure out the closest spectra to view.            
            if (e != null && e.SelectedPoint != null)
            {
                float x = e.SelectedPoint.X;

                if (this.UMCFeature != null)
                {
                    int point           = Convert.ToInt32(x);
                    MSFeatureLight closest   = null;
                    int closestDist     = int.MaxValue;
                    foreach (MSFeatureLight feature in UMCFeature.MSFeatures)
                    {
                        int diff = Math.Abs(feature.Scan - point);
                        if (closest == null || diff < closestDist && (SelectedCharge == feature.ChargeState))
                        {
                            closest     = feature; 
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

        public bool UsesDriftTime
        {
            get { return (bool)GetValue(UsesDriftTimeProperty); }
            set { SetValue(UsesDriftTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsesDriftTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsesDriftTimeProperty =
            DependencyProperty.Register("UsesDriftTime", typeof(bool), typeof(ClusterControl), new UIPropertyMetadata(false));



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
        void sender_ViewPortChanged(ctlChartBase chart, ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;
            m_adjustingFeaturePlots = true;

            if (!m_chartSynchMap.ContainsKey(chart))
                return;

            foreach (ChartSynchData data in m_chartSynchMap[chart]) 
            {
                RectangleF senderView   = args.ViewPort;                
                ctlChartBase target     = data.Target; 
                RectangleF targetView   = target.ViewPort;            
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

            
            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   m_clusterChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_clusterChart.ViewPort.Width,                                                            
                                                            otherView.Height);
            m_clusterChart.ViewPort     = newViewport;
            m_adjustingFeaturePlots     = false;

            Viewport = m_clusterChart.ViewPort;
        }
        void m_clusterChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {

            if (m_adjustingFeaturePlots) return;
            
            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            
            RectangleF newViewport      = new RectangleF(   m_driftChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_driftChart.ViewPort.Width,                                                            
                                                            otherView.Height);
            m_driftChart.ViewPort       = newViewport;
            m_adjustingFeaturePlots     = false;

            Viewport = m_clusterChart.ViewPort;
        }        
        void m_msFeaturePlot_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots     = true;            
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   otherView.X,
                                                            m_sicChart.ViewPort.Y, 
                                                            otherView.Width,
                                                            m_sicChart.ViewPort.Height);
            m_sicChart.ViewPort         = newViewport;
            m_adjustingFeaturePlots     = false;
        }
        void m_sicChart_ViewPortChanged(PNNLControls.ctlChartBase chart, PNNLControls.ViewPortChangedEventArgs args)
        {
            if (m_adjustingFeaturePlots) return;

            m_adjustingFeaturePlots     = true;
            RectangleF otherView        = args.ViewPort;
            RectangleF newViewport      = new RectangleF(   otherView.X, 
                                                            m_msFeaturePlot.ViewPort.Y,
                                                            otherView.Width, 
                                                            m_msFeaturePlot.ViewPort.Height);
            m_msFeaturePlot.ViewPort    = newViewport;
            m_adjustingFeaturePlots     = false;            
        }
        #endregion

        
        private string m_selectedFeatureName;

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
                    //OnPropertyChanged("SelectedFeatureName");
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
                    OnNotify("NumberOfIsotopes");
                }
            }
        }

        #region Dependency Property Setters
        /// <summary>
        /// Updates the single feature plots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FeatureSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;

            if (e.NewValue != null)
            {                                                
                UMCLight feature = e.NewValue as UMCLight;

                if (feature != null)
                    thisSender.SetFeature(feature);
            }
        }
        private static void SpectraSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;

            if (e.NewValue != null)
            {                                                
                MSSpectra feature = e.NewValue as MSSpectra;

            }
        }                
        private void SetFeature(UMCLight feature)
        {
            m_adjustingFeaturePlots                  = true;
            DatasetInformation info                  = SingletonDataProviders.GetDatasetInformation(feature.GroupID);

            if (info != null)
            {
                SelectedFeatureName = info.DatasetName;
            }

            m_msFeaturePlot.MainFeature = feature;

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

        #region View Model Stuff
        private void UpdateCharges(UMCLight feature)
        {
            Charges.Clear();

            m_scanMaps = feature.CreateChargeMap();

            foreach (int charge in m_scanMaps.Keys)            
            {
                Charges.Add(charge);
                
            }
            Charges.OrderBy(x => x);
            if (Charges.Count > 0)
            {
                m_charge        = -1;
                SelectedCharge  = Charges[0];
            }
        }
        private int m_charge;
        public int SelectedCharge
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
                    OnNotify("SelectedCharge");

                    if (m_scanMaps.ContainsKey(value))
                    {
                        int minScan = int.MaxValue;
                        int maxScan = int.MinValue;
                        foreach (MSFeatureLight msFeature in m_scanMaps[value])
                        {
                            minScan = Math.Min(minScan, msFeature.Scan);
                            maxScan = Math.Max(maxScan, msFeature.Scan);
                        }

                        if (m_scanMaps[value].Count > 0)
                        {
                            MinimumScan = minScan;
                            MaximumScan = maxScan;
                        }
                    }
                }
            }
        }

        //TODO:Put this all in a view model
        public ObservableCollection<int> Charges { get; set; }

        public ObservableCollection<UMCLight> Features { get; set; }

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
                    OnNotify("MinimumScan");
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
                    OnNotify("MaximumScan");
                }
            }
        }
        private void OnNotify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        #region Spectrum Loading 
        
        private void LoadSpectrum(MSFeatureLight msFeature)
        {
            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(msFeature.GroupID);            
            if (info != null && info.Raw != null && info.RawPath != null)
            {
                Dictionary<int, List<MSFeatureLight>> features = msFeature.ParentFeature.CreateChargeMap();
                
                int N = features[msFeature.ChargeState].Count;
                if (!features.ContainsKey(msFeature.ChargeState) || N < 1)
                    return;

                double sum          = 0;
                double min          = double.MaxValue;
                double max          = double.MinValue;
                double maxIntensity = 0;

                foreach (MSFeatureLight chargeFeature in features[msFeature.ChargeState])
                {
                    min          = Math.Min(min, chargeFeature.Mz);
                    max          = Math.Max(max, chargeFeature.Mz);
                    maxIntensity = Math.Max(chargeFeature.Abundance, maxIntensity);
                    sum += chargeFeature.Mz;                    
                }
                maxIntensity += (maxIntensity * .1);

                double averageMz    = sum / Convert.ToDouble(N);
                double spacing      = (1.0 / Convert.ToDouble(msFeature.ChargeState));
                double adder        = spacing * m_numberOfIsotopes;
                double lowMz        = averageMz - spacing;
                double highMz       = averageMz + adder;

                List<XYData> spectrum = ParentSpectraFinder.GetParentSpectrum(info.RawPath,
                                                                                msFeature.Scan,
                                                                                lowMz,
                                                                                highMz);

                m_parentSpectra.SetFeature(msFeature, spectrum);
                m_parentSpectra.AutoViewPort();                
            }
        }
        #endregion


        /// <summary>
        /// Grabs the data from the provider cache and shoves into UI where needed.  This is done
        /// here like this to prevent holding large amounts of data in memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SetCluster(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisSender = (ClusterControl)sender;
            if (e.NewValue != null)
            {
                thisSender.m_adjustingFeaturePlots = true;

                // Grab the data from the cache
                UMCClusterLightMatched matchedCluster = (UMCClusterLightMatched)e.NewValue;                             
                thisSender.UpdatePlotsWithClusterData(matchedCluster);                
                thisSender.m_adjustingFeaturePlots = false;
            }
        }
        #endregion

        /// <summary>
        /// Dependency Property
        /// </summary>
        public UMCClusterLightMatched Cluster
        {
            get 
            {
                return (UMCClusterLightMatched)GetValue(ClusterProperty);
            }
            set 
            { 
                SetValue(ClusterProperty, value);
            }
        }                

        // Using a DependencyProperty as the backing store for Cluster.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClusterProperty =            
            DependencyProperty.Register("Cluster",
                                        typeof(UMCClusterLightMatched), 
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(SetCluster)));



        public RectangleF Viewport
        {
            get { return (RectangleF)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Viewport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(RectangleF), typeof(ClusterControl));



        public MSSpectra SelectedSpectrum
        {
            get { return (MSSpectra)GetValue(SelectedSpectraProperty); }
            set { SetValue(SelectedSpectraProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedSpectra.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSpectraProperty =
            DependencyProperty.Register("SelectedSpectrum", 
                                        typeof(MSSpectra), 
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(SpectraSet)));



        /// <summary>
        /// Gets or sets the UMC feature that was selected to view in the feature viewer
        /// </summary>
        public UMCLight UMCFeature
        {
            get
            {
                return (UMCLight)GetValue(FeatureProperty);
            }
            set
            {
                SetValue(FeatureProperty, value);
            }
        }
        
        public static readonly DependencyProperty FeatureProperty     =
            DependencyProperty.Register("UMCFeature",
                                        typeof(UMCLight),
                                        typeof(ClusterControl),
                                        new FrameworkPropertyMetadata(new PropertyChangedCallback(FeatureSet)));

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
            m_mainCluster                   = cluster;
            m_clusterChart.MainCluster      = matchedCluster;
            m_driftChart.MainCluster        = matchedCluster;
                                   
            // Then we find all the nearby clusters
            double massPpm  = ClusterTolerances.Mass;
            double net      = ClusterTolerances.RetentionTime;
            double minMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, massPpm);
            double maxMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, -massPpm);
            double minNet   = cluster.RetentionTime - net;
            double maxNet   = cluster.RetentionTime + net;

            Features.Clear();
            matchedCluster.Cluster.Features.ForEach(x => Features.Add(x));

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

            m_clusterChart.AdjustViewPortWithTolerances(ClusterTolerances,  false);
            m_driftChart.AdjustViewPortWithTolerances(ClusterTolerances,    true);
            Viewport = m_clusterChart.ViewPort;

            // Map out the MS/MS spectra.
            List<MSFeatureMsMs> msmsFeatures = new List<MSFeatureMsMs>();
            foreach (UMCLight feature in cluster.Features)
            {
                foreach (MSFeatureLight msFeature in feature.MSFeatures)
                {
                    foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                    {
                        MSFeatureMsMs map           = new MSFeatureMsMs();
                        map.FeatureID               = msFeature.ID;
                        map.FeatureGroupID          = msFeature.GroupID;
                        map.Mz                      = msFeature.Mz;
                        map.PrecursorMZ             = spectrum.PrecursorMZ;                     
                        map.FeatureScan             = msFeature.Scan;
                        map.MsMsScan                = spectrum.Scan;
                        map.MassMonoisotopicAligned = msFeature.MassMonoisotopicAligned;
                        map.ChargeState             = msFeature.ChargeState;
                        map.Sequence                = "";
                        map.PeptideSequence         = "";
                        msmsFeatures.Add(map);
                    }
                }
            }

            List<KeyValuePair<float, float>> distances = new List<KeyValuePair<float, float>>();
            List<UMCLight> features = new List<UMCLight>();
            features.AddRange(matchedCluster.Cluster.Features);                       
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
    
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
