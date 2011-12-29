using System;
using MultiAlignCore.Data.SequenceData;
using System.Collections.Generic;
using System.Linq;
using MultiAlignCore.Data;
using System.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using MultiAlignEngine.Features;

namespace MultiAlignCore.Algorithms.Alignment
{
    public class AnchorPointAlignment: IProgressNotifer
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        
        #region Delegate Handlers / Marshallers
        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message, 0));
            }
        }
        #endregion

        public void ChebyShev(double[] c, double[] x, ref double func, object obj)
        {
            double sum = 0;
            double t0 = c[0];
            double t1 = c[1] * x[0];
            double prev = t0;
            for (int i = 0; i < c.Length - 1; i++)
            {
                double value = 2 * x[0] * c[i] - prev;
                prev = value;
                sum += value;
            }
            func = sum;
        }

        void msCluster_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }

        public void Align(MultiAlignAnalysis analysis)
        {
            if (analysis.MetaData.OtherFiles.Count <= 0)
            {
                throw new Exception("No RAW files were specified.  Cannot continue with the analysis.");
            }



            Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>> sequenceMap = new Dictionary<int, Dictionary<int, Dictionary<int, DatabaseSearchSequence>>>();
            
            // Create the object that knows how to read the RAW files we are analyzing.
            ThermoRawDataFileReader reader = new ThermoRawDataFileReader();
            for (int i = 0; i < analysis.MetaData.Datasets.Count; i++)
            {
                DatasetInformation datasetInformation   = analysis.MetaData.Datasets[i];

                for (int j = 0; j < analysis.MetaData.OtherFiles.Count; j++)
                {
                    InputFile rawInformation = analysis.MetaData.OtherFiles[j];

                    string rawName = DatasetInformation.CleanNameDatasetNameOfExtensions(rawInformation.Path);
                    if (datasetInformation.DatasetName != rawName)
                    {
                        continue;
                    }

                    switch (rawInformation.FileType)
                    {
                        case InputFileType.Raw:
                            reader.AddDataFile(rawInformation.Path, datasetInformation.DatasetId);
                            break;
                        
                        case InputFileType.Sequence:                        
                            List<DatabaseSearchSequence> sequences = analysis.DataProviders.DatabaseSequenceCache.FindByDatasetId(datasetInformation.DatasetId);
                            Dictionary<int, Dictionary<int, DatabaseSearchSequence>> map = new Dictionary<int, Dictionary<int, DatabaseSearchSequence>>();
                            foreach (DatabaseSearchSequence sequence in sequences)
                            {

                                if (sequence.TrypticEnds > -1)
                                {
                                    try
                                    {
                                        Dictionary<int, DatabaseSearchSequence> chargeMap = new Dictionary<int, DatabaseSearchSequence>();
                                        if (!map.ContainsKey(sequence.Scan))
                                        {
                                            map.Add(sequence.Scan, chargeMap);
                                        }
                                        map[sequence.Scan].Add(sequence.Charge, sequence);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw;
                                    }
                                }
                            }                            
                            sequenceMap.Add(datasetInformation.DatasetId, map);
                            break;
                    }
                }
            }


            MSMSFeatureExtractor extractor = new MSMSFeatureExtractor();
            extractor.Progress += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
            Dictionary<int, List<UMCLight>> features = extractor.ExtractUMCWithMSMS(analysis.DataProviders,
                                                                                    analysis.MetaData.Datasets);

            UpdateStatus("Starting MS/MS clustering.");

            double[] sim = new double[] { .95, .8, .7, .6, .5 };

            string path = analysis.MetaData.AnalysisPath; // System.IO.Path.GetDirectoryName(m_analysis.MetaData.AnalysisPath);
            foreach (double similarity in sim)
            {
                // Create the object o align.
                SpectralNormalizedDotProductComparer comparer = new SpectralNormalizedDotProductComparer();
                comparer.TopPercent = .8;
                MSMSClusterer msCluster = new MSMSClusterer();
                msCluster.SimilarityTolerance = similarity;
                msCluster.ScanRange = 2000;
                msCluster.SpectralComparer = comparer;
                msCluster.Progress += new EventHandler<ProgressNotifierArgs>(msCluster_Progress);
                List<MSMSCluster> clusters = msCluster.Cluster(features, reader);
                List<MSMSClusterMap> maps = new List<MSMSClusterMap>();
                int id = 0;

                List<double> xValues = new List<double>();
                List<double> yValues = new List<double>();

                List<int> groupIDS = new List<int>() {0, 0};
                using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("clusters-{0}.csv", similarity))))
                {
                    foreach (MSMSCluster cluster in clusters)
                    {
                        cluster.ID = id++;
                        string line = string.Format("{0},", cluster.ID);

                        // This is temporary.  So that we only export clusters with two features.
                        if (cluster.Features.Count > 2) continue;

                        // Organize the spectra so they are sorted by dataset.
                        cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                        {
                            // Sort by scan if they are the same feature.
                            if (x.GroupID == y.GroupID)
                            {
                                return x.Scan.CompareTo(y.Scan);
                            }
                            // Otherwise we want to sort by what dataset they came from.
                            return x.GroupID.CompareTo(y.GroupID);
                        });

                        groupIDS[0] = cluster.Features[0].GroupID;
                        groupIDS[1] = cluster.Features[1].GroupID;

                        xValues.Add(Convert.ToDouble(cluster.Features[0].Scan));
                        yValues.Add(Convert.ToDouble(cluster.Features[1].Scan));

                        foreach (MSFeatureLight feature in cluster.Features)
                        {
                            MSMSClusterMap newMap   = new MSMSClusterMap();
                            newMap.ClusterID        = cluster.ID;
                            newMap.MSMSID           = feature.MSnSpectra[0].ID;
                            newMap.GroupID          = feature.GroupID;
                            maps.Add(newMap);
                            line += string.Format("{2},{0},{1},", feature.Scan, feature.Mz, feature.GroupID);
                        }
                        xwriter.WriteLine(line);
                    }
                }

                List<KeyValuePair<double, double>> values = new List<KeyValuePair<double, double>>();
                for (int i = 0; i < xValues.Count(); i++)
                {
                    KeyValuePair<double, double> value = new KeyValuePair<double, double>(xValues.ElementAt(i), yValues.ElementAt(i));
                    values.Add(value);
                }
                values = values.OrderBy(i => i.Key).ToList();
                xValues = values.Select(i => i.Key).ToList();
                yValues = values.Select(i => i.Value).ToList();


                List<double> tX = xValues.ToList();
                List<double> tY = yValues.ToList();

                PNNLOmics.Algorithms.Solvers.LevenburgMarquadt warp = new PNNLOmics.Algorithms.Solvers.LevenburgMarquadt();
                warp.BasisFunction = ChebyShev;

                double[] coeffs = new double[20];
                coeffs[1] = 1;
                bool passed = warp.Solve(tX, tY, ref coeffs);
                
                if (passed)
                {
                    using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("matches-{0}.csv", similarity))))
                    {
                        double[] x = new double[1];
                        double value = 0;
                        for (int i = 0; i < tX.Count; i++)
                        {
                            x[0] = tX[i];
                            ChebyShev(coeffs, x, ref value, null);
                                                     

                            xwriter.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", tX[i],
                                                                                            tY[i],
                                                                                            value,
                                                                                            tY[i],
                                                                                            tX[i] - tY[i],
                                                                                            tY[i] - value,
                                                                                            (tX[i] - tY[i]) - (tY[i] - value)
                                                                                            );

                            
                        }
                    }                    
                }
                if (passed)
                {
                    using (TextWriter xwriter = File.CreateText(System.IO.Path.Combine(path, string.Format("match-sequences-{0}.csv", similarity))))
                    {
                        xwriter.WriteLine("cluster id, cluster mean score, dataset id, scan, mz, charge, mass most abundant, find scan, pep seq, score, mass, charge, tryptic ends");
                            
                        foreach (MSMSCluster cluster in clusters)
                        {
                            cluster.ID = id++;
                            string line = string.Format("{0},{1},", cluster.ID, cluster.MeanScore);

                            // This is temporary.  So that we only export clusters with two features.
                            if (cluster.Features.Count > 2) continue;

                            // Organize the spectra so they are sorted by dataset.
                            cluster.Features.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                            {
                                // Sort by scan if they are the same feature.
                                if (x.GroupID == y.GroupID)
                                {
                                    return x.Scan.CompareTo(y.Scan);
                                }
                                // Otherwise we want to sort by what dataset they came from.
                                return x.GroupID.CompareTo(y.GroupID);
                            });
                            
                            foreach (MSFeatureLight feature in cluster.Features)
                            {
                                int findScan    = feature.MSnSpectra[0].Scan;
                                
                                    DatabaseSearchSequence sequence = null;
                                    if (!sequenceMap[feature.GroupID].ContainsKey(findScan))
                                    {
                                        line += string.Format("{0},{1},,,,,,,,,", feature.GroupID, feature.Scan);
                                        continue;
                                    }

                                    try
                                    {
                                        sequence = sequenceMap[feature.GroupID][findScan][feature.ChargeState];
                                    }
                                    catch
                                    {
                                    }
                                    string pepsequence  = "";
                                    double score        = 0;
                                    double mass         = 0;
                                    int charge          = 0;
                                    int ends            = 0;

                                    if (sequence != null)
                                    {
                                        pepsequence  = sequence.Sequence;
                                        score        = sequence.Score;
                                        mass         = sequence.Mass;
                                        charge       = sequence.Charge;
                                        ends         = sequence.TrypticEnds;
                                    }

                                    line += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},",
                                                                feature.GroupID,

                                                                feature.Scan,
                                                                feature.Mz,
                                                                feature.ChargeState,
                                                                feature.MassMonoisotopicMostAbundant,
                                                                findScan,
                                                                pepsequence,
                                                                score,
                                                                mass,
                                                                charge,
                                                                ends);                                
                            }
                            xwriter.WriteLine(line);
                        }

                    }
                }
            }


            //analysis.DataProviders.MSMSClusterCache.AddAll(maps);
            reader.Dispose();
        }

        void extractor_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }

        #region IProgressNotifer Members


        #endregion
    }
}
