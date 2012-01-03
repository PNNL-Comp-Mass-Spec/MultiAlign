using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Algorithms.FeatureMatcher;
using MultiAlignCore.Algorithms.MSLinker;
using MultiAlignCore.Data;
using MultiAlignCore.Data.Alignment;
using MultiAlignCore.Data.MassTags;
using MultiAlignCore.IO;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.InputFiles;
using MultiAlignCore.IO.Mammoth;
using MultiAlignCore.IO.MTDB;
using MultiAlignEngine.Clustering;
using MultiAlignEngine.Features;
using MultiAlignEngine.MassTags;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.Alignment;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.FeatureMatcher.MSnLinker;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.MassTags;
using PNNLOmicsIO.IO;

namespace MultiAlignCore.IO.Features
{
    /// <summary>
    /// Pulls MS/MS data from the database 
    /// </summary>
    public class MsmsExtractor: IProgressNotifer
    {

        #region IProgressNotifer Members
        public event EventHandler<ProgressNotifierArgs> Progress;
        #endregion

        #region Delegate Handlers / Marshallers
        /// <summary>
        /// Updates listeners with status messages.
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStatus(string message)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressNotifierArgs(message));
            }
        }
        #endregion

        /// <summary>
        /// Extracts the clusters with MS/MS spectra and reports them.
        /// </summary
        /// <param name="providers"></param>
        /// <param name="datasets"></param>
        public FeaturesExtractedEventArgs ExtractMSMS(FeatureDataAccessProviders providers,
                                List<DatasetInformation> datasets)
        {
            UpdateStatus("Producing feature traceback data structures.");
            UpdateStatus("Extracting Clusters from database.");
            List<clsCluster> clusters = providers.ClusterCache.FindAll();
            Dictionary<int, clsCluster> clusterMap = new Dictionary<int, clsCluster>();
            Dictionary<int, List<FeatureExtractionMap>> clusterFeatureMap = new Dictionary<int, List<FeatureExtractionMap>>();
            foreach (clsCluster cluster in clusters)
            {
                clusterMap.Add(cluster.Id, cluster);
                clusterFeatureMap.Add(cluster.Id, new List<FeatureExtractionMap>());
            }

            UpdateStatus("Mapping any matched mass tags to clusters.");
            Dictionary<int, List<ClusterToMassTagMap>> clusterMatches = new Dictionary<int, List<ClusterToMassTagMap>>();
            Dictionary<int, MassTagLight> massTagsMap = new Dictionary<int, MassTagLight>();

            List<ClusterToMassTagMap> matches = providers.MassTagMatches.FindAll();
            foreach (ClusterToMassTagMap map in matches)
            {
                if (!clusterMatches.ContainsKey(map.ClusterId))
                {
                    clusterMatches.Add(map.ClusterId, new List<ClusterToMassTagMap>());
                }
                clusterMatches[map.ClusterId].Add(map);
            }

            UpdateStatus("Finding all mass tags used in analysis.");
            List<MassTagLight> tags = providers.MassTags.FindAll();
            foreach (MassTagLight tag in tags)
            {
                massTagsMap.Add(tag.ID, tag);
            }

            UpdateStatus("Extracting data from each dataset.");
            foreach (DatasetInformation dataset in datasets)
            {
                int datasetID = dataset.DatasetId;

                UpdateStatus(string.Format("Mapping data from dataset {0} with id {1}", dataset.DatasetName, dataset.DatasetId));

                UpdateStatus("Exctracting LC-MS to MS Feature Map");
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
                List<clsUMC> lcmsFeatures = providers.FeatureCache.FindByDatasetId(datasetID);
                Dictionary<int, clsUMC> lcmsFeatureIDToFeature = new Dictionary<int, clsUMC>();
                foreach (clsUMC umc in lcmsFeatures)
                {
                    lcmsFeatureIDToFeature.Add(umc.Id, umc);
                }

                UpdateStatus("Extracting MS Features");
                List<MSFeatureLight> msFeatures = providers.MSFeatureCache.FindByDatasetId(datasetID);
                foreach (MSFeatureLight feature in msFeatures)
                {
                    /// only map things that have clusters.
                    if (!msFeatureIDToMsMsFeatureID.ContainsKey(feature.ID))
                        continue;

                    if (!msFeatureIDToLCMSFeatureID.ContainsKey(feature.ID))
                        continue;

                    int msnID = msFeatureIDToMsMsFeatureID[feature.ID];
                    MSSpectra spectrum = msMsFeatureIDToSpectrum[msnID];
                    int featureID = msFeatureIDToLCMSFeatureID[feature.ID];

                    // This would mean the feature was filtered out.
                    if (!lcmsFeatureIDToFeature.ContainsKey(featureID))
                    {
                        continue;
                    }

                    clsUMC umc = null;
                    try
                    {
                        umc = lcmsFeatureIDToFeature[featureID];
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    int clusterID = umc.ClusterId;

                    FeatureExtractionMap fex = new FeatureExtractionMap();

                    fex.DatasetID = datasetID;

                    fex.MSnScan = spectrum.Scan;
                    fex.MSnRetentionTime = spectrum.RetentionTime;
                    fex.MSnPrecursorMz = spectrum.PrecursorMZ;
                    fex.MSnFeatureId = spectrum.ID;
                    fex.MSnDatasetID = spectrum.GroupID;

                    fex.MSCharge = feature.ChargeState;
                    fex.MSIntensity = feature.Abundance;
                    fex.MSFeatureId = feature.ID;
                    fex.MSMz = feature.Mz;
                    fex.MSScan = feature.Scan;

                    fex.LCMSAbundance = umc.AbundanceMax;
                    fex.LCMSFeatureID = umc.Id;
                    fex.LCMSMass = umc.MassCalibrated;
                    fex.LCMSNet = umc.Net;
                    fex.LCMSScan = umc.Scan;

                    clusterFeatureMap[clusterID].Add(fex);
                }
            }           
            FeaturesExtractedEventArgs args = new FeaturesExtractedEventArgs(clusterFeatureMap,
                                                                                clusterMap,
                                                                                massTagsMap,
                                                                                clusterMatches);

            return args;
        }
    }
}
