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

using PNNLOmics.Extensions;

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
                                                                             IMSFeatureToLCMSFeatureDAO msToLCMSFeatureCache,
                                                                             IMSFeatureDAO msFeatureCache)
        {
            // for each dataset
            foreach (int key in features.Keys)
            {
                List<MSFeatureToLCMSFeatureMap> map = msToLCMSFeatureCache.FindByDatasetId(key);
                List<MSFeatureLight> allFeatures    = msFeatureCache.FindByDatasetId(key);

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

        ///// <summary>
        ///// Extracts UMC SIC's for all of the features.
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="analysis"></param>
        //public void ExtractUMCSICsRaw(string path, MultiAlignAnalysis analysis)
        //{
        //    UpdateStatus("Extracting MSMS data");
        //    MSMSFeatureExtractor extractor = new MSMSFeatureExtractor();
        //    extractor.Progress += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
        //    Dictionary<int, List<UMCLight>> features = extractor.ExtractUMCWithMSMS(analysis.DataProviders,
        //                                                                            analysis.MetaData.Datasets);

        //    // Create the object that knows how to read the RAW files we are analyzing.
        //    ThermoRawDataFileReader reader = null;

        //    //if (analysis.MetaData.OtherFiles.Count > 0)
        //    //{
        //    //    UpdateStatus("Raw files specified.");
        //    //    reader = new ThermoRawDataFileReader();
        //    //    for (int i = 0; i < analysis.MetaData.Datasets.Count; i++)
        //    //    {
        //    //        DatasetInformation datasetInformation = analysis.MetaData.Datasets[i];
        //    //        InputFile rawInformation = analysis.MetaData.OtherFiles[i];

        //    //        reader.AddDataFile(rawInformation.Path, datasetInformation.DatasetId);
        //    //    }
        //    //}

        //    UpdateStatus("Completing mapping to rest of the features");
        //    features = MapRemainingMSFeaturesToUMCs(features,                                         
        //                                 analysis.DataProviders.MSFeatureToLCMSFeatureCache,
        //                                 analysis.DataProviders.MSFeatureCache);

        //    UpdateStatus("Exporting SIC's");

        //    // Look at each dataset.
        //    foreach (int datasetID in features.Keys)
        //    {
        //        // Look at each feature


        //        foreach (UMCLight feature in features[datasetID])
        //        {

        //            int min = int.MaxValue;
        //            int max = int.MinValue;
        //            int range = 0;
        //            int extendedMin = int.MaxValue;
        //            int extendedMax = int.MinValue;

        //            int totalFeatures = feature.MSFeatures.Count;
        //            double[] scans = new double[totalFeatures];
        //            double[] intensity = new double[totalFeatures];
        //            string baseName = string.Format("-{0}-{1}", datasetID, feature.ID);
        //            int i = 0;


        //            // Reconstruct the SIC profile for each feature using the RAW data.
        //            feature.MSFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
        //            {
        //                return x.Scan.CompareTo(y.Scan);
        //            });

        //            min = Math.Min(feature.MSFeatures.MinScan(), min);
        //            max = Math.Max(feature.MSFeatures.MaxScan(), max);


        //            int minDelta = int.MaxValue;
        //            for (i = 0; i < feature.MSFeatures.Count - 1; i++)
        //            {
        //                int tempMin = Math.Min(minDelta, Math.Abs(feature.MSFeatures[i].Scan - feature.MSFeatures[i + 1].Scan));
        //                if (tempMin > 0)
        //                {
        //                    minDelta = tempMin;
        //                }
        //            }
        //            range = minDelta * 5;
        //            extendedMin = min - range;
        //            extendedMax = max + range;

        //            i = 0;
        //            double maxI = 0;

        //            // Separate the MS features into charge state maps so we only look at one m/z
        //            foreach (MSFeatureLight msFeature in feature.MSFeatures)
        //            {

        //                // Build the SIC arrays for plotting
        //                scans[i] = msFeature.Scan;
        //                intensity[i] = Math.Log(msFeature.Abundance, 2);
        //                maxI = Math.Max(intensity[i], maxI);
        //                i++;
        //            }


        //            Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);

        //            System.Drawing.Color[] colors = new System.Drawing.Color[] {System.Drawing.Color.Green,
        //                                                                         System.Drawing.Color.Yellow,
        //                                                                         System.Drawing.Color.Blue,
        //                                                                         System.Drawing.Color.Orange,
        //                                                                         System.Drawing.Color.Purple};



        //            foreach (int charge in chargeMap.Keys)
        //            {
        //                List<double> featureScans = new List<double>();
        //                List<double> featureIntensities = new List<double>();

        //                i = 0;
        //                using (TextWriter chargeWriter = File.CreateText(Path.Combine(path, string.Format("sic-{0}-{1}-{2}.csv",
        //                                                                                                datasetID,
        //                                                                                                feature.ID,
        //                                                                                                charge))))
        //                {
        //                    chargeWriter.WriteLine("charge={0}", charge);
        //                    chargeWriter.WriteLine("[data]");
        //                    chargeWriter.WriteLine("mz,scan,intensity");

        //                    List<MSFeatureLight> msFeatures = chargeMap[charge];

        //                    Dictionary<int, MSFeatureLight> map = new Dictionary<int, MSFeatureLight>();

        //                    // Reconstruct the SIC profile for each feature using the RAW data.
        //                    msFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
        //                    {
        //                        return x.Scan.CompareTo(y.Scan);
        //                    });

        //                    // Map the scans.
        //                    foreach (MSFeatureLight x in msFeatures)
        //                    {
        //                        map.Add(x.Scan, x);
        //                    }
        //                    double lastMz = msFeatures[0].Mz;
        //                    double lastIntensity = msFeatures[0].Abundance;
        //                    for (int scan = extendedMin; scan < msFeatures[0].Scan; scan++)
        //                    {
        //                        List<XYData> spectra = reader.GetRawSpectra(scan, datasetID, 1);
        //                        if (spectra == null) continue;

        //                        XYData bestIntensity = spectra.FindByMZ(lastMz);

        //                        featureScans.Add(scan);
        //                        featureIntensities.Add(Math.Log(bestIntensity.Y, 2));
        //                        chargeWriter.WriteLine("{0},{1},{2}", lastMz, scan, bestIntensity.Y);


        //                        lastIntensity = bestIntensity.Y;
        //                    }

        //                    lastMz = msFeatures[0].Mz;
        //                    for (int scan = msFeatures[0].Scan; scan <= msFeatures[msFeatures.Count - 1].Scan; scan++)
        //                    {
        //                        List<XYData> spectra = reader.GetRawSpectra(scan, datasetID, 1);
        //                        if (spectra == null) continue;

        //                        if (map.ContainsKey(scan))
        //                        {
        //                            lastMz = map[scan].Mz;
        //                        }

        //                        XYData bestIntensity = spectra.FindByMZ(lastMz);

        //                        featureScans.Add(scan);
        //                        featureIntensities.Add(Math.Log(bestIntensity.Y, 2));
        //                        chargeWriter.WriteLine("{0},{1},{2}", lastMz, scan, bestIntensity.Y);

        //                        lastIntensity = bestIntensity.Y;
        //                    }
        //                    for (int scan = msFeatures[msFeatures.Count - 1].Scan + 1; scan < extendedMax; scan++)
        //                    {
        //                        List<XYData> spectra = reader.GetRawSpectra(scan, datasetID, 1);
        //                        if (spectra == null) continue;


        //                        XYData bestIntensity = spectra.FindByMZ(lastMz);

        //                        featureScans.Add(scan);
        //                        featureIntensities.Add(Math.Log(bestIntensity.Y, 2));
        //                        chargeWriter.WriteLine("{0},{1},{2}", lastMz, scan, bestIntensity.Y);



        //                        lastIntensity = bestIntensity.Y;
        //                    }
        //                }


        //                double[] fScans = new double[featureScans.Count];
        //                double[] fIntensities = new double[featureIntensities.Count];
        //                featureScans.CopyTo(fScans);
        //                featureIntensities.CopyTo(fIntensities);
        //            }
        //        }
        //    }
        //}

        public void ExtractUMCSICs(string path, MultiAlignAnalysis analysis)
        {
            UpdateStatus("Extracting MSMS data");
            MSMSFeatureExtractor extractor = new MSMSFeatureExtractor();
            extractor.Progress            += new EventHandler<ProgressNotifierArgs>(extractor_Progress);
            Dictionary<int, List<UMCLight>> features = extractor.ExtractUMCWithMSMS(analysis.DataProviders,
                                                                                    analysis.MetaData.Datasets);
            
            UpdateStatus("Completing mapping to rest of the features");
            features = MapRemainingMSFeaturesToUMCs(features,                                         
                                         analysis.DataProviders.MSFeatureToLCMSFeatureCache,
                                         analysis.DataProviders.MSFeatureCache);

            UpdateStatus("Exporting SIC's");
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
                                                          
                    // Separate the MS features into charge state maps so we only look at one m/z                    
                    Dictionary<int, List<MSFeatureLight>> chargeMap = MultiAlignCore.Data.Features.LCMSFeatureChargeMapBuilder.BuildChargeMap(feature);                    
                    foreach (int charge in chargeMap.Keys)
                    {
                        List<double> featureScans = new List<double>();
                        List<double> featureIntensities = new List<double>();

                        using (TextWriter chargeWriter = File.CreateText(Path.Combine(path, string.Format("sic-{0}-{1}-{2}.csv",
                                                                                                        datasetID,
                                                                                                        feature.ID,
                                                                                                        charge))))
                        {
                            chargeWriter.WriteLine("charge={0}", charge);
                            chargeWriter.WriteLine("[data]");
                            chargeWriter.WriteLine("mz,scan,intensity");

                            List<MSFeatureLight> msFeatures     = chargeMap[charge];
                            Dictionary<int, MSFeatureLight> map = new Dictionary<int, MSFeatureLight>();

                            // Reconstruct the SIC profile for each feature using the RAW data.
                            msFeatures.Sort(delegate(MSFeatureLight x, MSFeatureLight y)
                            {
                                return x.Scan.CompareTo(y.Scan);
                            });

                            // Map the scans.
                            foreach (MSFeatureLight x in msFeatures)
                            {
                                map.Add(x.Scan, x);
                            }
                            double lastMz = msFeatures[0].Mz;
                            double lastIntensity = msFeatures[0].Abundance;

                            foreach (MSFeatureLight msFeature in msFeatures)
                            {                                
                                chargeWriter.WriteLine("{0},{1},{2}", msFeature.Mz, msFeature.Scan, msFeature.Abundance);                                
                            }                            
                        }                        
                    }
                }
            }
        }

        void extractor_Progress(object sender, ProgressNotifierArgs e)
        {
            UpdateStatus(e.Message);
        }

        #region IProgressNotifer Members

        public event EventHandler<ProgressNotifierArgs> Progress;

        #endregion
    }
}



