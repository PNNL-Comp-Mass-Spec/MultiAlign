using System;
using MultiAlignCore.IO.Features;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.Algorithms.Features
{
    public class ClusterErrorHistograms
    {
        public void CalculateClusterErrorHistograms(List<UMCClusterLight> clusters, 
                                                    List<double> massErrorPpm, 
                                                    List<double> netError,
                                                    List<double> counts, FeatureTolerances tolerances,
                                                    Dictionary<int, List<double>> ranges) 
        {
            List<UMCClusterLight> sortedClusters = new List<UMCClusterLight>();
            sortedClusters.AddRange(clusters);
            sortedClusters.Sort(delegate(UMCClusterLight x, UMCClusterLight y)
            {
                return x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
            });
            
            foreach(UMCClusterLight x in sortedClusters)
            {
                int count = 0;                
                List<double> netErrors = new List<double>();
                foreach(UMCClusterLight y in sortedClusters)
                {
                    if (x.ID == y.ID)
                        continue;

                    double ppmDiff      = Feature.ComputeMassPPMDifference(y.MassMonoisotopicAligned, x.MassMonoisotopicAligned);
                    if (System.Math.Abs(ppmDiff) > tolerances.Mass)
                    {
                        continue;
                    }

                    double netDiff      = x.RetentionTime - y.RetentionTime;                        
                    netErrors.Add(netDiff);
                    count = count + 1;
                    
                    massErrorPpm.Add(ppmDiff);
                    netError.Add(netDiff);
                }

                counts.Add(Convert.ToDouble(count));

                if (ranges.ContainsKey(count) == false)
                {
                    ranges.Add(count, new List<double>());
                }
                ranges[count].AddRange(netErrors);
            }            
        }
        public void CalculateClusterErrorHistograms(FeatureDataAccessProviders providers, List<double> massErrorPpm, List<double> netError, List<double> counts, FeatureTolerances tolerances)
        {
            List<UMCLight> featuresA = providers.FeatureCache.FindByDatasetId(0);
            List<UMCLight> featuresB = providers.FeatureCache.FindByDatasetId(1);

            featuresA.Sort(

                delegate (UMCLight x, UMCLight y)
                    {
                        return x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
                    }                
                );

            featuresB.Sort(

                delegate (UMCLight x, UMCLight y)
                    {
                        return x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
                    }                
                );

            int i = 0;
            foreach(UMCLight featureA in featuresA)
            {
                double count = 0;
                int j        = i + 1;
                foreach (UMCLight featureB in featuresB)
                {                   
                    double ppmDiff = Feature.ComputeMassPPMDifference(featureB.MassMonoisotopicAligned, featureA.MassMonoisotopicAligned);
                    if (Math.Abs(ppmDiff) > tolerances.Mass)
                    {
                        continue;
                    }

                    double netDiff = featureA.RetentionTime - featureB.RetentionTime;
                    if (Math.Abs(netDiff) > tolerances.RetentionTime)
                    {
                        continue;
                    }

                    massErrorPpm.Add(ppmDiff);
                    netError.Add(netDiff);                    
                }
                i = j;
                counts.Add(count);
            }
        }

        public void CalculateClusterErrorHistogramsSingle(FeatureDataAccessProviders providers, int id, List<double> massErrorPpm, List<double> netError, List<double> counts, FeatureTolerances tolerances)
        {
            List<UMCLight> featuresA = providers.FeatureCache.FindByDatasetId(id);

            featuresA.Sort(

                delegate(UMCLight x, UMCLight y)
                {
                    return x.MassMonoisotopic.CompareTo(y.MassMonoisotopic);
                }
                );

           
            int i = 0;
            foreach (UMCLight featureA in featuresA)
            {
                double count = 0;
                int j = i + 1;
                foreach (UMCLight featureB in featuresA)
                {
                    if (featureA.ID == featureB.ID)
                        continue;

                    double ppmDiff = Feature.ComputeMassPPMDifference(featureB.MassMonoisotopicAligned, featureA.MassMonoisotopicAligned);
                    if (Math.Abs(ppmDiff) > tolerances.Mass)
                    {
                        continue;
                    }

                    double netDiff = featureA.RetentionTime - featureB.RetentionTime;
                    if (Math.Abs(netDiff) > tolerances.RetentionTime)
                    {
                        continue;
                    }

                    massErrorPpm.Add(ppmDiff);
                    netError.Add(netDiff);
                }
                i = j;
                counts.Add(count);
            }
        }

        public void CalculateClusterErrorHistograms(List<UMCClusterLight> list, List<double> mass20ppm, List<double> net20ppm, List<double> featureCounts20ppm, FeatureTolerances tolerances)
        {
            throw new NotImplementedException();
        }
    }


    public class ErrorHistogramWriter
    {
        public static void WriteData(string path, string header ,List<double> mass, List<double> net)
        {
            using (TextWriter writer = File.AppendText(path))
            {
                writer.WriteLine("");
                writer.WriteLine(header);
                for (int i = 0; i < mass.Count; i++)
                {
                    writer.WriteLine("{0}\t{1}", net[i], mass[i]);
                }
            }
        }

        public static void WriteCDFData(string path, string header, List<double> net)
        {
            List<double> nets = new List<double>();
            nets.AddRange(net);

            nets.Sort();
            using (TextWriter writer = File.AppendText(path))
            {
                writer.WriteLine(header);
                for (int i = 0; i < nets.Count; i++)
                {
                    writer.WriteLine("{0}\t{1}", nets[i], i);
                }
            }
        }

        public static void WriteRanges( string path,  
                                        Dictionary<int, List<double>> ranges)
        {
            List<int> keys = new List<int>();

            foreach (int neighborSize in ranges.Keys)
            {
                keys.Add(neighborSize);   
            }
            keys.Sort();

            foreach(int neighborSize in keys)
            {
                string header = string.Format("[net errors - {0}]", neighborSize);
                WriteCDFData(path, header, ranges[neighborSize]);
            }            
        }
    }
}
