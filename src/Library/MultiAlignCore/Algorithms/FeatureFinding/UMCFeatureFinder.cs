﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds features using the PNNL Omics feature finding algorithms
    /// </summary>
    public class UMCFeatureFinder: IFeatureFinder
    {
        private LCMSFeatureFindingOptions m_options;
        private int                       m_minScan;
        private int                       m_maxScan;
        /// <summary>
        /// The maximum distance two features can be from each other.
        /// </summary>
        private double                    m_maxDistance;

        /// <summary>
        /// Finds LCMS Features using the PNNL Omics linkage clustering algorithms.
        /// </summary>
        public List<UMCLight> FindFeatures( List<MSFeatureLight>        rawMsFeatures,
                                            LCMSFeatureFindingOptions   options,
                                            ISpectraProvider            provider)
        {
            const ClusterCentroidRepresentation centroidType = ClusterCentroidRepresentation.Mean;
            List<UMCLight>       features                    = null;
            m_options                                        = options;

            m_minScan = int.MaxValue;
            m_maxScan = int.MinValue;
            foreach (var feature in rawMsFeatures)
            {
                m_minScan = Math.Min(feature.Scan, m_minScan);
                m_maxScan = Math.Max(feature.Scan, m_maxScan);
            }

            var finder   = new MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight>
            {
                Parameters =
                {
                    DistanceFunction = WeightedNETDistanceFunction,
                    RangeFunction    = WithinRange,
                    Tolerances       = {Mass = options.ConstraintMonoMass, RetentionTime = 100, DriftTime = 100}
                }
            };
            finder.Parameters.CentroidRepresentation                            = centroidType;
            m_maxDistance                                                       = options.MaxDistance;
            features                                                            = finder.Cluster(rawMsFeatures);

            // Remove the short UMC's.
            features.RemoveAll(x => (x.ScanEnd - x.ScanStart + 1) < options.MinUMCLength);

            var id = 0;
            foreach (var feature in features)
            {
                feature.NET             = Convert.ToDouble(feature.Scan - m_minScan) / Convert.ToDouble(m_maxScan - m_minScan);
                feature.RetentionTime   = feature.NET;
                feature.ID              = id++;
            }

            return features;
        }
        /// <summary>
        /// Determines if two features are within proper range of each other.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool WithinRange(MSFeatureLight x, MSFeatureLight  y)
        {
            double distance = WeightedNETDistanceFunction(x, y);

            if (Math.Abs(x.Scan - y.Scan) > 6000)
            {
                if (distance < m_maxDistance)
                {
                    int xx = 0;
                    xx++;
                }
            }
            return (distance < m_maxDistance);
        }
        /// <summary>
        /// Weighted distance function that was used previously in the old feature finder.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double WeightedNETDistanceFunction(MSFeatureLight a, MSFeatureLight b)
        {
            if ((a.MassMonoisotopic - b.MassMonoisotopic) * m_options.MonoMassWeight > m_options.ConstraintMonoMass
                || (a.MassMonoisotopicAverage - b.MassMonoisotopicAverage) * m_options.AveMassWeight > m_options.ConstraintAveMass)
            {
                return double.MaxValue;
            }

            double a_log_abundance  = Math.Log10(a.Abundance);
            double b_log_abundance  = Math.Log10(b.Abundance);
            double ppm              = ((a.MassMonoisotopic - b.MassMonoisotopic) / a.MassMonoisotopic) * 1000000;
            double ppmAvg           = ((a.MassMonoisotopicAverage - b.MassMonoisotopicAverage) / a.MassMonoisotopicAverage) * 1000000;
            if (!m_options.UseNET)
            {
                double sqrDist = ppm * ppm * m_options.MonoMassWeight * m_options.MonoMassWeight;
                sqrDist += ppmAvg * ppmAvg * m_options.AveMassWeight  * m_options.AveMassWeight;
                sqrDist += (a_log_abundance - b_log_abundance) * (a_log_abundance - b_log_abundance) * m_options.LogAbundanceWeight * m_options.LogAbundanceWeight;
                sqrDist += (a.Scan  - b.Scan)  * (a.Scan  - b.Scan) * m_options.ScanWeight * m_options.ScanWeight;
                sqrDist += (a.Score - b.Score) * (a.Score - b.Score) * m_options.FitWeight * m_options.FitWeight;
                return Math.Sqrt(sqrDist);
            }
            else
            {
                double sqrDist      = ppm * ppm * m_options.MonoMassWeight * m_options.MonoMassWeight;
                double net_distance = Convert.ToDouble(a.Scan - b.Scan) / Convert.ToDouble(m_maxScan - m_minScan);
                sqrDist += (a_log_abundance - b_log_abundance) * (a_log_abundance - b_log_abundance) * m_options.LogAbundanceWeight * m_options.LogAbundanceWeight;
                // Convert scan difference to Generic NET
                sqrDist += net_distance * net_distance * m_options.NETWeight * m_options.NETWeight;
                sqrDist += (a.Score - b.Score) * (a.Score - b.Score) * m_options.FitWeight * m_options.FitWeight;
                return Math.Sqrt(sqrDist);
            }
        }
    }

    public static class EnumerableExtensions
    {
        public static int Median(this IEnumerable<int> list)
        {
            // Create a copy of the input, and sort the copy
            int[] temp = list.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }
    }
}
