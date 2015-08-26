using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MultiAlignCore.Algorithms.Chromatograms;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;
using PNNLOmics.Annotations;
using PNNLOmics.Data.Constants.Libraries;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Finds clusters using a tree aproach
    /// </summary>
    /// <typeparam name="TChildFeature"></typeparam>
    /// <typeparam name="TParentFeature"></typeparam>
    public class MsFeatureTreeClusterer<TChildFeature, TParentFeature>
        : IProgressNotifer
        where TChildFeature: MSFeatureLight, new()
        where TParentFeature: UMCLight, IFeatureCluster<TChildFeature>, IFeatureCluster<TParentFeature>, new ()
        
        
    {
        private const int CONST_SCAN_TOLERANCE = 30; // was 50

        /// <summary>
        /// Constructor
        /// </summary>
        public MsFeatureTreeClusterer()
        {
            Tolerances          = new FeatureTolerances();
            ScanTolerance       = CONST_SCAN_TOLERANCE;
            FilteringOptions    = new LcmsFeatureFilteringOptions();
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
        
        /// <summary>
        /// Clusters features based on some specified values. 
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TCluster"></typeparam>
        /// <param name="features"></param>
        /// <param name="sortFunction"></param>
        /// <param name="massDiffFunction"></param>
        /// <param name="comparison"></param>
        /// <param name="massTolerance"></param>
        /// <returns></returns>
        private IEnumerable<TCluster> Cluster<TChild, TCluster>( IEnumerable<TChild>             features, 
                                                                 Comparison<TChild>              sortFunction,
                                                                 Func<TChild, TChild, double>    massDiffFunction,
                                                                 Func<TChild, TChild, int>       comparison,
                                                                 double                          massTolerance) 
                where TChild:   FeatureLight, new()
                where TCluster: FeatureLight, IFeatureCluster<TChild>, new ()
        {
            var clusters            = new List<TCluster>();            
            var currentIndex        = 0;
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
                var gapFeatures  = new List<TChild> { lastFeature };
                while (!hasGap && lastIndex < n)
                {

                    var currentFeature      = msFeatures[lastIndex];


                    var massDiff         = massDiffFunction(currentFeature, lastFeature);

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
                var featureMap = new Dictionary<int, List<TChild>>();
                foreach (var feature in gapFeatures)
                {
                    var scan = feature.Scan;
                    if (!featureMap.ContainsKey(scan))
                        featureMap.Add(scan, new List<TChild>());
                    featureMap[scan].Add(feature);
                }

                // Now build the tree...where each node is a feature.
                var scans = featureMap.Keys.OrderBy(x => x);
                var tree  = new FeatureTree<TChild, TCluster>(comparison);

                foreach (var feature in scans.SelectMany(scan => featureMap[scan]))
                {
                    tree.Insert(feature);
                }

                var newFeatures = tree.Build();
                clusters.AddRange(newFeatures);
            }
            return clusters;          
        }

        /// <summary>
        /// Finds LCMS Features from MS Features.
        /// </summary>
        /// <param name="rawMsFeatures"></param>
        /// <returns></returns>
        public List<TParentFeature> Cluster(List<TChildFeature> rawMsFeatures)
        {            
            Comparison<TChildFeature> mzSort                         = (x, y) => x.Mz.CompareTo(y.Mz);                               
            Comparison<TParentFeature> monoSort                      = (x, y) => x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
            Func<TChildFeature, TChildFeature, double> mzDiff        = (x, y) => FeatureLight.ComputeMassPPMDifference(x.Mz, y.Mz);
            Func<TParentFeature, TParentFeature, double> monoDiff    = (x, y) => FeatureLight.ComputeMassPPMDifference(x.MassMonoisotopic, y.MassMonoisotopic);

            var minScan = Convert.ToDouble(rawMsFeatures.Min(x => x.Scan));
            var maxScan = Convert.ToDouble(rawMsFeatures.Max(x => x.Scan));
            foreach (var msFeature in rawMsFeatures)
            {
                msFeature.Net = (Convert.ToDouble(msFeature.Scan) - minScan)/(maxScan - minScan);
            }

            IEnumerable<TParentFeature> features;
            using (var logger = new StreamWriter("msfeatureClusteringStats.txt", true))
            {
                logger.WriteLine();

                OnProgress("Filtering ambiguous features");
                //rawMsFeatures = FilterMsFeatures(rawMsFeatures);

                var stopWatch = new Stopwatch();

                stopWatch.Start();
                OnProgress("Clustering child features into potential UMC candidates");
                // First cluster based on m/z finding the XIC's
                //var clusterer = new MSFeatureSingleLinkageClustering<TChildFeature, TParentFeature>();
                //features = clusterer.Cluster(rawMsFeatures);

                features = Cluster<TChildFeature, TParentFeature>(rawMsFeatures,
                                                            mzSort,
                                                            mzDiff,
                                                            CompareMz,
                                                            Tolerances.Mass).ToList();

                stopWatch.Stop();
                logger.WriteLine("Initial isos clustering: {0}s", stopWatch.Elapsed.TotalSeconds);

                var n = features.Count();
                OnProgress(string.Format("Found {0} unique  child features from {1} total features",
                                                n,
                                                rawMsFeatures.Count()));

                OnProgress("Filtering Features");

                // Then we group into UMC's for clustering across charge states...
                if (features == null)
                    throw new InvalidDataException("No features were found from the input MS Feature list.");

                OnProgress("Filtering poor features with no data.  Calculating statistics for the good ones.");
                features = features.Where(x => x.MsFeatures.Count > 0).ToList();
                foreach (var feature in features)
                {
                    feature.CalculateStatistics(ClusterCentroidRepresentation.Median);
                    feature.MassMonoisotopic = (feature.Mz * feature.ChargeState) - (SubAtomicParticleLibrary.MASS_PROTON * feature.ChargeState);
                }

                stopWatch.Restart();

                // Here we should merge the XIC data...trying to find the best possible feature
                // Note that at this point we dont have UMC's.  We only have features
                // that are separated by mass , scan , and charge 
                // so this method should interrogate each one of these....
                if (SpectraProvider != null)
                {
                    OnProgress(string.Format("Building XIC's from child features"));
                    var generator = new XicCreator();
                    generator.Progress += generator_Progress;
                    features = generator.CreateXicNew(features as List<UMCLight>, Tolerances.Mass, SpectraProvider) as List<TParentFeature>;
                    generator.Progress -= generator_Progress;
                }

                stopWatch.Stop();
                logger.WriteLine("XIC creation: {0}s", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Restart();

                OnProgress(string.Format("Calculating statistics for each feature"));
                foreach (var feature in features)
                {
                    feature.CalculateStatistics(ClusterCentroidRepresentation.Median);
                    feature.Net = Convert.ToDouble(feature.Scan - minScan) / Convert.ToDouble(maxScan - minScan);
                }

                OnProgress(string.Format("Combining child feature charge states"));

                features = Cluster<TParentFeature, TParentFeature>(features,
                                        monoSort,
                                        monoDiff,
                                        CompareMonoisotopic,
                                        Tolerances.Mass).ToList();

                stopWatch.Stop();
                logger.WriteLine("Final clustering time: {0}", stopWatch.Elapsed.TotalSeconds);
            }

            var id = 0;
            OnProgress(string.Format("Assigning unique feature id's to each feature."));
            var featureList = features.ToList();
            foreach (var feature in featureList)
            {
                feature.Id = id++;
            }
            return featureList;
        }

        private List<TChildFeature> FilterMsFeatures(List<TChildFeature> rawMsFeatures)
        {
            // sort by scan...
            var allFeatures     = rawMsFeatures.OrderBy(x => x.Scan).ToList();


            var newFeatures     = new List<TChildFeature>();
            var features        = new List<TChildFeature>();
            var totalFeatures   = rawMsFeatures.Count;
            var currentScan     = 0;
            
            for (var i = 0; i < totalFeatures; i++)
            {
                var feature = allFeatures[i];
                // Process the scans...
                if (currentScan != feature.Scan)
                {
                    var mzFeatures = features.OrderBy(x => x.Mz).ToList();
                    var mzMap = new Dictionary<double, List<TChildFeature>>();
                    for (var j = 1; j < mzFeatures.Count; j++)
                    {
                        var featureJ    = mzFeatures[j];
                        var featurePrev = mzFeatures[j - 1];

                        // find the mass difference, here we are looking to see if there are unique  
                        // m/z features or not, if not, then we need to process them.
                        var ppm = FeatureLight.ComputeMassPPMDifference(featureJ.Mz, featureJ.Mz);
                        if (Math.Abs(ppm) > 1)
                        {
                            if (!mzMap.ContainsKey(featureJ.Mz))
                            {
                                mzMap.Add(featureJ.Mz, new List<TChildFeature>());
                            }
                            mzMap[featureJ.Mz].Add(featureJ);
                            mzMap[featureJ.Mz].Add(featurePrev);                            
                        }    
                    }
                    features.Clear();                    
                }
                else
                {
                    features.Add(feature);
                }
            }

            return newFeatures;
        }

        void generator_Progress(object sender, ProgressNotifierArgs e)
        {
            OnProgress(e.Message);
        }


        #region Comparison Methods
        /// <summary>
        /// Compares a feature to the list of feature
        /// </summary>
        public int CompareMonoisotopic(TParentFeature featureX, TParentFeature featureY)
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
        private int CompareMz(TChildFeature featureX, TChildFeature featureY)
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
    }
}
