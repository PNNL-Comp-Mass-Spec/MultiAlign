using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.IO.FileReaders;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds features using the PNNL Omics feature finding algorithms
    /// </summary>
    public class UMCFeatureFinder: IFeatureFinder
    {
        private UMCFeatureFinderOptions m_options;
        private int                     m_minScan;
        private int                     m_maxScan;
        /// <summary>
        /// The maximum distance two features can be from each other.
        /// </summary>
        private double                  m_maxDistance;

        /// <summary>
        /// Finds LCMS Features using the PNNL Omics linkage clustering algorithms.  
        /// </summary>
        /// <param name="msFeatures"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<UMCLight> FindFeatures( List<MSFeatureLight>    rawMsFeatures, 
                                            UMCFeatureFinderOptions options)
        {
            ClusterCentroidRepresentation centroidType  = ClusterCentroidRepresentation.Mean;                           
            List<UMCLight>       features               = null;           
            m_options                                   = options;

            m_minScan = int.MaxValue;
            m_maxScan = int.MinValue;
            foreach (MSFeatureLight feature in rawMsFeatures)
            {
                m_minScan = Math.Min(feature.Scan, m_minScan);
                m_maxScan = Math.Max(feature.Scan, m_maxScan);                
            }

            MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight> finder   = new MSFeatureSingleLinkageClustering<MSFeatureLight, UMCLight>();            
            finder.Parameters.DistanceFunction                                  = new DistanceFunction<MSFeatureLight>(WeightedNETDistanceFunction);
            finder.Parameters.RangeFunction                                     = new WithinTolerances<MSFeatureLight>(WithinRange);
            finder.Parameters.Tolerances.Mass                                   = options.ConstraintMonoMass;
            finder.Parameters.Tolerances.RetentionTime                          = 100;
            finder.Parameters.Tolerances.DriftTime                              = 100;            
            finder.Parameters.CentroidRepresentation                            = centroidType;
            m_maxDistance                                                       = options.MaxDistance;
            features                                                            = finder.Cluster(rawMsFeatures);            
            
            // Remove the short UMC's.
            features.RemoveAll(x => (x.ScanEnd - x.ScanStart + 1) < options.MinUMCLength);

            bool split = options.Split;

            if (split)
            {
                Dictionary<UMCLight, List<UMCLight>> replacementFeatures = new Dictionary<UMCLight, List<UMCLight>>();

                foreach (UMCLight feature in features)
                {
                    // Sort first by scan, so we can look at consecutive MS feature distances.
                    feature.MSFeatures.Sort(
                            delegate(MSFeatureLight x, MSFeatureLight y)
                            {
                                return (x.Scan.CompareTo(y.Scan));
                            }
                        );



                    // Then calculate their respective scan deltas.
                    List<int> deltas = new List<int>();
                    for (int i = 1; i < feature.MSFeatures.Count; i++)
                    {
                        int delta = feature.MSFeatures[i].Scan - feature.MSFeatures[i - 1].Scan;
                        deltas.Add(delta);
                    }

                    int median          = deltas.Median(); 
                    List<int> divisions = new List<int>();

                    // Then we break up any features that are ill-behaived.
                    // We use the rule of one stdev from the sample population, s.t. that std !~ to the mean.                
                    
                        for (int i = 0; i < deltas.Count; i++)
                        {
                            if (deltas[i] > median)
                            {
                                divisions.Add(i + 1);
                            }
                        }

                        // Divide the features up and move them.
                        int j = 0;
                        int N = feature.MSFeatures.Count;
                        // This is the first set of partitions
                        foreach (int i in divisions)
                        {
                            // copy the jth to the ith feature
                            MSFeatureLight[] tempFeatures = new MSFeatureLight[i - j];
                            feature.MSFeatures.CopyTo(j, tempFeatures, 0, i - j);
                            j = i;

                            // Then set the parent feature and child features.
                            UMCLight newFeature = new UMCLight();
                            for (int k = 0; k < tempFeatures.Length; k++)
                            {
                                tempFeatures[k].SetParentFeature(newFeature);
                                newFeature.AddChildFeature(tempFeatures[k]);
                            }
                            newFeature.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                            if (!replacementFeatures.ContainsKey(feature))
                            {
                                replacementFeatures.Add(feature, new List<UMCLight>());
                            }
                            replacementFeatures[feature].Add(newFeature);
                        }
                        // Here we cleanup the last half of the feature.
                        if (N != j)
                        {
                            MSFeatureLight[] tempFeatures = new MSFeatureLight[N - j];
                            feature.MSFeatures.CopyTo(j, tempFeatures, 0, N - j);

                            UMCLight newFeature = new UMCLight();
                            for (int k = 0; k < tempFeatures.Length; k++)
                            {
                                tempFeatures[k].SetParentFeature(newFeature);
                                newFeature.AddChildFeature(tempFeatures[k]);
                            }
                            newFeature.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                            if (!replacementFeatures.ContainsKey(feature))
                            {
                                replacementFeatures.Add(feature, new List<UMCLight>());
                            }
                            replacementFeatures[feature].Add(newFeature);
                        }                    
                }

                // add the new features
                foreach (UMCLight feature in replacementFeatures.Keys)
                {
                    features.Remove(feature);
                    feature.MSFeatures.Clear();
                    features.AddRange(replacementFeatures[feature]);
                    
                }
                // Remove the short UMC's.
                features.RemoveAll(x => (x.ScanEnd - x.ScanStart + 1) < options.MinUMCLength);
                features.RemoveAll(x => (x.MSFeatures.Count) < options.MinUMCLength);
            }

            int id = 0;
            foreach (UMCLight feature in features)
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

			double a_log_abundance	= Math.Log10(a.Abundance);
            double b_log_abundance  = Math.Log10(b.Abundance); 
			double ppm				= ((a.MassMonoisotopic - b.MassMonoisotopic) / a.MassMonoisotopic) * 1000000;
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
