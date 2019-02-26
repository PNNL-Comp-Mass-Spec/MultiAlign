using System;
using System.Collections.Generic;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Features;
using InformedProteomics.Backend.Utils;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.Algorithms.Clustering
{
    /// <summary>
    /// Performs the base linkage types.
    /// </summary>
    /// <typeparam name="T">Features to cluster.</typeparam>
    /// <typeparam name="T">Clusters produced.</typeparam>
    public abstract class LinkageClustererBase<T, U> : IProgressNotifer, IClusterer<T, U>
        where T : FeatureLight, IChildFeature<U>, new()
        where U : FeatureLight, IFeatureCluster<T>, new()
    {

        /// <summary>
        /// Compares the masses of two light features.
        /// </summary>
        protected Comparison<T> m_massComparer;

        /// <summary>
        /// Maximum distance when calculating ambiguity
        /// </summary>
        protected double m_maxDistance;

        public LinkageClustererBase()
        {
            SeedClusterID  = 0;
            m_maxDistance  = 10000;
            m_massComparer = FeatureLight.MassAlignedComparison;

            ShouldTestClustersWithinTolerance = true;


        }

        public LinkageClustererBase(int id)
        {
            SeedClusterID = id;
            m_maxDistance = 10000;
        }
        /// <summary>
        /// Gets or sets the initial cluster Id to use when assingning ID's to a cluster.
        /// </summary>
        public int SeedClusterID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the parameters used
        /// </summary>
        public FeatureClusterParameters<T> Parameters { get; set; }


        /// <summary>
        /// Clusters the UMC data and returns a list of valid UMC Clusters.
        /// </summary>
        /// <param name="data">Data to cluster.</param>
        /// <returns>List of UMC clusters.</returns>
        public List<U> Cluster(List<T> data, IProgress<PRISM.ProgressData> progress = null)
        {
            return Cluster(data, new List<U>(), progress);
        }



        /// <summary>
        ///
        /// </summary>
        /// <param name="distances"></param>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public abstract List<U> LinkFeatures(List<Data.PairwiseDistance<T>> distances, Dictionary<int, U> clusters);

        /// <summary>
        /// Clusters a set of data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public virtual List<U> Cluster(List<T> data, List<U> clusters, IProgress<PRISM.ProgressData> progress = null)
        {
            /*
             * This clustering algorithm first sorts the list of input UMC's by mass.  It then iterates
             * through this list partitioning the data into blocks of UMC's based on a mass tolerance.
             * When it finds gaps larger or equal to the mass (ppm) tolerance specified by the user,
             * it will process the data before the gap (a block) until the current index of the features in question.
             */

            var progressData = new PRISM.ProgressData(progress);

            // Make sure we have data to cluster first.
            if (data == null)
            {
                throw new NullReferenceException("The input feature data list was null.  Cannot process this data.");
            }

            // Make sure there is no null UMC data in the input list.
            var nullIndex = data.FindIndex(delegate(T x) { return x == null; });
            if (nullIndex > 0)
            {
                throw new NullReferenceException("The feature at index " + nullIndex + " was null.  Cannot process this data.");
            }

            OnNotify("Sorting cluster mass list");

            // The first thing we do is to sort the features based on mass since we know that has the least variability in the data across runs.
            data.Sort(m_massComparer);

            // Now partition the data based on mass ranges and the parameter values.
            var massTolerance = Parameters.Tolerances.Mass;

            // This is the index of first feature of a given mass partition.
            var startUMCIndex = 0;
            var totalFeatures = data.Count;


            OnNotify("Detecting mass partitions");
            var tenPercent = Convert.ToInt32(totalFeatures * .1);
            var counter    = 0;
            var percent    = 0;

            for (var i = 0; i < totalFeatures - 1; i++)
            {
                if (counter > tenPercent)
                {
                    counter = 0;
                    percent += 10;
                    OnNotify(string.Format("Clustering Completed...{0}%", percent));
                }
                counter++;

                // Here we compute the ppm mass difference between consecutive features (based on mass).
                // This will determine if we cluster a block of data or not.
                var umcX = data[i];
                var umcY = data[i + 1];
                var ppm = Math.Abs(FeatureLight.ComputeMassPPMDifference(umcX.MassMonoisotopicAligned, umcY.MassMonoisotopicAligned));

                // If the difference is greater than the tolerance then we cluster
                //  - we dont check the sign of the ppm because the data should be sorted based on mass.
                if (ppm > massTolerance)
                {
                    // If start UMC Index is equal to one, then that means the feature at startUMCIndex
                    // could not find any other features near it within the mass tolerance specified.
                    if (startUMCIndex == i)
                    {
                        var cluster = new U {AmbiguityScore = m_maxDistance};
                        umcX.SetParentFeature(cluster);
                        cluster.AddChildFeature(umcX);
                        clusters.Add(cluster);
                    }
                    else
                    {
                        // Otherwise we have more than one feature to to consider.
                        var distances     = CalculatePairWiseDistances(startUMCIndex, i, data);
                        var localClusters        = CreateSingletonClusters(data, startUMCIndex, i);
                        var blockClusters                   = LinkFeatures(distances, localClusters);
                        CalculateAmbiguityScore(blockClusters);
                        clusters.AddRange(blockClusters);
                    }

                    startUMCIndex = i + 1;
                }

                progressData.Report(i, totalFeatures);
            }

            // Make sure that we cluster what is left over.
            if (startUMCIndex < totalFeatures)
            {

                OnNotify(string.Format("Clustering last partition...{0}%", percent));
                var distances = CalculatePairWiseDistances(startUMCIndex, totalFeatures - 1, data);
                var localClusters    = CreateSingletonClusters(data, startUMCIndex, totalFeatures - 1);
                var blockClusters               = LinkFeatures(distances, localClusters);
                CalculateAmbiguityScore(blockClusters);
                if (localClusters.Count < 2)
                {
                    clusters.AddRange(localClusters.Values);
                }
                else
                {
                    clusters.AddRange(blockClusters);
                }
            }


            OnNotify("Generating cluster statistics");
            foreach (var cluster in clusters)
            {
                cluster.CalculateStatistics(Parameters.CentroidRepresentation);
            }

            return clusters;
        }

        /// <summary>
        /// Creates a list of singleton clusters from the UMC data between start and stop.
        /// </summary>
        /// <param name="data">Data to create singleton's from.</param>
        /// <param name="start">Start UMC index.</param>
        /// <param name="stop">Stop UMC Index.</param>
        /// <returns>List of singleton clusters.</returns>
        //private List<T> CreateSingletonClusters(List<UMC> data, int start, int stop)
        protected Dictionary<int, U> CreateSingletonClusters(List<T> data, int start, int stop)
        {
            var clusters = new Dictionary<int, U>();
            var tempID                  = SeedClusterID;

            for (var i = start; i <= stop; i++)
            {
                var umc        = data[i];

                // Add the feature to the parent so the cluster will point to the child.
                var parentFeature         = new U();
                var id                  = tempID++;
                parentFeature.Id        = id;

                parentFeature.AddChildFeature(umc);
                umc.SetParentFeature(parentFeature);

                // Add to output list...
                clusters.Add(id, parentFeature);
            }
            foreach (var cluster in clusters.Values)
            {
                cluster.CalculateStatistics(Parameters.CentroidRepresentation);
            }

            SeedClusterID = tempID;

            return clusters;
        }
        /// <summary>
        /// Calculates the minimum distance between two clusters by pairwise feature comparisons.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual double CalculateMinimumFeatureDistance(U x, U y)
        {
            var distance = m_maxDistance;

            foreach (var featureX in x.Features)
            {
                foreach(var featureY in y.Features)
                {
                    var tempDistance = Parameters.DistanceFunction(featureX, featureY);
                    distance = Math.Min(tempDistance, distance);
                }
            }

            return distance;
        }
        /// <summary>
        /// Calculates the ambiguity score
        /// </summary>
        /// <param name="clusters"></param>
        protected virtual void CalculateAmbiguityScore(List<U> clusters)
        {
            for (var i = 0; i < clusters.Count; i++)
            {
                var minDistance          = m_maxDistance;
                var minCentroidDistance  = m_maxDistance;
                var clusterI          = clusters[i];

                for (var j = 0; j < clusters.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var clusterJ = clusters[j];

                    var featureJ = new T
                    {
                        MassMonoisotopicAligned = clusterJ.MassMonoisotopicAligned,
                        Net = clusterJ.Net,
                        DriftTime = clusterJ.DriftTime
                    };

                    var featureI = new T
                    {
                        MassMonoisotopicAligned = clusterI.MassMonoisotopicAligned,
                        Net = clusterI.Net,
                        DriftTime = clusterI.DriftTime
                    };

                    var centroidDistance             = Parameters.DistanceFunction(featureJ, featureI);
                    minCentroidDistance                 = Math.Min(centroidDistance, minCentroidDistance);

                    var distance = CalculateMinimumFeatureDistance(clusterJ, clusterI);
                    minDistance = Math.Min(minDistance, distance);
                }
                clusterI.MinimumCentroidDistance = minCentroidDistance;
                clusterI.AmbiguityScore = minDistance;
            }
        }

        /// <summary>
        /// Calculates pairwise distances between features in the list of
        /// potential features to cluster.
        /// </summary>
        /// <param name="start">Start UMC index.</param>
        /// <param name="stop">Stop UMC index.</param>
        /// <param name="data">List of data to compute distances over.</param>
        /// <returns>List of UMC distances to consider during clustering.</returns>
        protected virtual List<Data.PairwiseDistance<T>> CalculatePairWiseDistances(int start, int stop, List<T> data)
        {
            var massTolerance                = Parameters.Tolerances.Mass;
            var netTolerance                 = Parameters.Tolerances.Net;
            var driftTolerance               = Parameters.Tolerances.DriftTime;
            var onlyClusterSameChargeStates  = Parameters.OnlyClusterSameChargeStates;

            var distances = new List<Data.PairwiseDistance<T>>();
            for (var i = start; i < stop; i++)
            {
                var featureX     = data[i];
                var driftTimeX   = featureX.DriftTime;
                var netAlignedX  = featureX.Net;
                var massAlignedX = featureX.MassMonoisotopicAligned;
                var chargeStateX = featureX.ChargeState;

                for (var j = i + 1; j <= stop; j++)
                {
                    // Don't calculate distance to self.
                    var featureY      = data[j];

                    // Calculate the distances here (using a cube).  We dont care if we are going to re-compute
                    // these again later, because here we want to fall within the cube, the distance function used
                    // later is more related to determining a scalar value instead.
                    var withinRange = AreClustersWithinTolerance(featureX, featureY); //Parameters.RangeFunction(featureX, featureY);

                    // Make sure we fall within the distance range before computing...
                    if (withinRange)
                    {
                        // If IMS or equivalent only cluster similar charge states
                        if (onlyClusterSameChargeStates)
                        {
                            // Make sure it's the same charge state
                            if (chargeStateX == featureY.ChargeState)
                            {
                                // Calculate the pairwise distance
                                var pairwiseDistance    = new Data.PairwiseDistance<T>();
                                pairwiseDistance.FeatureX               = featureX;
                                pairwiseDistance.FeatureY               = featureY;
                                pairwiseDistance.Distance               = Parameters.DistanceFunction(featureX, featureY);
                                distances.Add(pairwiseDistance);
                            }
                        }
                        else
                        {
                            // Calculate the pairwise distance
                            var pairwiseDistance    = new Data.PairwiseDistance<T>();
                            pairwiseDistance.FeatureX               = featureX;
                            pairwiseDistance.FeatureY               = featureY;
                            pairwiseDistance.Distance               = Parameters.DistanceFunction(featureX, featureY);
                            distances.Add(pairwiseDistance);
                        }
                    }
                }
            }
            return distances;
        }

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        protected void OnNotify(string message)
        {
            Progress?.Invoke(this, new ProgressNotifierArgs(message));
        }

        #endregion


        /// <summary>
        /// Determines if the algorithm should allow for clusters to be formed if they are not within tolerance.
        /// </summary>
        public bool ShouldTestClustersWithinTolerance
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if two clusters are within mass, NET, and drift time tolerances
        /// </summary>
        /// <param name="clusterX">One of the two clusters to test</param>
        /// <param name="clusterY">One of the two clusters to test</param>
        /// <returns>True if clusters are within tolerance, false otherwise</returns>
        protected virtual bool AreClustersWithinTolerance(U clusterX, U clusterY)
        {
            if (!ShouldTestClustersWithinTolerance)
                return true;

            // Grab the tolerances
            var massTolerance  = Parameters.Tolerances.Mass;
            var netTolerance   = Parameters.Tolerances.Net;
            var driftTolerance = Parameters.Tolerances.DriftTime;

            // Calculate differences
            var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(clusterX.MassMonoisotopicAligned, clusterY.MassMonoisotopicAligned));
            var netDiff = Math.Abs(clusterX.Net - clusterY.Net);
            var driftDiff = Math.Abs(clusterX.DriftTime - clusterY.DriftTime);

            // Return true only if all differences are within tolerance
            if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
            {
                return true;
            }
            return false;
        }
        protected virtual bool AreClustersWithinTolerance(T clusterX, T clusterY)
        {
            if (!ShouldTestClustersWithinTolerance)
                return true;

            // Grab the tolerances
            var massTolerance  = Parameters.Tolerances.Mass;
            var netTolerance   = Parameters.Tolerances.Net;
            var driftTolerance = Parameters.Tolerances.DriftTime;

            // Calculate differences
            var massDiff = Math.Abs(FeatureLight.ComputeMassPPMDifference(clusterX.MassMonoisotopicAligned, clusterY.MassMonoisotopicAligned));
            var netDiff = Math.Abs(clusterX.NetAligned - clusterY.NetAligned);
            var driftDiff = Math.Abs(clusterX.DriftTime - clusterY.DriftTime);

            // Return true only if all differences are within tolerance
            if (massDiff <= massTolerance && netDiff <= netTolerance && driftDiff <= driftTolerance)
            {
                return true;
            }
            return false;
        }


        #region IClusterer<T,U> Members
        /// <summary>
        /// Clusters the data but does not store the results, instead immediately writes the data to the stream writer provided.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        public void ClusterAndProcess(List<T> data, IClusterWriter<U> writer, IProgress<PRISM.ProgressData> progress = null)
        {
            /*
             * This clustering algorithm first sorts the list of input UMC's by mass.  It then iterates
             * through this list partitioning the data into blocks of UMC's based on a mass tolerance.
             * When it finds gaps larger or equal to the mass (ppm) tolerance specified by the user,
             * it will process the data before the gap (a block) until the current index of the features in question.
             */

            // Make sure we have data to cluster first.
            if (data == null)
            {
                throw new NullReferenceException("The input feature data list was null.  Cannot process this data.");
            }

            // Make sure there is no null UMC data in the input list.
            var nullIndex = data.FindIndex(delegate(T x) { return x == null; });
            if (nullIndex > 0)
            {
                throw new NullReferenceException("The feature at index " + nullIndex + " was null.  Cannot process this data.");
            }

            OnNotify("Sorting cluster mass list");

            // The first thing we do is to sort the features based on mass since we know that has the least variability in the data across runs.
            data.Sort(m_massComparer);

            // Now partition the data based on mass ranges and the parameter values.
            var massTolerance = Parameters.Tolerances.Mass;

            // This is the index of first feature of a given mass partition.
            var startUMCIndex = 0;
            var totalFeatures = data.Count;


            OnNotify("Detecting mass partitions");
            var tenPercent = Convert.ToInt32(totalFeatures * .1);
            var counter    = 0;
            var percent    = 0;

            var clusterId = 0;

            for (var i = 0; i < totalFeatures - 1; i++)
            {
                if (counter > tenPercent)
                {
                    counter = 0;
                    percent += 10;
                    OnNotify(string.Format("Clustering Completed...{0}%", percent));
                }
                counter++;

                // Here we compute the ppm mass difference between consecutive features (based on mass).
                // This will determine if we cluster a block of data or not.
                var umcX = data[i];
                var umcY = data[i + 1];
                var ppm = Math.Abs(FeatureLight.ComputeMassPPMDifference(umcX.MassMonoisotopicAligned, umcY.MassMonoisotopicAligned));

                // If the difference is greater than the tolerance then we cluster
                //  - we dont check the sign of the ppm because the data should be sorted based on mass.
                if (ppm > massTolerance)
                {
                    // If start UMC Index is equal to one, then that means the feature at startUMCIndex
                    // could not find any other features near it within the mass tolerance specified.
                    if (startUMCIndex == i)
                    {
                        var cluster = new U {AmbiguityScore = m_maxDistance};

                        umcX.SetParentFeature(cluster);
                        cluster.AddChildFeature(umcX);

                        cluster.CalculateStatistics(Parameters.CentroidRepresentation);
                        cluster.Id = clusterId++;
                        writer.WriteCluster(cluster);
                    }
                    else
                    {
                        // Otherwise we have more than one feature to to consider.
                        var distances = CalculatePairWiseDistances(startUMCIndex, i, data);
                        var localClusters    = CreateSingletonClusters(data, startUMCIndex, i);
                        var blockClusters               = LinkFeatures(distances, localClusters);

                        CalculateAmbiguityScore(blockClusters);

                        foreach (var cluster in localClusters.Values)
                        {
                            cluster.Id = clusterId++;
                            CalculateStatistics(cluster);
                            writer.WriteCluster(cluster);
                        }
                    }

                    startUMCIndex = i + 1;
                }
            }

            // Make sure that we cluster what is left over.
            if (startUMCIndex < totalFeatures)
            {
                OnNotify(string.Format("Clustering last partition...{0}%", percent));
                var distances = CalculatePairWiseDistances(startUMCIndex, totalFeatures - 1, data);
                var localClusters = CreateSingletonClusters(data, startUMCIndex, totalFeatures - 1);
                var blockClusters = LinkFeatures(distances, localClusters);

                CalculateAmbiguityScore(blockClusters);

                if (localClusters.Count < 2)
                {
                    foreach (var cluster in localClusters.Values)
                    {
                        cluster.Id = clusterId++;
                        CalculateStatistics(cluster);
                        writer.WriteCluster(cluster);
                    }
                }
                else
                {
                    foreach (var cluster in blockClusters)
                    {
                        cluster.Id = clusterId++;
                        CalculateStatistics(cluster);
                        writer.WriteCluster(cluster);
                    }
                }
            }
           // OnNotify("Generating cluster statistics");
        }
        #endregion

        protected virtual void CalculateStatistics(U cluster)
        {
            cluster.CalculateStatistics(Parameters.CentroidRepresentation);
        }
    }
}
