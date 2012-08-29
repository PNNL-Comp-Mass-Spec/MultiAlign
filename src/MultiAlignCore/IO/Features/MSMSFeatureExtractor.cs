using System;
using System.Collections.Generic;
using MultiAlignCore.Data;
using MultiAlignEngine.Features;
using PNNLOmics.Algorithms;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace MultiAlignCore.IO.Features
{
    public class MSMSFeatureExtractor: IProgressNotifer
    {
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Extracts the features from the database that have MS/MS
        /// </summary>
        public Dictionary<int, List<UMCLight>> ExtractUMCWithMSMS(FeatureDataAccessProviders providers,
                                                                    List<DatasetInformation> datasets)
        {
            UpdateStatus("Producing feature traceback data structures.");

            UpdateStatus("Extracting data from each dataset.");
             
            Dictionary<int, List<UMCLight>> features = new Dictionary<int, List<UMCLight>>();

            foreach (DatasetInformation dataset in datasets)
            {
                features.Add(dataset.DatasetId, new List<UMCLight>());

                int datasetID = dataset.DatasetId;
                UpdateStatus(string.Format("Mapping data from dataset {0} with id {1}", dataset.DatasetName, dataset.DatasetId));

                UpdateStatus("Extracting LC-MS to MS Feature Map");
                Dictionary<int, int> msFeatureIDToLCMSFeatureID = new Dictionary<int, int>();
                List<MSFeatureToLCMSFeatureMap> msToLcmsFeatureMaps = providers.MSFeatureToLCMSFeatureCache.FindByDatasetId(datasetID);
                foreach (MSFeatureToLCMSFeatureMap map in msToLcmsFeatureMaps)
                {
                    msFeatureIDToLCMSFeatureID.Add(map.MSFeatureID, map.LCMSFeatureID);
                }

                UpdateStatus("Extracting MS to MSn Feature Map");
                List<MSFeatureToMSnFeatureMap> msnToMSFeatureMaps = providers.MSFeatureToMSnFeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, int> msFeatureIDToMsMsFeatureID = new Dictionary<int, int>();
                foreach (MSFeatureToMSnFeatureMap map in msnToMSFeatureMaps)
                {
                    msFeatureIDToMsMsFeatureID.Add(map.MSFeatureID, map.MSMSFeatureID);
                }

                UpdateStatus("Extracting MSn Features");
                Dictionary<int, MSSpectra> msMsFeatureIDToSpectrum = new Dictionary<int, MSSpectra>();
                List<MSSpectra> msnSpectra = providers.MSnFeatureCache.FindByDatasetId(datasetID);
                foreach (MSSpectra spectrum in msnSpectra)
                {
                    msMsFeatureIDToSpectrum.Add(spectrum.ID, spectrum);
                }

                UpdateStatus("Extracting LC-MS Features");
                List<UMCLight> lcmsFeatures = providers.FeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, UMCLight> lcmsFeatureIDToFeature = new Dictionary<int, UMCLight>();
                foreach (UMCLight umc in lcmsFeatures)
                {                    
                    lcmsFeatureIDToFeature.Add(umc.ID, umc);
                }

                UpdateStatus("Extracting MS Features");
                List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByDatasetId(datasetID);

                // Tracks that the UMC was found already.
                Dictionary<int, UMCLight> foundUMC = new Dictionary<int, UMCLight>();

                UpdateStatus("Mapping data.");
                foreach (MSFeatureLight feature in msFeatures)
                {
                    if (!msFeatureIDToMsMsFeatureID.ContainsKey(feature.ID))
                        continue;

                    if (!msFeatureIDToLCMSFeatureID.ContainsKey(feature.ID))
                        continue;

                    // Map the MS/MS feature.
                    int msnID = msFeatureIDToMsMsFeatureID[feature.ID];
                    MSSpectra spectrum = msMsFeatureIDToSpectrum[msnID];
                    spectrum.GroupID = datasetID;
                    int featureID = msFeatureIDToLCMSFeatureID[feature.ID];

                    // This would mean the feature was filtered out.
                    if (!lcmsFeatureIDToFeature.ContainsKey(featureID))
                    {
                        continue;
                    }

                    feature.MSnSpectra.Add(spectrum);

                    // Map to the UMC 
                    UMCLight umc = null;
                    umc = lcmsFeatureIDToFeature[featureID];

                    feature.GroupID = datasetID;
                    umc.AddChildFeature(feature);

                    // Only add if we found the feature once before.
                    if (!foundUMC.ContainsKey(umc.ID))
                    {
                        features[datasetID].Add(umc);
                        foundUMC.Add(umc.ID, umc);
                    }
                }
            }
            return features;
        }
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
    }
}
