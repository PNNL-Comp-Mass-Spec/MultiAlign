using System;
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

namespace MultiAlignCore.IO.Features
{
    public class SICExtractor: IProgressNotifer
    {

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

        /// <summary>
        /// Extracts umc's with MSMS and determines whether to include all MS features or just the ones
        /// with related MSMS.
        /// </summary>
        /// <param name="full"></param>
        private Dictionary<int, List<UMCLight>> MapRemainingMSFeaturesToUMCs(Dictionary<int, List<UMCLight>> features,
                                                                             ThermoRawDataFileReader reader,
                                                                             IMSFeatureToLCMSFeatureDAO msToLCMSFeatureCache,
                                                                             IMSFeatureDAO msFeatureCache)
        {
            // for each dataset
            foreach (int key in features.Keys)
            {
                List<MSFeatureToLCMSFeatureMap> map = msToLCMSFeatureCache.FindByDatasetId(key);
                List<MSFeatureLight> allFeatures = msFeatureCache.FindByDatasetId(key);

                Dictionary<int, MSFeatureLight> featureMap = new Dictionary<int, MSFeatureLight>();
                foreach (MSFeatureLight msFeature in allFeatures)
                {
                    featureMap.Add(msFeature.ID, msFeature);
                }
                Dictionary<int, List<MSFeatureToLCMSFeatureMap>> msFeatureMap = new Dictionary<int, List<MSFeatureToLCMSFeatureMap>>();
                foreach (MSFeatureToLCMSFeatureMap keyMap in map)
                {
                    if (!msFeatureMap.ContainsKey(keyMap.LCMSFeatureID))
                    {
                        msFeatureMap.Add(keyMap.LCMSFeatureID, new List<MSFeatureToLCMSFeatureMap>());
                    }
                    msFeatureMap[keyMap.LCMSFeatureID].Add(keyMap);
                }

                // Look at each feature.                   
                foreach (UMCLight feature in features[key])
                {
                    foreach (MSFeatureToLCMSFeatureMap lcMap in msFeatureMap[feature.ID])
                    {
                        MSFeatureLight xMap = featureMap[lcMap.MSFeatureID];

                        int index = feature.MSFeatures.FindIndex(delegate(MSFeatureLight x)
                        {
                            return x.ID == xMap.ID;
                        });
                        if (index < 0)
                        {
                            feature.MSFeatures.Add(xMap);
                        }
                    }

                    feature.MSFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                    {
                        return x.Scan.CompareTo(y.Scan);
                    }
                    );
                }
            }
            return features;
        }

        /// <summary>
        /// Extracts UMC SIC's for all of the features.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="analysis"></param>
        public void ExtractUMCSICs(string path, MultiAlignAnalysis analysis)
        {
            UpdateStatus("Extracting MSMS data");
            MSMSFeatureExtractor extractor = new MSMSFeatureExtractor();
            Dictionary<int, List<UMCLight>> features = extractor.ExtractUMCWithMSMS(analysis.DataProviders,
                                                                                    analysis.MetaData.Datasets);

            // Create the object that knows how to read the RAW files we are analyzing.
            ThermoRawDataFileReader reader = null;

            if (analysis.MetaData.OtherFiles.Count <= 0)
            {
                UpdateStatus("Raw files specified.");
                reader = new ThermoRawDataFileReader();
                for (int i = 0; i < analysis.MetaData.Datasets.Count; i++)
                {
                    DatasetInformation datasetInformation = analysis.MetaData.Datasets[i];
                    InputFile rawInformation = analysis.MetaData.OtherFiles[i];

                    reader.AddDataFile(rawInformation.Path, datasetInformation.DatasetId);
                }
            }

            UpdateStatus("Completing mapping to rest of the features");
            features = MapRemainingMSFeaturesToUMCs(features,
                                         reader,
                                         analysis.DataProviders.MSFeatureToLCMSFeatureCache,
                                         analysis.DataProviders.MSFeatureCache);

            string sicTitle = "SIC";
            string sicXLabel = "Scan";
            string sicYLabel = "Intensity";

            // Look at each dataset.
            foreach (int datasetID in features.Keys)
            {
                // Look at each feature
                foreach (UMCLight feature in features[datasetID])
                {
                    int totalFeatures = feature.MSFeatures.Count;
                    double[] scans = new double[totalFeatures];
                    double[] intensity = new double[totalFeatures];
                    string baseName = string.Format("-{0}-{1}", datasetID, feature.ID);
                    int i = 0;

                    double maxI = 0;
                    // Separate the MS features into charge state maps so we only look at one m/z
                    foreach (MSFeatureLight msFeature in feature.MSFeatures)
                    {

                        // Build the SIC arrays for plotting
                        scans[i] = msFeature.Scan;
                        intensity[i] = Math.Log(msFeature.Abundance, 2);
                        maxI = Math.Max(intensity[i], maxI);
                        i++;
                    }

                    System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 400, 400);
                    ZedGraph.GraphPane pane = new ZedGraph.GraphPane(rect, sicTitle + string.Format(" {0:0.000}", feature.MassMonoisotopic), sicXLabel, sicYLabel);
                    pane.LineType = ZedGraph.LineType.Normal;
                    //pane.AddCurve("", scans, intensity, System.Drawing.Color.LightGray, ZedGraph.SymbolType.Default);

                    Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);

                    System.Drawing.Color[] colors = new System.Drawing.Color[] {System.Drawing.Color.Green,
                                                                                 System.Drawing.Color.Yellow,
                                                                                 System.Drawing.Color.Blue,
                                                                                 System.Drawing.Color.Orange,
                                                                                 System.Drawing.Color.Purple};
                    foreach (int charge in chargeMap.Keys)
                    {

                        double[] fScans = new double[chargeMap[charge].Count];
                        double[] fIntensities = new double[chargeMap[charge].Count];

                        i = 0;
                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            fScans[i] = msFeature.Scan;
                            fIntensities[i] = Math.Log(msFeature.Abundance, 2);
                            i++;
                        }

                        pane.AddCurve("",
                                        fScans,
                                        fIntensities,
                                        colors[(charge - 1) % colors.Length],
                                        ZedGraph.SymbolType.Square);
                    }

                    using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                    {
                        pane.Fill.Brush = brush;
                        foreach (MSFeatureLight msFeature in feature.MSFeatures)
                        {
                            foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                            {
                                pane.AddStick("",
                                                new double[] { msFeature.Scan, msFeature.Scan },
                                                new double[] { Math.Log(msFeature.Abundance, 2), maxI },
                                                System.Drawing.Color.Red);
                            }
                        }
                    }
                    pane.AxisChange();
                    string fullPath = Path.Combine(path, "SIC" + baseName + ".png");
                    pane.GetImage().Save(fullPath);

                    /*Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);
                    foreach (int charge in chargeMap.Keys)
                    {
                        string baseName     = string.Format("{0}-{1}", feature.ID, charge);
                        int totalFeatures   = chargeMap[charge].Count;
                        double[] scans      = new double[totalFeatures];
                        double[] intensity  = new double[totalFeatures];

                        // Build the SIC arrays for plotting
                        int i = 0;
                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            scans[i]        = msFeature.Scan;
                            intensity[i]    = msFeature.Abundance;
                            i++;
                        }
                        System.Drawing.RectangleF rect  = new System.Drawing.RectangleF(0, 0, 400, 400);
                        ZedGraph.GraphPane pane         = new ZedGraph.GraphPane(rect, sicTitle, sicXLabel, sicYLabel);

                        pane.AddCurve("", scans, intensity, System.Drawing.Color.Black, ZedGraph.SymbolType.Circle);

                        foreach (MSFeatureLight msFeature in chargeMap[charge])
                        {
                            foreach (MSSpectra spectrum in msFeature.MSnSpectra)
                            {
                                pane.AddCurve("",
                                                new double[] { spectrum.Scan },
                                                new double[] { spectrum.TotalIonCurrent },
                                                System.Drawing.Color.Red,
                                                ZedGraph.SymbolType.Square);
                            }
                            i++;
                        }

                        pane.AxisChange();

                        string fullPath = Path.Combine(path, "SIC" + baseName + ".png");
                        pane.GetImage().Save(fullPath);
                    }*/

                }
            }
        }

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion
    }
}
