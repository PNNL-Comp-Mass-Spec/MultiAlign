using System;
using System.Drawing;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using PNNLControls;
using PNNLOmics.Data.Features;
using PNNLProteomics.IO;
using Mammoth.Data;

namespace FunctionalRedundancy
{
    class Program
    {
        /// <summary>
        /// Path to log file.
        /// </summary>
        private static string m_logPath         = null;
        /// <summary>
        /// Path to database.
        /// </summary>
        private static string m_databasePath    = null;
        /// <summary>
        /// The required datasets.
        /// </summary>
        private static int m_requiredDatasets   = 0;

        /// <summary>
        /// Prints a message to the console and log file.
        /// </summary>
        /// <param name="message"></param>
        static void PrintMessage(string message)
        {
            if (m_logPath != null)
            {
                File.AppendAllText(m_logPath, message + Environment.NewLine);
            }
            Console.WriteLine(message);
        }

        /// <summary>
        /// filters the features by consolidating multiple features from the same dataset to one.
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        static List<UMCLight> ConsolidateFeatures(MammothCluster cluster)
        {
            List<UMCLight> features = new List<UMCLight>();
            Dictionary<int, List<UMCLight>> map = new Dictionary<int, List<UMCLight>>();

            foreach (UMCLight feature in cluster.UMCList)
            {
                bool contains = map.ContainsKey(feature.GroupID);
                if (!contains)
                {
                    map.Add(feature.GroupID, new List<UMCLight>());
                }
                map[feature.GroupID].Add(feature);
            }

            foreach (int key in map.Keys)
            {
                UMCLight prominentFeature = map[key][0];
                foreach (UMCLight featureX in map[key])
                {
                    if (featureX.Abundance > prominentFeature.Abundance)
                    {
                        prominentFeature = featureX;                        
                    }
                }

                features.Add(prominentFeature);
            }

            return features;
        }
        private static clsSeries MakeSeries(List<float> x, List<float> y, Color color, int size, string name = "series")
        {            
            float[] xf = new float[x.Count];
            float[] yf = new float[y.Count];
            x.CopyTo(xf, 0);
            y.CopyTo(yf, 0);

            clsPlotParams p  = new clsPlotParams(new BubbleShape(size, false), color);
            p.Name           = name;
            clsSeries series = new clsSeries(ref xf, ref yf, p);            
            return series;
        }
        private static bool HasEnoughMembers(MammothCluster cluster)
        {
            return cluster.UMCList.Count >= m_requiredDatasets;
        }

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const int LC_DATA   = 0;
        private const int IMS_DATA  = 1;
        /// <summary>
        /// Main entry point for application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string imageName        = "plot";
            IDictionary<string, List<string>> options = CommandLineParser.ProcessArgs(args, 0);            
            foreach(string option in options.Keys)
            {
                try
                {
                    List<string> values     = options[option];
                    switch(option)
                    {
                        case "-database":
                            m_databasePath  = values[0];
                            break;
                        case "-log":
                            m_logPath       = values[0];
                            break;                        
                        case "-image":
                            imageName       = values[0];
                            break;
                        case "-filter":
                            m_requiredDatasets = Convert.ToInt32(values[0]);
                            break;
                    }   
                }
                catch(ArgumentOutOfRangeException)
                {
                    PrintMessage(string.Format("You did not provide enough information for the option {0}", option));
                    return;
                }
            }

            if (options.ContainsKey("-filter"))
            {
                ComputeFR(imageName);
            }
            else
            {
                int      N = 10;
                float [] x = new float[N];
                float [] y = new float[N];

                for(int i = 0; i < N; i++)
                {
                    string name         = imageName + "-" + i.ToString();
                    m_requiredDatasets  = i;
                    y[i] = ComputeFR(name);
                    x[i] = i;
                }
            }
        }

        private static float ComputeFR(string imageName)
        {
            /// Get the clusters and filter them.
            List<MammothCluster> clustersF = new List<MammothCluster>();
            List<MammothCluster> clustersX = new List<MammothCluster>();
            using (Mammoth.Data.MammothDatabase database = new Mammoth.Data.MammothDatabase(m_databasePath))
            {
                database.Connect();

                Mammoth.Data.MammothDatabaseRange range = new Mammoth.Data.MammothDatabaseRange(-100000, 1000000, -100, 1000, -100000, 10000);
                range.SingleChargeState = -1;
                clustersX = database.GetClusters(range);
                clustersF.AddRange(clustersX);
                foreach (MammothCluster cluster in clustersF)
                {
                    cluster.UMCList = ConsolidateFeatures(cluster);
                    cluster.CalculateStatistics(ClusterCentroidRepresentation.Mean);
                }
            }

            List<MammothCluster> clusters = clustersF.FindAll(new Predicate<MammothCluster>(HasEnoughMembers));
            Dictionary<int, string> groupMap = new Dictionary<int, string>();

            int totalDatasets = 0;
            /// Get the factor data.
            using (SQLiteConnection connection = new SQLiteConnection("Data Source = " + m_databasePath + ";"))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT DISTINCT * FROM T_FACTORS";

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = new object[4];
                            reader.GetValues(values);
                            string factorValue = Convert.ToString(values[3]);
                            int groupID = Convert.ToInt32(values[1]);
                            bool contains = groupMap.ContainsKey(groupID);

                            totalDatasets++;    // This is the total number of datasets counter

                            if (!contains)
                            {
                                groupMap.Add(groupID, factorValue);
                            }
                        }
                    }
                }
                connection.Close();
            }

            List<float> x = new List<float>();
            List<float> y = new List<float>();
            List<float> xtotal = new List<float>();
            List<float> ytotal = new List<float>();

            float meanFunctionalRedundancy = 0;

            float binSize = .01F;
            int N = Convert.ToInt32(2.0F / binSize);
            int[] frCounts = new int[N];
            int[] paCounts = new int[N];
            float[] bins = new float[N];

            for (int i = 0; i < N; i++)
            {
                bins[i] = -1.0F + binSize * i;
            }

            double sumIntensity  = 0;
            double meanIntensity = 0;
            double sigma         = 0;
            long totalFeatures = 0;
            foreach (MammothCluster cluster in clusters)
            {
                foreach (UMCLight feature in cluster.UMCList)
                {
                    sumIntensity += feature.Abundance;
                    totalFeatures++;
                }                
            }

            meanIntensity = sumIntensity / Convert.ToDouble(totalFeatures);

            sumIntensity  = 0;
            foreach (MammothCluster cluster in clusters)
            {
                foreach (UMCLight feature in cluster.UMCList)
                {
                    double diff = feature.Abundance - meanIntensity;
                    sumIntensity = (diff*diff);                    
                }
            }

            sigma = sumIntensity / totalFeatures;
            sigma = Math.Sqrt(sigma);

            foreach (MammothCluster cluster in clusters)
            {
                // PA = presence absence count.
                Dictionary<string, int> paCount = new Dictionary<string, int>();
                Dictionary<string, float> intensity = new Dictionary<string, float>();

                // Build the dataset specific items for each factor.
                foreach (UMCLight feature in cluster.UMCList)
                {
                    string factor = groupMap[feature.GroupID];
                    if (!paCount.ContainsKey(factor))
                    {
                        paCount.Add(factor, 0);
                    }
                    if (!intensity.ContainsKey(factor))
                    {
                        intensity.Add(factor, 0);
                    }

                    paCount[factor]++;

                    float zscore = (feature.Abundance - Convert.ToSingle(meanIntensity)) /  Convert.ToSingle(sigma);
                    intensity[factor] += zscore;
                }
                // Calculate the mean intensity.
                string[] keys = new string[intensity.Keys.Count];
                intensity.Keys.CopyTo(keys, 0);


                int countA = 0;
                int countB = 0;
                int total = 0;
                float meanA = 0;
                float meanB = 0;

                string keyA = "AM";
                string keyB = "PM";
                if (intensity.ContainsKey(keyA))
                {
                    countA = Convert.ToInt32(paCount[keyA]);
                    meanA = Convert.ToSingle(intensity[keyA]) / Convert.ToSingle(countA);
                }
                if (intensity.ContainsKey(keyB))
                {
                    countB = Convert.ToInt32(paCount[keyB]);
                    meanB = Convert.ToSingle(intensity[keyB]) / Convert.ToSingle(countB);
                }

                total = countA + countB;
                float presenceAbsence = 0;
                float diffAB = Convert.ToSingle(countA - countB);
                if (total > 0)
                {
                    presenceAbsence = diffAB / Convert.ToSingle(total);
                }
                x.Add(presenceAbsence);


                float functionRedundancy = 0;
                if (meanA + meanB > 0)
                {
                    functionRedundancy = (meanA - meanB) / (meanA + meanB);
                }
                y.Add(functionRedundancy);

                float presenceTotal = 0;
                if (totalDatasets > 0)
                {
                    presenceTotal = diffAB / totalDatasets;
                }

                xtotal.Add(presenceTotal);
                ytotal.Add(functionRedundancy);

                meanFunctionalRedundancy += functionRedundancy;


                int bin = Convert.ToInt32((functionRedundancy + 1.0F) / binSize);                
                bin     = Math.Max(0, Math.Min(frCounts.Length - 1, bin));
                frCounts[bin]++;
                bin     = Convert.ToInt32((presenceTotal + 1.0F) / binSize); 
                paCounts[bin]++;
            }
            meanFunctionalRedundancy /= Convert.ToSingle(clusters.Count);

            if (xtotal.Count > 0)
            {
                ctlScatterChart chart = new ctlScatterChart();
                chart.Width = 800;
                chart.Height = 800;
                chart.Title = string.Format("Global Redundancy (Total Clusters: {0} - Mean FR - {1:0.000})",
                                                                                        clusters.Count,
                                                                                        meanFunctionalRedundancy);
                chart.XAxisLabel  = "Presence/Absence";
                chart.YAxisLabel  = "Functional Redundancy";
                clsSeries seriesB = MakeSeries(xtotal, ytotal, Color.Blue, 1, "FR/Datasets");
                clsSeries seriesA = MakeSeries(x, y, Color.Red, 1, "FR");

                chart.AddSeries(seriesA);
                chart.AddSeries(seriesB);
                chart.AutoViewPort();
                chart.ViewPort    = new RectangleF(-1.1F, -1.1F, 2.2F, 2.2F);
                chart.Refresh();

                System.Drawing.Image image = chart.ToBitmap();
                image.Save(imageName + ".png");

                int maxFR  = int.MinValue;
                int minFR  = int.MaxValue;
                for (int i = 0; i < N; i++)
                {
                    maxFR  = Math.Max(maxFR, frCounts[i]);
                    minFR  = Math.Min(minFR, frCounts[i]);
                }
                float[] ffrCounts = new float[N];
                frCounts.CopyTo(ffrCounts, 0);

                MultiAlign.Charting.controlHistogram histogram = new MultiAlign.Charting.controlHistogram();
                histogram.AddData(bins, ffrCounts, "Functional Redundancy Histogram");
                histogram.Title     = string.Format("Functional Redundancy (Total Clusters: {0})", clusters.Count); 
                histogram.BinSize   = binSize;                
                histogram.Width     = 800;
                histogram.Height    = 800;
                histogram.ViewPort  = new RectangleF(-1.1F, 0.0F, 2.2F, 1000); // Convert.ToSingle(maxFR));
                image = histogram.ToBitmap();
                image.Save(imageName + "-FRHistogram.png");

                maxFR = int.MinValue;
                minFR = int.MaxValue;
                for (int i = 0; i < N; i++)
                {
                    maxFR = Math.Max(maxFR, paCounts[i]);
                    minFR = Math.Min(minFR, paCounts[i]);
                }
                float[] parCounts = new float[N];
                paCounts.CopyTo(parCounts, 0);

                histogram           = new MultiAlign.Charting.controlHistogram();
                histogram.AddData(bins, parCounts, "Presence / Absence Histogram");
                histogram.Title     = string.Format("Presence Absence (Total Clusters: {0})", clusters.Count);
                histogram.BinSize   = binSize;
                histogram.Width     = 800;
                histogram.Height    = 800;
                histogram.ViewPort  = new RectangleF(-1.1F, 0.0F, 2.2F, Convert.ToSingle(maxFR));
                image = histogram.ToBitmap();
                image.Save(imageName + "-PAHistogram.png");                
            }

            return meanFunctionalRedundancy;
        }

    }
}
