using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Algorithms.Chromatograms;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;
using PNNLOmics.Annotations;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.Clustering
{
    public enum MassComparison
    {
        Mz,
        Monoisotopic
    };

    /// <summary>
    /// Finds clusters using a tree aproach
    /// </summary>
    /// <typeparam name="TChildFeature"></typeparam>
    /// <typeparam name="TParentFeature"></typeparam>
    public class MsFeatureTreeClusterer<TChildFeature, TParentFeature>
        : IProgressNotifer, IClusterer<TChildFeature, TParentFeature>
        where TChildFeature : FeatureLight, new()
        where TParentFeature : FeatureLight, IFeatureCluster<TChildFeature>, new()
        
        
    {
        private const int CONST_SCAN_TOLERANCE = 30; // was 50

        private readonly Comparison<TChildFeature> sortFunction;

        private readonly Func<TChildFeature, TChildFeature, double> massDiffFunc;

        private readonly Func<FeatureLight, FeatureLight, int> comparison;

        private readonly double massTolerance;

        /// <summary>
        /// Constructor
        /// </summary>
        public MsFeatureTreeClusterer()
        {
            Tolerances          = new FeatureTolerances();
            ScanTolerance       = CONST_SCAN_TOLERANCE;
            FilteringOptions    = new LcmsFeatureFilteringOptions();
        }

        public MsFeatureTreeClusterer(Comparison<TChildFeature> sortFunction,
                                      Func<TChildFeature, TChildFeature, double> massDiffFunc,
                                      MassComparison comparisonMethod,
                                      double massTolerance) : this()
        {
            this.sortFunction = sortFunction;
            this.massDiffFunc = massDiffFunc;
            if (comparisonMethod == MassComparison.Mz)
            {
                this.comparison = CompareMz;
            }
            else
            {
                this.comparison = CompareMonoisotopic;
            }

            this.massTolerance = massTolerance;
        }

            /// <summary>
        /// Gets or sets the filtering options
        /// </summary>
        [UsedImplicitly]
        public LcmsFeatureFilteringOptions FilteringOptions { get; set; }

        /// <summary>
        /// Gets or sets the tolerances
        /// </summary>
        [UsedImplicitly]
        public FeatureTolerances Tolerances
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the value between 
        /// </summary>
        [UsedImplicitly]
        public int ScanTolerance { get; set; }

        /// <summary>
        /// Gets or sets the object that can be used to go back to the raw data to grab XIC's
        /// </summary>
        public InformedProteomicsReader SpectraProvider { get; set; }

        public List<TParentFeature> Cluster(List<TChildFeature> data, List<TParentFeature> clusters, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clusters features based on some specified values. 
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public List<TParentFeature> Cluster(List<TChildFeature> features, IProgress<ProgressData> progress = null)
        {
            var progressData = new ProgressData(progress);
            var clusters            = new List<TParentFeature>();            
            var currentIndex        = 0;
            int i = 0;
            //var n                   = features.Count();

            // Sort the features based on m/z first
            //var msFeatures = new List<TChild>();
            //msFeatures.AddRange(features);
            var msFeatures = features.ToList();
            var n = msFeatures.Count;

            OnProgress("Sorting features for partitioning step");
            msFeatures.Sort(sortFunction);

            OnProgress("Examining features within partitions");

            // Iterate through all of the features
            while (currentIndex < n)
            {
                var hasGap       = false;
                var lastFeature  = msFeatures[currentIndex];
                var lastIndex    = currentIndex + 1;
                var gapFeatures  = new List<TChildFeature> { lastFeature };
                while (!hasGap && lastIndex < n)
                {
                    i++;
                    var currentFeature      = msFeatures[lastIndex];
                    var massDiff         = this.massDiffFunc(currentFeature, lastFeature);

                    // Time to quit
                    if (Math.Abs(massDiff) > massTolerance)
                    {
                        // Stop here...
                        hasGap       = true;
                    }
                    else
                    {
                        // Increment and save this feature
                        lastIndex++;
                        gapFeatures.Add(currentFeature);
                        lastFeature = currentFeature;
                    }
                }
                currentIndex = lastIndex;

                // Now that we have a gap...let's go a head and start building the features
                // first we build a scan dictionary 
                // sorted by scans
                var featureMap = new Dictionary<int, List<TChildFeature>>();
                foreach (var feature in gapFeatures)
                {
                    var scan = feature.Scan;
                    if (!featureMap.ContainsKey(scan))
                        featureMap.Add(scan, new List<TChildFeature>());
                    featureMap[scan].Add(feature);
                }

                // Now build the tree...where each node is a feature.
                var scans = featureMap.Keys.OrderBy(x => x);
                var tree  = new FeatureTree<TChildFeature, TParentFeature>(comparison);

                foreach (var feature in scans.SelectMany(scan => featureMap[scan]))
                {
                    tree.Insert(feature);
                }

                var newFeatures = tree.Build();
                clusters.AddRange(newFeatures);
                progressData.Report(i, n);
            }
            return clusters;          
        }

        void generator_Progress(object sender, ProgressNotifierArgs e)
        {
            OnProgress(e.Message);
        }


        #region Comparison Methods
        /// <summary>
        /// Compares a feature to the list of feature
        /// </summary>
        public int CompareMonoisotopic(FeatureLight featureX, FeatureLight featureY)
        {
            // If they are in mass range...
            var mzDiff = FeatureLight.ComputeMassPPMDifference(featureX.MassMonoisotopic, featureY.MassMonoisotopic);
            if (Math.Abs(mzDiff) < Tolerances.Mass && featureX.ChargeState != featureY.ChargeState)
            {
                // otherwise make sure that our scan value is within range
                var scanDiff = featureX.Net - featureY.Net;                
                return Math.Abs(scanDiff) <= Tolerances.Net ? 0 : 1;
            }
            if (mzDiff < 0)
                return -1;
            return 1;
        }
        /// <summary>
        /// Compares a feature to the list of feature
        /// </summary>
        public int CompareMz(FeatureLight featureX, FeatureLight featureY)
        {
            // If they are in mass range...
            var mzDiff = FeatureLight.ComputeMassPPMDifference(featureX.Mz, featureY.Mz);
            if (Math.Abs(mzDiff) < Tolerances.Mass )
            {
                // otherwise make sure that our scan value is within range                
                var scanDiff = featureX.Scan - featureY.Scan;
                if (Math.Abs(scanDiff) > ScanTolerance)
                    return 1;

                return featureX.ChargeState != featureY.ChargeState ? 1 : 0;
            }
            if (mzDiff < 0)
                return -1;
            return 1;
        } 
        #endregion


        private void OnProgress(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }

        public event EventHandler<ProgressNotifierArgs> Progress;

        public List<TParentFeature> Cluster(List<TChildFeature> data, List<TParentFeature> clusters)
        {
            throw new NotImplementedException();
        }

        public void ClusterAndProcess(List<TChildFeature> data, IClusterWriter<TParentFeature> writer, IProgress<ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public FeatureClusterParameters<TChildFeature> Parameters { get; set; }
    }
}
