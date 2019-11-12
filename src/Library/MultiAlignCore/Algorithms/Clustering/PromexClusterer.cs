using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureAlignment.Algorithms;
using FeatureAlignment.Data.Features;
using MultiAlignCore.Data.Features;
using MultiAlignCore.IO.RawData;
using InformedProteomics.Backend.Utils;
using InformedProteomics.FeatureFinding.Alignment;
using InformedProteomics.FeatureFinding.Data;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;

namespace MultiAlignCore.Algorithms.Clustering
{
    public class PromexClusterer : IClusterer<UMCLight, UMCClusterLight>
    {
        private Dictionary<Tuple<int, int>, UMCLight> featureMap;

        private readonly Dictionary<int, int> promexToMultiAlignDatasetIdMap;

        private readonly Dictionary<int, int> multiAlignToPromexDatasetIdMap;

        private int maxFeatureId;

        public PromexClusterer()
        {
            this.maxFeatureId = 0;
            this.promexToMultiAlignDatasetIdMap = new Dictionary<int, int>();
            this.multiAlignToPromexDatasetIdMap = new Dictionary<int, int>();
        }

        public ScanSummaryProviderCache Readers { get; set; }

        /// <summary>
        /// The progress event.
        /// </summary>
        [Obsolete("Never triggered. Use IProgress parameter instead.")]
        public event EventHandler<ProgressNotifierArgs> Progress;

        /// <summary>
        /// Gets the clusterer paramaters.
        /// These are not actually used by the Promex clusterer right now.
        /// </summary>
        public FeatureClusterParameters<UMCLight> Parameters { get; set; }

        public List<UMCClusterLight> Cluster(List<UMCLight> data, List<UMCClusterLight> clusters, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        public List<UMCClusterLight> Cluster(List<UMCLight> data, IProgress<PRISM.ProgressData> progress = null)
        {
            progress = progress ?? new Progress<PRISM.ProgressData>();

            if (data.Count == 0)
            {
                return new List<UMCClusterLight>();
            }

            this.maxFeatureId = data.Select(d => d.Id).Max();

            this.featureMap = new Dictionary<Tuple<int, int>, UMCLight>();
            foreach (var feature in data)
            {
                var key = new Tuple<int, int>(feature.GroupId, feature.Id);
                this.featureMap.Add(key, feature);
            }

            var lcmsFeatureAligner =
                new LcMsFeatureAlignment(new LcMsFeatureAlignComparer(new Tolerance(10, ToleranceUnit.Ppm)));

            // Group features by dataset
            var idToFeatures = new Dictionary<int, List<UMCLight>>();
            foreach (var umcLight in data)
            {
                if (!idToFeatures.ContainsKey(umcLight.GroupId))
                {
                    idToFeatures.Add(umcLight.GroupId, new List<UMCLight>());
                }

                idToFeatures[umcLight.GroupId].Add(umcLight);
            }

            // Convert UMCLights to InformedProteomics LcMsFeatures
            foreach (var ds in idToFeatures)
            {
                var lcmsFeatures = new List<LcMsFeature>(ds.Value.Select(this.GetLcMsFeature));
                lcmsFeatureAligner.AddDataSet(ds.Key, lcmsFeatures, this.GetLcMsRun(ds.Key));
            }

            // Perform clustering
            lcmsFeatureAligner.AlignFeatures();

            // Fill in mising features using noise.
            lcmsFeatureAligner.RefineAbundance(-30, progress);

            var clusteredFeatures = lcmsFeatureAligner.GetAlignedFeatures();

            // Convert InformedProteomics clusters to UMCClusterLight
            var clusterId = 0;
            var clusters = new List<UMCClusterLight>();
            foreach (var cluster in clusteredFeatures)
            {
                var firstFeature = cluster.FirstOrDefault(f => f != null);
                if (firstFeature == null)
                {
                    continue;
                }

                var umcCluster = new UMCClusterLight
                {
                    Id = clusterId++,
                };

                int datasetId = 0;  // Promex doesn't keep track of which dataset noise features belong to, so we need to.
                foreach (var feature in cluster)
                {
                    if (feature == null)
                    {
                        continue;
                    }

                    feature.DataSetId = datasetId++;
                    var umc = this.GetUMC(feature);
                    umcCluster.AddChildFeature(umc);
                    umc.SetParentFeature(umcCluster);
                }

                umcCluster.CalculateStatistics(ClusterCentroidRepresentation.Median);
                clusters.Add(umcCluster);
            }

            return clusters;
        }

        public void ClusterAndProcess(List<UMCLight> data, IClusterWriter<UMCClusterLight> writer, IProgress<PRISM.ProgressData> progress = null)
        {
            throw new NotImplementedException();
        }

        private LcMsFeature GetLcMsFeature(UMCLight umcLight)
        {
            var lcmsRun = this.GetLcMsRun(umcLight.GroupId);
            return new LcMsFeature(
                                    umcLight.MassMonoisotopicAligned,
                                    umcLight.ChargeState,
                                    umcLight.Mz,
                                    umcLight.Scan,
                                    umcLight.Abundance,
                                    umcLight.MinCharge,
                                    umcLight.MaxCharge,
                                    umcLight.ScanStart,
                                    umcLight.ScanEnd,
                                    lcmsRun.GetElutionTime(umcLight.ScanStart),
                                    lcmsRun.GetElutionTime(umcLight.ScanEnd),
                                    umcLight.NetStart,
                                    umcLight.NetEnd)
            {
                DataSetId = this.GetPromexDatasetId(umcLight.GroupId),
                FeatureId = umcLight.Id,
            };
        }

        private UMCLight GetUMC(LcMsFeature lcmsFeature)
        {
            UMCLight umc;
            if (lcmsFeature.FeatureId == 0)
            {   // Promex clusterer added a new noise feature (filled in missing data for a cluster.
                umc = new UMCLight
                {
                    GroupId = lcmsFeature.DataSetId,
                    Id = ++this.maxFeatureId,
                    Abundance = lcmsFeature.Abundance,
                    AbundanceSum = lcmsFeature.Abundance,
                    Net = (lcmsFeature.MinNet + lcmsFeature.MaxNet) / 2,
                    NetStart = lcmsFeature.MinNet,
                    NetEnd = lcmsFeature.MaxNet,
                    ChargeState = (lcmsFeature.MinCharge + lcmsFeature.MaxCharge) / 2,
                    MinCharge = lcmsFeature.MinCharge,
                    MaxCharge = lcmsFeature.MaxCharge,
                    MassMonoisotopic = lcmsFeature.Mass,
                    MassMonoisotopicAligned = lcmsFeature.Mass,
                };
            }
            else
            {   // Clustered existing feature.
                var key = new Tuple<int, int>(this.promexToMultiAlignDatasetIdMap[lcmsFeature.DataSetId], lcmsFeature.FeatureId);
                umc = this.featureMap[key];
            }

            return umc;
        }

        private int GetPromexDatasetId(int multiAlignDatasetId)
        {
            if (!this.multiAlignToPromexDatasetIdMap.ContainsKey(multiAlignDatasetId))
            {
                var maxPromexId = this.promexToMultiAlignDatasetIdMap.Keys.Any() ?
                                  this.promexToMultiAlignDatasetIdMap.Keys.Max() :
                                  -1;
                var promexId = maxPromexId + 1;
                this.multiAlignToPromexDatasetIdMap.Add(multiAlignDatasetId, promexId);
                this.promexToMultiAlignDatasetIdMap.Add(promexId, multiAlignDatasetId);
            }

            return this.multiAlignToPromexDatasetIdMap[multiAlignDatasetId];
        }

        /// <summary>
        /// Get the <see cref="LcMsRun" /> for the specified dataset.
        /// </summary>
        /// <param name="groupId">The dataset ID.</param>
        /// <returns>The loaded LcMsRun.</returns>
        private LcMsRun GetLcMsRun(int groupId)
        {
            var ipr = this.Readers.GetScanSummaryProvider(groupId) as InformedProteomicsReader;
            if (ipr == null)
            {
                throw new ArgumentException(string.Format("No valid LcMsRun available for dataset {0}", groupId));
            }

            return ipr.LcMsRun;
        }
    }
}
