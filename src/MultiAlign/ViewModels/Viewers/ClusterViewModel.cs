using System.Linq;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlign.ViewModels.Features;
using MultiAlign.ViewModels.TreeView;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Extensions;
using MultiAlignCore.IO.Features;
using MultiAlignCustomControls.Charting;
using MultiAlignCustomControls.Extensions;
using PNNLControls;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
        private readonly WindowsFormsHost m_clusterChartHost;
        private readonly WindowsFormsHost m_driftChartHost;
        private UMCTreeViewModel        m_selectedFeature;
        private UMCClusterTreeViewModel m_clusterViewModel;

        private int m_numberOfIsotopes;
        private int m_minScan;
        private int m_maxScan;
        private Dictionary<int, List<MSFeatureLight>> m_scanMaps;

        readonly SingleClusterChart m_clusterChart;
        readonly SingleClusterChart m_driftChart;
        private bool m_usesDriftTime;


        public ClusterDetailViewModel()
        {
            m_numberOfIsotopes  = 4;
            m_scanMaps          = new Dictionary<int, List<MSFeatureLight>>();
            Charges             = new ObservableCollection<ChargeStateViewModel>();
            
            FeatureFindingTolerances                = new FeatureTolerances
            {
                Mass = 10,
                DriftTime = 3,
                RetentionTime = 50
            };
            ClusterTolerances                       = new FeatureTolerances
            {
                Mass = 10,
                DriftTime = 3,
                RetentionTime = .03
            };


            m_clusterChart                  = new SingleClusterChart
            {
                TitleVisible = false,
                XAxisShortHand = "NET",
                YAxisShortHand = "ppm",
                LegendVisible = false
            };
            m_clusterChartHost              = new WindowsFormsHost{ Child = m_clusterChart };

            m_driftChart                    = new SingleClusterChart
            {
                TitleVisible = false,
                XAxisShortHand = "ms",
                YAxisShortHand = "ppm",
                XAxisLabel = "Drift Time (ms)",
                LegendVisible = false
            };
            m_driftChartHost                = new WindowsFormsHost{ Child = m_driftChart };


            m_clusterChart.ViewPortChanged  += (m_clusterChart_ViewPortChanged);
            m_driftChart.ViewPortChanged    += (m_driftChart_ViewPortChanged);

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
        #endregion

        public PlotModelBase XicModel
        {
            get { return m_model; }
            set
            {
                m_model = value;
                
                OnPropertyChanged("XicModel");
            }
        }

        public UmcClusterChargeHistogram ChargeHistogramModel
        {
            get
            {
                return m_chargeStateHistogramModel;
            }
            set 
            { 
                m_chargeStateHistogramModel = value;
                OnPropertyChanged("ChargeHistogramModel");
            }
        }

        /// <summary>
        /// Gets or sets the current cluster.
        /// </summary>
        public UMCClusterLight Cluster { get; set; }

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

            DatasetInformation info = SingletonDataProviders.GetDatasetInformation(feature.GroupID);

            if (info != null)
            {
                SelectedFeatureName = info.DatasetName;
            }

            var model = new XicViewModel(new List<UMCLight> {feature}, "XIC");
            model.PointClicked += model_PointClicked;
            XicModel = model;
            UpdateCharges(feature);
            
        }

        void model_PointClicked(object sender, PositionArgs e)
        {
            var best = FindFeature(e.X);
            if (best != null)
            {
                LoadSpectrum(best);
            }
        }

        #region Chart Event Handlers

        #region Viewport synching
        void m_driftChart_ViewPortChanged(ctlChartBase chart, ViewPortChangedEventArgs args)
        {
            RectangleF otherView = args.ViewPort;
            var newViewport = new RectangleF(m_clusterChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_clusterChart.ViewPort.Width,
                                                            otherView.Height);
            m_clusterChart.ViewPort = newViewport;

            Viewport = m_clusterChart.ViewPort;
        }
        void m_clusterChart_ViewPortChanged(ctlChartBase chart, ViewPortChangedEventArgs args)
        {
            RectangleF otherView = args.ViewPort;

            var newViewport = new RectangleF(m_driftChart.ViewPort.X,
                                                            otherView.Y,
                                                            m_driftChart.ViewPort.Width,
                                                            otherView.Height);
            m_driftChart.ViewPort = newViewport;

            Viewport = m_clusterChart.ViewPort;
        }        
        #endregion
        #endregion

        private string m_selectedFeatureName;
        private PlotModelBase m_model;
        private PlotModelBase m_parentSpectrumViewModel;
        private UmcClusterChargeHistogram m_chargeStateHistogramModel;


        public PlotModelBase ParentSpectrumViewModel
        {
            get { return m_parentSpectrumViewModel; }
            set
            {
                m_parentSpectrumViewModel = value;
                OnPropertyChanged("ParentSpectrumViewModel");
            }
        }

        /// <summary>
        /// Grabs the data from the provider cache and shoves into UI where needed.  This is done
        /// here like this to prevent holding large amounts of data in memory.
        /// </summary>
        private void SetCluster(UMCClusterLightMatched cluster)
        {
            // Grab the data from the cache            
            UpdatePlotsWithClusterData(cluster); 

            ChargeHistogramModel = new UmcClusterChargeHistogram(cluster.Cluster, "Charge State Histogram");
        }
        private void UpdateCharges(UMCLight feature)
        {
            Charges.Clear();
            m_scanMaps = feature.CreateChargeMap();

            foreach (var charge in m_scanMaps.Keys)
            {
                double mz           = 0;
                int  minScan        = int.MaxValue;
                int  maxScan        = int.MinValue;
                long maxIntensity   = 0;

                foreach (var msFeature in m_scanMaps[charge])
                {
                    minScan = Math.Min(minScan, msFeature.Scan);
                    maxScan = Math.Max(maxScan, msFeature.Scan);
                    if (maxIntensity >= msFeature.Abundance) continue;

                    maxIntensity = msFeature.Abundance;
                    mz           = msFeature.Mz;
                }

                Charges.Add(new ChargeStateViewModel(charge, mz, minScan, maxScan));
            }
            if (Charges.Count <= 0) return;
            SelectedCharge = Charges[0];

        }

        private MSFeatureLight FindFeature(double x)
        {
            MSFeatureLight best = null;

            if (SelectedCharge == null)
                return null;

            if (!m_scanMaps.ContainsKey(SelectedCharge.ChargeState)) return best;

            var bestDist = double.MaxValue;
            foreach (var msFeature in m_scanMaps[SelectedCharge.ChargeState])
            {
                var dist = Math.Abs(msFeature.Scan - x);
                if (!(dist < bestDist)) continue;
                best = msFeature;
                bestDist = dist;
            }
            return best;
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

                    if (!m_scanMaps.ContainsKey(value.ChargeState)) return;
                    var minScan = int.MaxValue;
                    var maxScan = int.MinValue;
                    long bestAbundance = 0;
                    MSFeatureLight bestFeature = null;
                    foreach (var msFeature in m_scanMaps[value.ChargeState])
                    {
                        minScan = Math.Min(minScan, msFeature.Scan);
                        maxScan = Math.Max(maxScan, msFeature.Scan);
                        if (msFeature.Abundance < bestAbundance) continue;
                        bestAbundance = msFeature.Abundance;
                        bestFeature   = msFeature ;
                    }
                    if (m_scanMaps[value.ChargeState].Count <= 0) return;

                    MinimumScan = minScan;
                    MaximumScan = maxScan;

                    if (bestFeature != null)
                        LoadSpectrum(bestFeature);
                }
            }
        }
        public ObservableCollection<ChargeStateViewModel> Charges
        { 
            get;
            private set; 
        }
        public ICommand CreateXic { get; set; }
        
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
        private void LoadSpectrum(MSFeatureLight msFeature)
        {
            var info = SingletonDataProviders.GetDatasetInformation(msFeature.GroupID);
            if (info == null || info.Raw == null || info.RawPath == null) return;
            
            var mz       = msFeature.Mz;  
            var charge   = msFeature.ChargeState;
            var spacing  = 1.0/Convert.ToDouble(charge);
            var lowMz    = mz - spacing*3;
            var highMz   = mz + spacing * (NumberOfIsotopes + 1);

            var spectrum = ParentSpectraFinder.GetParentSpectrum(info.RawPath,
                                                                msFeature.Scan,
                                                                lowMz,
                                                                highMz);
            if ( spectrum == null)
                return;

            var name = string.Format("Scan {0} Charge {1} Dataset {2}",
                                                    msFeature.Scan,
                                                    msFeature.ChargeState,
                                                    msFeature.GroupID
                                                    );
            
           var msFeatureSpectra = new MsFeatureSpectraViewModel(msFeature, spectrum, name);
           msFeatureSpectra.SetXExtrema(lowMz, highMz);
           ParentSpectrumViewModel = msFeatureSpectra;

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
        private void UpdatePlotsWithClusterData(UMCClusterLightMatched matchedCluster)
        {
            // Clear the charta
            m_clusterChart.ClearData();
            m_driftChart.ClearData();

            var cluster = matchedCluster.Cluster;

            m_clusterChart.MainCluster  = matchedCluster;
            m_driftChart.MainCluster    = matchedCluster;

            // Then we find all the nearby clusters
            var massPpm  = ClusterTolerances.Mass;
            var net      = ClusterTolerances.RetentionTime;
            var minMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, massPpm);
            var maxMass  = Feature.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, -massPpm);
            var minNet   = cluster.RetentionTime - net;
            var maxNet   = cluster.RetentionTime + net;


            List<UMCClusterLight> otherClusters
                = SingletonDataProviders.Providers.ClusterCache.FindNearby(minMass, maxMass, minNet, maxNet);

            // Remove self from the list
            int index = otherClusters.FindIndex(x => x.ID == cluster.ID);

            if (index > -1)
            {
                otherClusters.RemoveAt(index);
            }


            // Then find the matching clusters and map them back to previously matched (to mass tag data)
            var otherClusterMatches = new List<UMCClusterLightMatched>();
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
            var msmsFeatures = new List<MSFeatureMsMs>();
            foreach (UMCLight feature in cluster.Features)
            {
                foreach (MSFeatureLight msFeature in feature.MSFeatures)
                {
                    msmsFeatures.AddRange(msFeature.MSnSpectra.Select(spectrum => new MSFeatureMsMs
                    {
                        FeatureID = msFeature.ID, 
                        FeatureGroupID = msFeature.GroupID,
                        Mz = msFeature.Mz,
                        PrecursorMZ = spectrum.PrecursorMZ, 
                        FeatureScan = msFeature.Scan, 
                        MsMsScan = spectrum.Scan, 
                        MassMonoisotopicAligned = msFeature.MassMonoisotopicAligned, 
                        ChargeState = msFeature.ChargeState, 
                        Sequence = "", 
                        PeptideSequence = ""
                    }));
                }
            }

            
            var features = new List<UMCLight>();
            features.AddRange(matchedCluster.Cluster.Features);

            Features.Clear();
            features.ForEach(x  => Features.Add(new UMCTreeViewModel(x)));
            SelectedFeature     =  Features[0];
        }
        #endregion           
    }
}
