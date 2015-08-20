using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MultiAlign.IO;
using MultiAlign.ViewModels.Charting;
using MultiAlign.ViewModels.Features;
using MultiAlign.ViewModels.TreeView;
using MultiAlignCore.Data.Features;
using MultiAlignCore.Data.MetaData;
using MultiAlignCore.Drawing;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.MsMs;
using PNNLOmics.Algorithms;
using PNNLOmics.Annotations;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Extensions;

namespace MultiAlign.ViewModels.Viewers
{
    /// <summary>
    ///     View model for the main cluste control viewer
    /// </summary>
    public class ClusterDetailViewModel : ViewModelBase
    {
        private ChargeStateViewModel m_charge;
        private UmcClusterChargeHistogram m_chargeStateHistogramModel;
        private PlotBase m_clusterDriftModel;
        private PlotBase m_clusterMassModel;
        private UMCClusterTreeViewModel m_clusterViewModel;

        private int m_maxScan;
        private int m_minScan;
        private PlotModelBase m_model;
        private int m_numberOfIsotopes;
        private PlotModelBase m_parentSpectrumViewModel;
        private Dictionary<int, List<MSFeatureLight>> m_scanMaps;
        private UMCTreeViewModel m_selectedFeature;
        private string m_selectedFeatureName;

        private bool m_usesDriftTime;


        public ClusterDetailViewModel()
        {
            m_numberOfIsotopes = 4;
            m_scanMaps = new Dictionary<int, List<MSFeatureLight>>();
            Charges = new ObservableCollection<ChargeStateViewModel>();

            FeatureFindingTolerances = new FeatureTolerances
            {
                Mass = 10,
                DriftTime = 3,
                Net = 50
            };
            ClusterTolerances = new FeatureTolerances
            {
                Mass = 10,
                DriftTime = 3,
                Net = .03
            };


            Features = new ObservableCollection<UMCTreeViewModel>();
        }

        [UsedImplicitly]
        public PlotModelBase XicModel
        {
            get { return m_model; }
            set
            {
                m_model = value;
                OnPropertyChanged("XicModel");
            }
        }

        [UsedImplicitly]
        public UmcClusterChargeHistogram ChargeHistogramModel
        {
            get { return m_chargeStateHistogramModel; }
            set
            {
                m_chargeStateHistogramModel = value;
                OnPropertyChanged("ChargeHistogramModel");
            }
        }

        /// <summary>
        ///     Gets or sets the current cluster.
        /// </summary>
        public UMCClusterLight Cluster { get; set; }

        public UMCTreeViewModel SelectedFeature
        {
            get { return m_selectedFeature; }
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

        public MSSpectra SelectedSpectrum { get; set; }

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
        ///     Dependency Property
        /// </summary>
        public UMCClusterTreeViewModel SelectedCluster
        {
            get { return m_clusterViewModel; }
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

        public PlotBase ClusterDriftPlot
        {
            get { return m_clusterDriftModel; }
            set
            {
                if (value == null || value == m_clusterDriftModel)
                    return;

                m_clusterDriftModel = value;
                OnPropertyChanged("ClusterDriftPlot");
            }
        }

        public PlotBase ClusterMassPlot
        {
            get { return m_clusterMassModel; }
            set
            {
                if (value == null || value == m_clusterMassModel)
                    return;

                m_clusterMassModel = value;
                OnPropertyChanged("ClusterMassPlot");
            }
        }

        /// <summary>
        ///     Updates the plots with data stored in the cache.
        /// </summary>
        private void UpdatePlotsWithClusterData(UMCClusterLightMatched matchedCluster)
        {
            //TODO: Make this select mass!
            ClusterMassPlot = ScatterPlotFactory.CreateFeatureMassScatterPlot(matchedCluster.Cluster.Features);

            //TODO: Make this select drift time!
            ClusterDriftPlot = ScatterPlotFactory.CreateFeatureDriftTimeScatterPlot(matchedCluster.Cluster.Features);

            var cluster = matchedCluster.Cluster;

            // Then we find all the nearby clusters
            var massPpm = ClusterTolerances.Mass;
            var net = ClusterTolerances.Net;


            //TODO: Add other clusters back
            // var minMass = FeatureLight.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, massPpm);
            //var maxMass = FeatureLight.ComputeDaDifferenceFromPPM(cluster.MassMonoisotopic, -massPpm);
            //var minNet = cluster.Net - net;
            //var maxNet = cluster.Net + net;

            //var otherClusters
            //    = SingletonDataProviders.Providers.ClusterCache.FindNearby(minMass, maxMass, minNet, maxNet);

            //// Remove self from the list
            //var index = otherClusters.FindIndex(x => x.Id == cluster.Id);

            //if (index > -1)
            //{
            //    otherClusters.RemoveAt(index);
            //}


            //// Then find the matching clusters and map them back to previously matched (to mass tag data)
            //var otherClusterMatches = new List<UMCClusterLightMatched>();
            //otherClusters.ForEach(x => otherClusterMatches.Add(FeatureCacheManager<UMCClusterLightMatched>.FindById(x.Id)));

            //foreach (var matchedOtherCluster in otherClusterMatches)
            //{
            //    matchedOtherCluster.Cluster.Features.Clear();
            //    matchedOtherCluster.Cluster.ReconstructUMCCluster(SingletonDataProviders.Providers, false, false);
            //}


            // Map out the MS/MS spectra.
            var msmsFeatures = new List<MSFeatureMsMs>();
            foreach (var feature in cluster.Features)
            {
                foreach (var msFeature in feature.MsFeatures)
                {
                    msmsFeatures.AddRange(msFeature.MSnSpectra.Select(spectrum => new MSFeatureMsMs
                    {
                        FeatureID = msFeature.Id,
                        FeatureGroupID = msFeature.GroupId,
                        Mz = msFeature.Mz,
                        PrecursorMZ = spectrum.PrecursorMz,
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
            features.ForEach(x => Features.Add(new UMCTreeViewModel(x)));
            SelectedFeature = Features[0];
        }

        #endregion

        #region Properties

        public string SelectedFeatureName
        {
            get { return m_selectedFeatureName; }
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
            get { return m_usesDriftTime; }
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
        ///     Gets or sets the number of isotopes to display.
        /// </summary>
        public int NumberOfIsotopes
        {
            get { return m_numberOfIsotopes; }
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
        ///     Cluster tolerances
        /// </summary>
        public FeatureTolerances ClusterTolerances { get; set; }

        /// <summary>
        ///     Feature finding tolerances
        /// </summary>
        public FeatureTolerances FeatureFindingTolerances { get; set; }

        #endregion

        private void SetFeature(UMCLight feature)
        {
            if (feature == null)
                return;

            var info = SingletonDataProviders.GetDatasetInformation(feature.GroupId);

            if (info != null)
            {
                SelectedFeatureName = info.DatasetName;
            }

            var model = new XicViewModel(new List<UMCLight> { feature }, "XIC");
            model.PointClicked += model_PointClicked;
            XicModel = model;
            UpdateCharges(feature);
        }

        private void model_PointClicked(object sender, PositionArgs e)
        {
            var best = FindFeature(e.X);
            if (best != null)
            {
                LoadSpectrum(best);
            }
        }

        /// <summary>
        ///     Grabs the data from the provider cache and shoves into UI where needed.  This is done
        ///     here like this to prevent holding large amounts of data in memory.
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
                double mz = 0;
                var minScan = int.MaxValue;
                var maxScan = int.MinValue;
                double maxIntensity = 0;

                foreach (var msFeature in m_scanMaps[charge])
                {
                    minScan = Math.Min(minScan, msFeature.Scan);
                    maxScan = Math.Max(maxScan, msFeature.Scan);

                    if (maxIntensity >= msFeature.Abundance)
                        continue;

                    maxIntensity = msFeature.Abundance;
                    mz = msFeature.Mz;
                }

                Charges.Add(new ChargeStateViewModel(charge, mz, minScan, maxScan));
            }
            if (Charges.Count <= 0)
                return;

            SelectedCharge = Charges[0];
        }

        private MSFeatureLight FindFeature(double x)
        {
            MSFeatureLight best = null;

            if (SelectedCharge == null)
                return null;

            if (!m_scanMaps.ContainsKey(SelectedCharge.ChargeState))
                return best;

            var bestDist = double.MaxValue;
            foreach (var msFeature in m_scanMaps[SelectedCharge.ChargeState])
            {
                var dist = Math.Abs(msFeature.Scan - x);
                if (!(dist < bestDist))
                    continue;

                best = msFeature;
                bestDist = dist;
            }
            return best;
        }

        #region View Model

        public ChargeStateViewModel SelectedCharge
        {
            get { return m_charge; }
            set
            {
                if (m_charge != value)
                {
                    m_charge = value;
                    OnPropertyChanged("SelectedCharge");

                    if (value == null)
                        return;

                    if (!m_scanMaps.ContainsKey(value.ChargeState))
                        return;

                    var minScan = int.MaxValue;
                    var maxScan = int.MinValue;
                    double bestAbundance = 0;
                    MSFeatureLight bestFeature = null;
                    foreach (var msFeature in m_scanMaps[value.ChargeState])
                    {
                        minScan = Math.Min(minScan, msFeature.Scan);
                        maxScan = Math.Max(maxScan, msFeature.Scan);
                        if (msFeature.Abundance < bestAbundance)
                            continue;

                        bestAbundance = msFeature.Abundance;
                        bestFeature = msFeature;
                    }

                    if (m_scanMaps[value.ChargeState].Count <= 0)
                        return;

                    MinimumScan = minScan;
                    MaximumScan = maxScan;

                    if (bestFeature != null)
                        LoadSpectrum(bestFeature);
                }
            }
        }

        public ObservableCollection<ChargeStateViewModel> Charges { get; private set; }
        public ICommand CreateXic { get; set; }

        public ObservableCollection<UMCTreeViewModel> Features { get; private set; }

        public int MinimumScan
        {
            get { return m_minScan; }
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
            get { return m_maxScan; }
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
            var info = SingletonDataProviders.GetDatasetInformation(msFeature.GroupId);
            if (info == null || info.Raw == null || info.RawPath == null)
                return;


            var mz = msFeature.Mz;
            var charge = msFeature.ChargeState;
            var spacing = 1.0 / Convert.ToDouble(charge);
            var lowMz = mz - spacing * 3;
            var highMz = mz + spacing * (NumberOfIsotopes + 1);

            var spectrum = ParentSpectraFinder.GetParentSpectrum(info.RawPath,
                msFeature.Scan,
                lowMz,
                highMz);
            if (spectrum == null)
                return;

            var name = string.Format("Scan {0} Charge {1} Dataset {2}",
                msFeature.Scan,
                msFeature.ChargeState,
                msFeature.GroupId
                );

            var msFeatureSpectra = new MsFeatureSpectraViewModel(msFeature, spectrum, name);
            msFeatureSpectra.SetXExtrema(lowMz, highMz);
            ParentSpectrumViewModel = msFeatureSpectra;
        }

        #endregion
    }
}