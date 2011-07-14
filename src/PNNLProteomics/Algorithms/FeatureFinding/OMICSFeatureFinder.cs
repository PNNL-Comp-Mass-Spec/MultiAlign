using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.IO.FileReaders;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Algorithms.FeatureFinding
{
    /// <summary>
    /// Finds features using the PNNL Omics feature finding algorithms
    /// </summary>
    public class OMICSFeatureFinder: IFeatureFinder
    {
        private clsUMCFindingOptions    m_options;
        private int                     m_minScan;
        private int                     m_maxScan;

        public List<UMCLight> FindFeatures(string path, clsUMCFindingOptions options)
        {
            List<UMCLight>       features   = null;
            List<MSFeatureLight> msFeatures = null;
            MSFeatureLightFileReader reader = new MSFeatureLightFileReader();
            reader.Delimeter                = ",";
            msFeatures                      = reader.ReadFile(path).ToList();
            m_options                       = options;

            m_minScan = int.MaxValue;
            m_maxScan = int.MinValue;
            foreach (MSFeatureLight feature in msFeatures)
            {
                m_minScan = Math.Min(feature.Scan, m_minScan);
                m_maxScan = Math.Max(feature.Scan, m_maxScan);
            }

            UMCSingleLinkageClusterer<MSFeatureLight, UMCLight> finder  = new UMCSingleLinkageClusterer<MSFeatureLight, UMCLight>();            
            finder.Parameters.DistanceFunction      = new DistanceFunction<MSFeatureLight>(WeightedNETDistanceFunction);            
            features                                =  finder.Cluster(msFeatures);
            return features;
        }
        /// <summary>
        /// Weighted distance function.
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
			double ppm				= (a.MassMonoisotopic - b.MassMonoisotopic) / a.MassMonoisotopic * 1000000;
            double ppmAvg           = (a.MassMonoisotopicAverage - b.MassMonoisotopicAverage) / a.MassMonoisotopicAverage * 1000000; 
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
				double net_distance = (a.Scan - b.Scan) * 1.0 / (m_maxScan - m_minScan);				
                sqrDist += (a_log_abundance - b_log_abundance) * (a_log_abundance - b_log_abundance) * m_options.LogAbundanceWeight * m_options.LogAbundanceWeight; 
				// Convert scan difference to Generic NET
                sqrDist += net_distance * net_distance * m_options.NETWeight * m_options.NETWeight;
                sqrDist += (a.Score - b.Score) * (a.Score - b.Score) * m_options.FitWeight * m_options.FitWeight; 
				return Math.Sqrt(sqrDist); 
			}            
        }
    }
}
